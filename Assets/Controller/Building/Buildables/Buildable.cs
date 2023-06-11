using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    [RequireComponent(typeof(MaterialEffect))]
    public abstract class Buildable : Placeable, ISaveableComponent
	{
		public BuildableDefinition Definition => m_Definition.Def;
		public MaterialEffect MaterialEffect => GetComponent<MaterialEffect>();

		//Settings (Buildable)

		[SerializeField, DataReferenceDetails(HasAssetReference = true, HasNullElement = false)]
		private DataIdReference<BuildableDefinition> m_Definition;


		[SerializeField]
		private GameObject m_BuildFX;

		[SerializeField]
		private SoundPlayer m_BuildAudio;


		public abstract void OnCreated(bool playEffects = true);
		public abstract void OnPlaced(bool playEffects = true);
		public abstract void OnBuilt(bool playEffects = true);

		protected void DoBuildEffects()
		{
			m_BuildAudio.Play2D();

			if (m_BuildFX != null)
				Instantiate(m_BuildFX, transform.position, Quaternion.identity, null);
		}

        #region Save & Load
        public virtual void LoadMembers(object[] members) { }
		public virtual object[] SaveMembers() => null;
		#endregion
	}
}
