namespace IdenticalStudios.UISystem
{
    public abstract class DataInfoUI<DataType> : DataInfoBaseUI where DataType : class
    {
        protected DataType data;


        public void SetData(DataType newData)
        {
            if (data != null && data == newData)
                OnInfoUpdate();
            else
            {
                data = newData;

                if (data != null)
                {
                    if (CanEnableInfo())
                    {
                        OnInfoUpdate();
                        return;
                    }
                }

                OnInfoDisabled();
            }
        }

        protected abstract bool CanEnableInfo();
        protected abstract void OnInfoUpdate();
        protected abstract void OnInfoDisabled();

#if UNITY_EDITOR
        protected virtual void OnValidate() { }
#endif
    }
}
