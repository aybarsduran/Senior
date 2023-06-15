using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public class PlayerItemContainerUI : ItemContainerUI
    {
		#region Internal
		private enum ContainerCullMode
		{
			AttachOnStart,
			AttachManually
		}
		#endregion

		[SpaceArea]

		[SerializeField]
		private string m_ContainerName;

		[SerializeField]
		private ContainerCullMode m_ContainerCullingMode;


		public void AttachToPlayerContainer()
		{
			var playerContainer = Player.Inventory.GetContainerWithName(m_ContainerName);
			AttachToContainer(playerContainer);
		}

		protected override void OnAttachment()
		{
			if (m_ContainerCullingMode == ContainerCullMode.AttachOnStart)
				AttachToPlayerContainer();
		}
    }
}
