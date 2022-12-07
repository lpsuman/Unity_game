using System;
using Bluaniman.SpaceGame.General;
using Bluaniman.SpaceGame.Input;

namespace Bluaniman.SpaceGame.Player
{
	public interface IInputController : IReadiable
	{
		Controls Controls { get; }
		event Action OnControlsEnabled;
		event Action OnControlsSetupDone;
		IInputProvider<float> GetInputAxisProvider();
		IInputProvider<bool> GetInputButtonsProvider();
	}
}