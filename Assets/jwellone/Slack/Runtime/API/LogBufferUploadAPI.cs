using System;
using System.Collections;

namespace jwellone.Slack
{
	public class LogBufferUploadAPI : FileUploadAPI
	{
		public LogBufferUploadAPI(string channels, string title, string comment, string fileName = "")
			: base(channels, title, comment, fileName, null)
		{
		}

		protected override IEnumerator OnProcessBeforeSend()
		{
			var fileName = RequestParameter.FileName;
			if (string.IsNullOrEmpty(fileName))
			{
				fileName = "Log" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
			}

			SetParam(new RequestParam(
					RequestParameter.Channels,
					RequestParameter.Title,
					RequestParameter.Comment,
					fileName,
					Slack.Provider.LogBuffer.Bytes));

			yield break;
		}
	}
}
