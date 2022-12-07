using System.Collections.Generic;
using Bluaniman.SpaceGame.General;
using UnityEngine.InputSystem;

namespace Bluaniman.SpaceGame.Player
{
	public interface IInputProvider<T> : IReadiable where T : struct
	{
		int BindInput(InputAction inputAction);
		void FinalizeInputMapping();
		T GetInput(int index);

        public bool AreAnyInputsPresentInterval(int startIndex, int count)
        {
            for (int i = startIndex; i < startIndex + count; i++)
            {
                if (IsInputPresent(i)) { return true; }
            }
            return false;
        }

        bool AreAnyInputsPresent(params int[] indices)
        {
            foreach (int index in indices)
            {
                if (IsInputPresent(index))
                {
                    return true;
                }
            }
            return false;
        }

        bool IsInputPresent(int index) => !EqualityComparer<T>.Default.Equals(GetInput(index), default);
    }
}