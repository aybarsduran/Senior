using UnityEngine;

namespace IdenticalStudios
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField]
        private bool m_SyncXRotation;


        private void LateUpdate()
        {
            if (UnityUtils.CachedMainCamera == null)
                return;

            Quaternion rot = Quaternion.LookRotation(transform.position - UnityUtils.CachedMainCamera.transform.position);

            if (!m_SyncXRotation)
                rot = Quaternion.Euler(0f, rot.eulerAngles.y, 0f);

            transform.rotation = rot;
        }
    }
}