#define SLACK_SAMPLE_USE_COROUTINE
using UnityEngine;
using UnityEngine.UI;

namespace jwellone.Slack.Sample
{
	public class SlackSampleScene : MonoBehaviour
	{
		private class Provider : DefaultProvider
		{
			public Provider(string token) : base(token)
			{
			}
		}

		[SerializeField] private string m_token;
		[SerializeField] private string m_channel;
		[SerializeField] private string m_message;
		[SerializeField] private InputField m_channleInputField;
		[SerializeField] private InputField m_messageInputField;

		private string Channel => m_channleInputField.text;

		private string Message => m_messageInputField.text;

		private void Start()
		{
			Slack.Provider = new Provider(m_token);

			Debug.Log($"Provider->{Slack.Provider.GetType().FullName}");
			Debug.Log($"Token->{Slack.GetToken()}");
			Debug.Log($"LogBuffer Enabled->{Slack.Provider.LogBuffer.Enabled}");

			m_channleInputField.text = m_channel;
			m_messageInputField.text = m_message;
		}

		public void OnClickScreenShotAPI()
		{
#if SLACK_SAMPLE_USE_COROUTINE
			var api = new ScreenShotUploadAPI(Channel, "Screen Shot", Message);
			StartCoroutine(api.Send());
#else
			Slack.Send(new ScreenShotUploadAPI(Channel, "Screen Shot", Message));
#endif
		}

		public void OnClickMessageAPI()
		{
#if SLACK_SAMPLE_USE_COROUTINE
			var api = new MessageAPI(Channel, Message);
			StartCoroutine(api.Send());
#else
			Slack.Send(new MessageAPI(Channel, Message));
#endif
		}

		public void OnClickLogBufferAPI()
		{
#if SLACK_SAMPLE_USE_COROUTINE
			var api = new LogBufferUploadAPI(Channel, "Log Buffer", Message);
			StartCoroutine(api.Send());
#else
			Slack.Send(new LogBufferUploadAPI(Channel, "Log Buffer", Message));
#endif
		}
	}
}
