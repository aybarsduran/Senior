using UnityEngine;

namespace IdenticalStudios
{
	public interface ISleepingPlace
	{
		GameObject gameObject { get; }
		Vector3 SleepPosition { get; }
		Vector3 SleepRotation { get; }
	}
}