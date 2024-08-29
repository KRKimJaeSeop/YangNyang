public abstract class BaseStorage : IStorage
{
    public bool IsLoaded { get; protected set; }
    public bool IsDirty { get; protected set; }

    #region IStorage

    public virtual void Clear()
    {
        IsLoaded = false;
        IsDirty = false;
    }

    public virtual bool Load()
    {
        IsLoaded = true;
        return IsLoaded;
    }

    public virtual bool Save()
    {
        bool res = IsDirty;
        IsDirty = false;
        return res;
    }
    
    public abstract void Overwrite(string strJson);
    public abstract string ToJson();

    #endregion

    public abstract void Initialize();

    protected void SetDirty()
    {
        IsDirty = true;
    }


}
