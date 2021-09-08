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

#if UNITY_EDITOR
			Texture2D tex = null;
			if (Application.isPlaying)
			{
				tex = ScreenCapture.CaptureScreenshotAsTexture();
			}
			else
			{
				var ssFileName = Application.dataPath + "/../Temp/_ss_upload.png";
				System.IO.File.Delete(ssFileName);

				ScreenCapture.CaptureScreenshot(ssFileName);

				var assembly = typeof(UnityEditor.EditorWindow).Assembly;
				var type = assembly.GetType("UnityEditor.GameView");
				var gameview = UnityEditor.EditorWindow.GetWindow(type);
				gameview.Repaint();

				float wait = 0f, limit = 2f;
				while (wait < limit)
				{
					if (System.IO.File.Exists(ssFileName))
					{
						var bytes = System.IO.File.ReadAllBytes(ssFileName);
						tex = new Texture2D(Screen.width, Screen.height);
						tex.LoadImage(bytes);
						System.IO.File.Delete(ssFileName);
						break;
					}
					wait += Time.deltaTime;
					yield return null;
				}
			}
#else
			var tex = ScreenCapture.CaptureScreenshotAsTexture();
#endif

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
					tex?.EncodeToPNG()));

			if (tex)
			{
#if UNITY_EDITOR
				Texture.DestroyImmediate(tex);
#else
				Texture.Destroy(tex);
#endif
			}
		}
	}
}
