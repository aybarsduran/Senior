using UnityEngine;

namespace CTI
{
    [RequireComponent (typeof (WindZone))]
	public class CTI_CustomWind : MonoBehaviour {

		[SerializeField]
		private float m_WindMultiplier = 1.0f;

		[SerializeField, Min(0f)]
		private float m_DirectionChangeSpeed = 0.01f;

		private WindZone m_WindZone;
		private Vector3 WindDirection;
		private float WindStrength;
		private float WindTurbulence;

	    private int TerrainLODWindPID;


        private void Start()
        {
			m_WindZone = GetComponent<WindZone>();
			TerrainLODWindPID = Shader.PropertyToID("_TerrainLODWind");
		}

        private void Update () 
		{
			if (Application.isPlaying)
				UpdateWindDirection();

			UpdateWindShader();
		}

		private void UpdateWindDirection() 
		{
			transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y + m_DirectionChangeSpeed, 0f);
			WindDirection = transform.forward;
		}

		private void UpdateWindShader() 
		{
			WindStrength = m_WindZone.windMain * m_WindMultiplier;
			WindStrength += m_WindZone.windPulseMagnitude * (1.0f + Mathf.Sin(Time.time * m_WindZone.windPulseFrequency) + 1.0f + Mathf.Sin(Time.time * m_WindZone.windPulseFrequency * 3.0f)) * 0.5f;
			WindTurbulence = m_WindZone.windTurbulence * m_WindZone.windMain * m_WindMultiplier;

			WindDirection *= WindStrength;

			Shader.SetGlobalVector(TerrainLODWindPID, new Vector4(WindDirection.x, WindDirection.y, WindDirection.z, WindTurbulence));
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (m_WindZone == null)
				m_WindZone = GetComponent<WindZone>();

			UpdateWindShader();
		}
#endif
	}
}