using System;

namespace Bluaniman.SpaceGame.General
{
	public interface IReadiable
	{
		bool IsReady { get; set; }
		event Action OnReady;
		void DoWhenReady(Action action)
		{
			if (IsReady)
			{
				action.Invoke();
			}
			else
			{
				OnReady += action;
			}
		}
	}
}