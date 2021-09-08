using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace jwellone.Slack
{
	public interface ISlackAPI
	{
		bool IsDone { get; }
		bool IsSending { get; }
		bool IsSuccess { get; }
		int Timeout { get; set; }
		long ResponseCode { get; }
		string ErrorMessage { get; }
		string Url { get; }
		string Endpoint { get; }
		string Method { get; }
		string Uri { get; }
		event Action<ISlackAPI> SendFinishCallback;

		IEnumerator Send();
		void Abort();
	}

	public abstract class SlackAPI<TRequestParam, TResponseParam> : ISlackAPI where TRequestParam : IRequestParam where TResponseParam : class, IResponseParam
	{
		private static readonly WaitForSeconds Wait = new WaitForSeconds(1.0f);

		private Action<ISlackAPI> m_callback;
		private UnityWebRequest m_request = null;

		public event Action<ISlackAPI> SendFinishCallback
		{
			add
			{
				m_callback += value;
			}
			remove
			{
				m_callback -= value;
			}
		}

		private bool IsAbortRequest
		{
			get;
			set;
		}

		public bool IsDone
		{
			get;
			private set;
		}

		public bool IsSending
		{
			get;
			private set;
		}

		public bool IsSuccess
		{
			get;
			private set;
		}

		public int Timeout
		{
			get;
			set;
		} = 30;

		private int RetryCount
		{
			get;
			set;
		}

		protected virtual int RetryMaxCount
		{
			get { return 3; }
		}

		public long ResponseCode
		{
			get;
			private set;
		}

		public string ErrorMessage
		{
			get;
			protected set;
		}

		public string Url => "https://slack.com/api";
		public abstract string Endpoint { get; }
		public abstract string Method { get; }

		public virtual string Uri
		{
			get
			{
				return string.IsNullOrEmpty(Endpoint) ? Url : UnityWebRequest.UnEscapeURL(Path.Combine(Url, Endpoint));
			}
		}

		public TRequestParam RequestParameter
		{
			get;
			private set;
		}

		public TResponseParam ResponseParameter
		{
			get;
			private set;
		}

		public string Token => Slack.Token;

		public void SetParam(in TRequestParam parameter)
		{
			RequestParameter = parameter;
		}

		public IEnumerator Send()
		{
			if (IsSending)
			{
				Debug.LogWarning("[Slack]実行中に呼び出すことはできません");
				yield break;
			}

			IsSending = true;
			IsAbortRequest = false;
			IsSuccess = false;
			IsDone = false;
			RetryCount = 0;
			ResponseCode = 0;
			var uri = Uri;
			var header = GetHeader();
			var body = GetBodyRawData();

			var before = OnProcessBeforeSend();
			while(before.MoveNext())
			{
				yield return before.Current;
			}

			while (true)
			{
				ErrorMessage = string.Empty;

				m_request = CreateRequest(uri);
				if (header != null)
				{
					foreach (DictionaryEntry entry in header)
					{
						m_request.SetRequestHeader(entry.Key.ToString(), entry.Value.ToString());
					}
				}

				if (body != null)
				{
					m_request.uploadHandler = new UploadHandlerRaw(body);
				}

				m_request.downloadHandler = new DownloadHandlerBuffer();
				m_request.timeout = Timeout;

				RequestLog(uri, header);

				m_request.SendWebRequest();
				while (!m_request.isDone && !IsAbortRequest)
				{
					yield return null;
				}

				if (IsAbortRequest)
				{
					m_request.Abort();
					while (!m_request.isDone)
					{
						yield return null;
					}
				}

				ResponseCode = m_request.responseCode;
				ErrorMessage = m_request.error;
				var isRetry = false;

				if (m_request.isNetworkError)
				{
					isRetry = CanRetry(m_request);
				}
				else if (!m_request.isHttpError)
				{
					try
					{
						ResponseParameter = GetResponseParam(m_request);
						IsSuccess = true;
					}
					catch
					{
						ErrorMessage = "Failed to parse the response parameters.";
					}
				}

				ResponseLog(uri);

				m_request.Dispose();
				m_request = null;

				if (!isRetry || ++RetryCount > RetryMaxCount)
				{
					break;
				}

				yield return Wait;
			}

			var after = OnProcessAfterSend();
			while(after.MoveNext())
			{
				yield return after.Current;
			}

			IsSending = false;

			m_callback?.Invoke(this);
			m_callback = null;
		}

		public void Abort()
		{
			IsAbortRequest = true;
		}

		protected virtual UnityWebRequest CreateRequest(string uri)
		{
			return new UnityWebRequest(uri, Method);
		}

		protected virtual Hashtable GetHeader()
		{
			var hash = new Hashtable();
			hash.Add("Content-Type", "application/json; charset=utf-8");
			hash.Add("Authorization", "Bearer " + Token);
			return hash;
		}

		protected virtual IEnumerator OnProcessBeforeSend()
		{
			yield break;
		}

		protected virtual IEnumerator OnProcessAfterSend()
		{
			yield break;
		}

		protected virtual byte[] GetBodyRawData()
		{
			return Encoding.UTF8.GetBytes(JsonUtility.ToJson(RequestParameter));
		}

		protected virtual TResponseParam GetResponseParam(in UnityWebRequest request)
		{
			return JsonUtility.FromJson<TResponseParam>(request.downloadHandler.text);
		}

		protected virtual bool CanRetry(in UnityWebRequest request)
		{
			if (!request.isNetworkError)
			{
				return false;
			}

			if (request.error.Contains("aborted"))
			{
				return false;
			}

			return true;
		}

		[System.Diagnostics.Conditional("ENABLE_SLACK_API_LOG")]
		private void RequestLog(string uri, in Hashtable header)
		{
			var sb = new System.Text.StringBuilder();
			sb.AppendFormat("<color=#{0}>[Slack]Request({1}) {2}</color>\n", ColorUtility.ToHtmlStringRGB(Slack.RequestLogColor), RetryCount + 1, Endpoint);
			sb.AppendLine(uri);
			if (header != null)
			{
				sb.Append("Header: {");
				var index = 0;
				foreach (DictionaryEntry entry in header)
				{
					var addComma = (index<(header.Keys.Count-1));
					sb.Append($"\n\t{entry.Key}: {entry.Value} {(addComma ? "," : string.Empty)}");
					++index;
				}
				sb.AppendLine("\n}");
			}

			sb.Append($"Body: {JsonUtility.ToJson(RequestParameter, true)}");
			Debug.Log(sb.ToString());
		}

		[System.Diagnostics.Conditional("ENABLE_SLACK_API_LOG")]
		private void ResponseLog(string uri)
		{
			if (IsSuccess)
			{
				Debug.Log(
					$"<color=#{ColorUtility.ToHtmlStringRGB(Slack.ResponseLogColor)}>[Slack]Response({(RetryCount + 1)}) {Endpoint}</color>"
					+ $"\nUri: {uri}"
					+ $"\nMethod: {Method}"
					+ $"\nResponseCode: {ResponseCode}"
					+ $"\nParsedResponseBody: {(ResponseParameter != null ? JsonUtility.ToJson(ResponseParameter, true) : "null")}"
					+ $"\nText: {m_request.downloadHandler.text}"
				);
			}
			else
			{
				Debug.LogError(
					$"[Slack]Response({(RetryCount + 1)}) {Endpoint}"
					+ $"\nUri: {uri}"
					+ $"\nMethod: {Method}"
					+ $"\nResponseCode: {ResponseCode}"
					+ $"\nErrorMessage: {ErrorMessage}"
					+ $"\nText: {m_request.downloadHandler.text}"
				);
			}
		}
	}
}
