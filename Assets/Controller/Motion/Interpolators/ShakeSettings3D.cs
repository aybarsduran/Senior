using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [Serializable]
	public struct ShakeSettings3D
	{
		[Title("Settings")]
		
		[Range(-25, 25f)]
		public float XAmplitude;

		[Range(-25f, 25f)]
		public float YAmplitude;

		[Range(-25f, 25f)]
		public float ZAmplitude;

		[Title("Settings")]

		[Range(0f, 10f)]
		public float Duration;

		[Range(0f, 100f)]
		public float Speed;


		public static ShakeSettings3D Default =>
			new ShakeSettings3D()
			{
				XAmplitude = 0f,
				YAmplitude = 0f,
				ZAmplitude = 0f,
				Duration = 0.2f,
				Speed = 20f
			};
	}
}