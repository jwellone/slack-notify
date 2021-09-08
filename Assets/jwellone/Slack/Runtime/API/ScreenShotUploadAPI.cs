using System;
using System.Collections;
using UnityEngine;

namespace jwellone.Slack
{
	public class ScreenShotUploadAPI : FileUploadAPI
	{
		public ScreenShotUploadAPI(string channels, string title, string comment, string fileName = "")
			: base(channels, title, comment, fileName, null)
		{
		}

		protected override IEnumerator OnProcessBeforeSend()
		{
			yield return new WaitForEndOfFrame();

			var width = Screen.width;
			var height = Screen.height;
			var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
			var source = new Rect(0, 0, width, height);
			tex.ReadPixels(source, 0, 0);

			var fileName = RequestParameter.FileName;
			if (string.IsNullOrEmpty(fileName))
			{
				fileName = "ScreenShot" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
			}

			SetParam(new RequestParam(
					RequestParameter.Channels,
					RequestParameter.Title,
					RequestParameter.Comment,
					fileName,
					tex.EncodeToPNG()));

			Texture.Destroy(tex);
		}
	}
}
