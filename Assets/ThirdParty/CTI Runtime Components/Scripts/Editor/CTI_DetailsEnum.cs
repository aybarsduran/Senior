using UnityEditor;
using UnityEngine;

namespace CTI
{
	public class CTI_DetailsEnum : MaterialPropertyDrawer
	{
		public enum DetailMode
		{
			Disabled = 0,
			Enabled = 1,
			FadeBaseTextures = 2,
			SkipBaseTextures = 3
		}

		private DetailMode m_Status;


		override public void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
		{

			Material material = editor.target as Material;

			m_Status = (DetailMode)((int)prop.floatValue);
			m_Status = (DetailMode)EditorGUI.EnumPopup(position, label, m_Status);
			prop.floatValue = (float)m_Status;

			if (prop.floatValue == 0.0f)
			{
				material.DisableKeyword("GEOM_TYPE_BRANCH");
				material.DisableKeyword("GEOM_TYPE_BRANCH_DETAIL");
				material.DisableKeyword("GEOM_TYPE_FROND");
			}
			else if (prop.floatValue == 1.0f)
			{
				material.EnableKeyword("GEOM_TYPE_BRANCH");
				material.DisableKeyword("GEOM_TYPE_BRANCH_DETAIL");
				material.DisableKeyword("GEOM_TYPE_FROND");
			}
			else if (prop.floatValue == 2.0f)
			{
				material.DisableKeyword("GEOM_TYPE_BRANCH");
				material.EnableKeyword("GEOM_TYPE_BRANCH_DETAIL");
				material.DisableKeyword("GEOM_TYPE_FROND");
			}
			else if (prop.floatValue == 3.0f)
			{
				material.DisableKeyword("GEOM_TYPE_BRANCH");
				material.DisableKeyword("GEOM_TYPE_BRANCH_DETAIL");
				material.EnableKeyword("GEOM_TYPE_FROND");
			}
		}
		public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
		{
			return base.GetPropertyHeight(prop, label, editor);
		}
	}
}