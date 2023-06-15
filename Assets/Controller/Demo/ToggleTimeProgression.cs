using IdenticalStudios.WorldManagement;
using UnityEngine;

namespace IdenticalStudios.Demo
{
    public class ToggleTimeProgression : MonoBehaviour
    {
        public void ToggleTimeProgress()
        {
            WorldManagerBase.Instance.TimeProgressionEnabled = !WorldManagerBase.Instance.TimeProgressionEnabled;
        }
    }
}