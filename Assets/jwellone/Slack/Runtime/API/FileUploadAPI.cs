using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace jwellone.Slack
{
	public class FileUploadAPI : SlackAPI<FileUploadAPI.RequestParam, FileUploadAPI.ResponseParam>
	{
		public class RequestParam : IRequestParam
		{
			[SerializeField] private string title;
			[SerializeField] private string initial_comment;
			[SerializeField] private string channels;

			public string Title => title;
			public string Comment => initial_comment;
			public string Channels => channels;
			public string FileName { get; private set; }
			public byte[] Binary { get; private set; }

			public RequestParam(string channels, string title, string comment, string fileName, byte[] binary)
			{
				this.title = title;
				this.initial_comment = comment;
				this.channels = channels;
				this.FileName = fileName;
				this.Binary = binary;
			}
		}

		public class ResponseParam : CommonResponseParam
		{
		}

		public override string Endpoint => "files.upload";
		public override string Method => "POST";

		public FileUploadAPI(string channels, string title, string comment, string fileName, byte[] binary)
		{
			SetParam(new RequestParam(channels, title, comment, fileName, binary));
		}

		protected override UnityWebRequest CreateRequest(string uri)
		{
			var form = new WWWForm();
			form.AddField("token", Token);
			form.AddField("title", RequestParameter.Title);
			form.AddField("initial_comment", RequestParameter.Comment);
			form.AddField("channels", RequestParameter.Channels);
			form.AddBinaryData("file", RequestParameter.Binary, RequestParameter.FileName, "image/png");
			return UnityWebRequest.Post(uri, form);
		}

		protected override Hashtable GetHeader()
		{
			return null;
		}

		protected override byte[] GetBodyRawData()
		{
			return null;
		}
	}
}
