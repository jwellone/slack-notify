using UnityEngine;

namespace jwellone.Slack
{
	public static class Slack
	{
		public interface IProvider
		{
			ILogBuffer LogBuffer { get; }
			string Token { get; }
			void Send<T>(T api) where T : ISlackAPI;
		}

		private class EmptyProvider : DefaultProvider
		{
			public EmptyProvider() : base(string.Empty)
			{
			}
		}

		public static IProvider Provider
		{
			get;
			set;
		} = new EmptyProvider();

		public static string Token => Provider.Token;

		public static Color RequestLogColor
		{
			get;
			set;
		} = Color.cyan;

		public static Color ResponseLogColor
		{
			get;
			set;
		} = Color.cyan;

		public static void Send<T>(T api) where T : ISlackAPI
		{
			Provider.Send(api);
		}

		public static string FormatBold(string text)
		{
			return string.Format("*{0}*", text);
		}

		public static string FormatItalicize(string text)
		{
			return string.Format("_{0}_", text);
		}

		public static string FormatStrikethrough(string text)
		{
			return string.Format("~{0}~", text);
		}

		public static string FormatCode(string text)
		{
			return string.Format("`{0}`", text);
		}

		public static string FormatBlockQuote(string text)
		{
			return string.Format(">{0}", text);
		}

		public static string FormatCodeBlock(string text)
		{
			return string.Format("```{0}```", text);
		}

		public static string FormatLink(string text, string url)
		{
			return string.Format("<{0}|{1}>", url, text);
		}
	}
}
