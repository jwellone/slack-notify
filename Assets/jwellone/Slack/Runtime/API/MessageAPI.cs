using System;
using UnityEngine;

namespace jwellone.Slack
{
	public class MessageAPI : SlackAPI<MessageAPI.RequestParam, MessageAPI.ResponseParam>
	{
		[Serializable]
		public class RequestParam : IRequestParam
		{
			[SerializeField] private string channel;
			[SerializeField] private string text;

			public RequestParam(string channel, string text)
			{
				this.channel = channel;
				this.text = text;
			}
		}

		[Serializable]
		public class ResponseParam : CommonResponseParam
		{
		}

		public override string Endpoint => "chat.postMessage";
		public override string Method => "POST";

		public MessageAPI(string channel, string text)
		{
			SetParam(new RequestParam(channel, text));
		}
	}
}
