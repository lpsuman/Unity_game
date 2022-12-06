using Bluaniman.SpaceGame.General;
using UnityEngine.InputSystem;

namespace Bluaniman.SpaceGame.Player
{
	public interface IInputProvider<T> : IReadiable where T : struct
	{
		void BindInput(InputAction inputAction);
		void FinalizeInputMapping();
		T GetInput(int index);
		bool AreInputsPresent(int startIndex, int count);
	}
}