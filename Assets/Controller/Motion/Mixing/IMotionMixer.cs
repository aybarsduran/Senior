using IdenticalStudios.ProceduralMotion;
using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public interface IMotionMixer
    {
        Transform TargetTransform { get; }
        Vector3 PivotPosition { get; }
        Quaternion PivotRotation { get; }


        IMixedMotion GetMotionOfType(Type type);
        T GetMotionOfType<T>() where T : class, IMixedMotion;
        bool TryGetMotionOfType<T>(out T motion) where T : class, IMixedMotion;

        void AddMixedMotion(IMixedMotion mixedMotion);
        void RemoveMixedMotion(IMixedMotion mixedMotion);

        #region Monobehaviour
        GameObject gameObject { get; }
        Transform transform { get; }
        T GetComponent<T>();
        T GetComponentInChildren<T>(bool inactive = false);
        #endregion
    }
}