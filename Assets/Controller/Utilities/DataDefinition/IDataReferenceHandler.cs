using UnityEngine;

namespace IdenticalStudios
{
    public interface IDataReferenceHandler
	{
		DataDefinitionBase GetDataAtIndex(int index);
		int GetDataCount();
		int GetIdAtIndex(int index);
		int GetIndexOfId(int id);
		GUIContent[] GetAllGUIContents(bool name, bool tooltip, bool icon, GUIContent including = null);
	}
}
