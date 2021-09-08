using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace jwellone.Slack
{
	public class DefaultProvider : Slack.IProvider
	{
#if UNITY_EDITOR
		private class EditorSender
		{
			private ISlackAPI m_api;
			private IEnumerator m_coroutine;

			public EditorSender(ISlackAPI api)
			{
				m_api = api;
				m_coroutine = m_api.Send();
				EditorApplication.update += Update;
			}

			private void Update()
			{
				if (!m_coroutine.MoveNext())
				{
					m_api = null;
					m_coroutine = null;
					EditorApplication.update -= Update;
				}
			}
		}
#endif
		private class RuntimeSender : MonoBehaviour
		{
			private static RuntimeSender s_instance;
			public static RuntimeSender Instance
			{
				get
				{
					if (s_instance == null)
					{
						s_instance = new GameObject("Slack").AddComponent<RuntimeSender>();
						GameObject.DontDestroyOnLoad(s_instance.gameObject);
					}

					return s_instance;
				}
			}

			private void OnDestroy()
			{
				s_instance = null;
			}
		}

		public ILogBuffer LogBuffer
		{
			get;
			private set;
		}

		private string Token { get; set; }

		public DefaultProvider(string token)
		{
			Token = token;
			LogBuffer = new LogBuffer();
		}

		public virtual string GetToken()
		{
			return Token;
		}

		public void Send<T>(T api) where T : ISlackAPI
		{
#if UNITY_EDITOR
			if (!EditorApplication.isPlaying)
			{
				new EditorSender(api);
				return;
			}
#endif
			RuntimeSender.Instance.StartCoroutine(api.Send());
		}
	}
}
