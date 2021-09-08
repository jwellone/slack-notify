using System;
using UnityEngine;

namespace jwellone.Slack
{
    public interface IResponseParam
	{
	}

    public class EmptyResponseParam : IResponseParam
	{
	}

	[Serializable]
	public class CommonResponseParam : IResponseParam
	{
		[SerializeField] private string ok;
		[SerializeField] private string error;

		public string Ok => ok;
		public string Error => error;
	}
}
