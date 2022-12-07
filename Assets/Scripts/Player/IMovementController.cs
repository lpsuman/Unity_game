using System;
using UnityEngine;
using Bluaniman.SpaceGame.General;
using System.Collections.Generic;

namespace Bluaniman.SpaceGame.Player
{
	public interface IMovementController : IReadiable
	{
		public enum MovementControllerInputID : int
        {
			Pitch,
			Yaw,
			Roll,
			ForwardThrust,
			HorizontalThrust,
			VerticalThrust,
			Stop,
			SnapMove
		}

		Dictionary<MovementControllerInputID, int> InputDict { get; }
		MovementData MovementData { get; }
		Vector3 AutomaticHeading { get; set; }

		void SetBindingIndex(MovementControllerInputID bindingID, int inputIndex)
        {
			if (InputDict.ContainsKey(bindingID))
            {
				throw new InvalidOperationException($"Movement controller already has {bindingID} bound to index {InputDict[bindingID]}.");
            }
			InputDict[bindingID] = inputIndex;
		}
	}

	public static class Extension
	{
		public static bool IsButton(this IMovementController.MovementControllerInputID inputID)
		{
			return (int)inputID > 5;
		}
	}
}