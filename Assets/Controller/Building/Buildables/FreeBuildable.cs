namespace IdenticalStudios.BuildingSystem
{
    public class FreeBuildable : Buildable
    {
		public override void OnCreated(bool playEffects = true)
		{
			EnableColliders(false);

			gameObject.SetLayerRecursively(BuildingManager.BuildablePreviewLayer);
			MaterialEffect.EnableCustomEffect(BuildingManager.PlacementAllowedMaterialEffect);
		}

		public override void OnPlaced(bool playEffects = true)
		{
			EnableColliders(true);

			if (playEffects)
				DoPlacementEffects(); 
		}

		public override void OnBuilt(bool playEffects = true)
		{
			gameObject.SetLayerRecursively(BuildingManager.BuildableLayer);
			MaterialEffect.DisableActiveEffect();

			if (playEffects)
				DoBuildEffects();
		}
	}
}