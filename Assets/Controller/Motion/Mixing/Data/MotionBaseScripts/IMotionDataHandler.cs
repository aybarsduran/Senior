using System;

namespace IdenticalStudios.ProceduralMotion
{
    public interface IMotionDataHandler
    {
        bool IsVisualizing { get; }
        event MotionDataChangedDelegate Changed;


        void ForceRefreshData();
        void AddDataOverride(MotionData data);
        void RemoveDataOverride(MotionData data);
        void RemoveDataOverride(Type dataType);

        T GetData<T>() where T : MotionData;
        bool TryGetData<T>(out T data) where T : MotionData;
    }

    public delegate void MotionDataChangedDelegate(bool forceUpdate);
}