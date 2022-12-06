using System;
using UnityEngine;
using Bluaniman.SpaceGame.General;

namespace Bluaniman.SpaceGame.Player
{
	public interface IMovementController : IReadiable
	{
		MovementData MovementData { get; }
		Vector3 AutomaticHeading { get; set; }
	}
}