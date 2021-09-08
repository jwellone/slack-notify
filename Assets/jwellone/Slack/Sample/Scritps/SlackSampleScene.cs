#define SLACK_SAMPLE_USE_COROUTINE
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
		[SerializeField] private string m_title;
		[SerializeField] private string m_message;
		[SerializeField] private InputField m_channleInputField;
		[SerializeField] private InputField m_titleInputField;
		[SerializeField] private InputField m_messageInputField;

		private string Channel => m_channleInputField.text;
		private string Title => m_titleInputField.text;
		private string Message => m_messageInputField.text;

		private void Start()
		{
			Slack.Provider = new Provider(m_token);

			Debug.Log($"Provider->{Slack.Provider.GetType().FullName}");
			Debug.Log($"Token->{Slack.Token}");
			Debug.Log($"LogBuffer Enabled->{Slack.Provider.LogBuffer.Enabled}");

			m_channleInputField.text = m_channel;
			m_titleInputField.text = m_title;
			m_messageInputField.text = m_message;
		}

		public void OnClickScreenShotAPI()
		{
#if SLACK_SAMPLE_USE_COROUTINE
			var api = new ScreenShotUploadAPI(Channel, Title, Message);
			StartCoroutine(api.Send());
#else
			Slack.Send(new ScreenShotUploadAPI(Channel, Title, Message));
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
			var api = new LogBufferUploadAPI(Channel, Title, Message);
			StartCoroutine(api.Send());
#else
			Slack.Send(new LogBufferUploadAPI(Channel, Title, Message));
#endif
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			CustomInspector.Provider.SetToken(m_token);
		}

		[CustomEditor(typeof(SlackSampleScene))]
		private class CustomInspector : Editor
		{
			public class InspectorProvider : DefaultProvider
			{
				public InspectorProvider() : base(string.Empty)
				{
				}

				public void SetToken(string token)
				{
					Token = token;
				}
			}

			public static InspectorProvider Provider
			{
				get;
				private set;
			} = new InspectorProvider();

			[InitializeOnLoadMethod]
			private static void OnInitializeOnLoadMethod()
			{
				Slack.Provider = Provider;
				EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
				EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
			}

			private static void OnPlayModeStateChanged(PlayModeStateChange state)
			{
				if (state == PlayModeStateChange.EnteredEditMode)
				{
					Slack.Provider = Provider;
				}
			}

			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				var instance = (SlackSampleScene)target;

				EditorGUI.BeginDisabledGroup(Application.isPlaying);
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("ScreenShotAPI"))
				{
					Slack.Send(new ScreenShotUploadAPI(instance.m_channel, instance.m_title, instance.m_message));
				}

				if (GUILayout.Button("MessageAPI"))
				{
					Slack.Send(new MessageAPI(instance.m_channel, instance.m_message));
				}

				if (GUILayout.Button("LogBufferAPI"))
				{
					Slack.Send(new LogBufferUploadAPI(instance.m_channel, instance.m_title, instance.m_message));
				}
				EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
			}
		}
#endif
	}
}
