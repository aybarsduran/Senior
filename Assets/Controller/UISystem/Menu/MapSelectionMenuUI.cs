using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.UISystem
{
    public sealed class MapSelectionMenuUI : MonoBehaviour
    {
        #region Internal
        [System.Serializable]
        private class MapUI
        {
            public SerializedScene Scene;

            public string Name;
            public Sprite Sprite;
        }
        #endregion

        [SerializeField, Range(0f, 10f)]
        private float m_LoadDelay;

        [SpaceArea]

        [SerializeField]
        private MapSlotUI m_MapSlotTemplate;

        [SerializeField]
        private RectTransform m_SpawnRoot;

        [SpaceArea]

        [SerializeField]
        private UnityEvent m_SelectMapEvent;

        [SpaceArea]

        [SerializeField, ReorderableList(childLabel: "Name")]
        private MapUI[] m_Maps;

        private bool m_IsLoading;


        private void Awake()
        {
            foreach (var map in m_Maps)
            {
                var template = Instantiate(m_MapSlotTemplate, m_SpawnRoot);

                template.Text.text = map.Name;
                template.Image.sprite = map.Sprite;
                template.Button.onClick.AddListener(() => LoadLevel(map.Scene.SceneName));
            }
        }

        private void LoadLevel(string sceneName) 
        {
            if (!m_IsLoading)
                StartCoroutine(C_LoadLevel(sceneName));
        }

        private IEnumerator C_LoadLevel(string sceneName)
        {
            m_IsLoading = true;
            FadeScreenUI.Instance.Fade(true, 0.2f);
            m_SelectMapEvent.Invoke();
            yield return new WaitForSeconds(m_LoadDelay);
            LevelManager.LoadScene(sceneName);
        }
    }
}