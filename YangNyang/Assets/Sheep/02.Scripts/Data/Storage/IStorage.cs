public interface IStorage
{
#if UNITY_EDITOR
    void Clear();
#endif
    bool Load();
    bool Save();
    void Overwrite(string strJson);
    string ToJson();
}

