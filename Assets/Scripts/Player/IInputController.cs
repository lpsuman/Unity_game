using System;
using Bluaniman.SpaceGame.General;

namespace Bluaniman.SpaceGame.Player
{
	public interface IInputController : IReadiable
	{
		event Action OnControlsEnabled;
		event Action OnControlsSetupDone;
		IInputProvider<float> GetInputAxisProvider();
		IInputProvider<bool> GetInputButtonsProvider();
	}
}