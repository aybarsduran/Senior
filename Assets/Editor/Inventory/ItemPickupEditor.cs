using UnityEditor;

namespace IdenticalStudios.InventorySystem
{
    [CustomEditor(typeof(ItemPickup), true)]
    public class ItemPickupEditor : InteractableEditor
    {
	    protected override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

			EditorGUILayout.Space();

			if (CustomGUILayout.ColoredButton("Reset", CustomGUIStyles.YellowColor))
			{
				string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot((ItemPickup)target);

				ItemPickup itemPickup;

				if (!string.IsNullOrEmpty(path))
					itemPickup = AssetDatabase.LoadAssetAtPath<ItemPickup>(path);
				else
					itemPickup = (ItemPickup)target;

				if (itemPickup == null)
					return;

				foreach (var def in ItemDefinition.Definitions)
				{
					if (def.Pickup == itemPickup)
					{
						target.SetFieldValue("m_Item", new DataIdReference<ItemDefinition>(def));
						break;
					}
				}
				
				EditorUtility.SetDirty(target);
			}
		}
    }
}
