using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;


public class BaseTable : ScriptableObject
{ 

    [Header("[TableObject Settings]")]
    [SerializeField] protected bool loadJson;
    [Tooltip("ProjectFolder/Assets ���� �н�. ex) '/ProjectName/99.Data/Tables/ExportLanguage.json'")]
    [SerializeField] protected string pathJson; // ������Ʈ���丮/Assets ���� �н�
    [SerializeField, ReadOnly] protected string lastLoadedTime;
    protected char[] stringTrims = new char[] { '[', ']' };
    protected char[] stringSeparators = new char[] { ',' };
    public string LastLoadedTime { get { return lastLoadedTime; } }
    public bool IsJsonLoadable { get { return loadJson; } }
    public bool IsReady { get; protected set; }

    public virtual bool Initialize()
    {
        return true;
    }

    public virtual bool LoadTable()
    {
        return false;
    }

    //public string GetLastLoadedTime()
    //{
    //    return lastLoadedTime;
    //}

    /// <summary>
    /// Application.dataPath(������Ʈ���丮/Assets) �ȿ� �ִ� json ������ �о�´�.
    /// �������̺� ��쿡�� �ٷ� �迭�� ���ĵǾ� �ֱ⿡ {"list": } �������� �ٲپ� �ش�.
    /// ���� : https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity/36244111#36244111
    /// </summary>
    /// <param name="path">Application.dataPath(������Ʈ���丮/Assets)���� �н�</param>
    /// <returns></returns>
    protected string LoadJsonText(string path, bool updateTime = true)
    {
        string res = MakeJasonArrayFromat("list", File.ReadAllText(Application.dataPath + path));
        if (updateTime)
            lastLoadedTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        return res;

    }

    protected string MakeJasonArrayFromat(string arrayKey, string strArray)
    {
        return $"{{ \"{arrayKey}\": {strArray}}}";
    }

    /// <summary>
    /// ex) string("[3,5]") -> array ({3,5})
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    protected int[] ConvertStringToIntArray(string str)
    {
        if (str == string.Empty)
            return null;

        str = str.Trim(stringTrims);

        return Array.ConvertAll<string, int>(str.Split(stringSeparators), int.Parse);
    }

    /// <summary>
    /// ex) string("[aaa,bbb]") -> array ({"aaa","bbb"})
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    protected string[] ConvertJsonToStringArray(string str)
    {
        if (str == string.Empty)
            return null;

        str = str.Trim(stringTrims);

        return str.Split(stringSeparators);
    }

    /// <summary>
    /// �ߺ� üũ.(���Ἲ üũ �� �� ����Ѵ�.)
    /// </summary>
    protected (bool success, int duplicatedIdx) CheckDuplicateCount(List<int> numbers)
    {
        var hashSet = new HashSet<int>();
        for (int i = 0; i < numbers.Count; i++)
        {
            if (!hashSet.Add(numbers[i]))
            {
                Debug.LogWarning($"{GetType()}::{nameof(CheckDuplicateCount)} - �ߺ� ��ȣ �߰�. idx={i}, number={numbers[i]}");
                return (false, i);
            }
        }
        return (true, -1);
    }

    // custom editor���� �ش� ������Ƽ�� �������� �������� �Ҷ� ����Ѵ�.
    static public bool IsTableObjectProperty(string name)
    {
        return (name.Equals("m_Script")
            || name.Equals("loadJson")
            || name.Equals("pathJson")
            || name.Equals("lastLoadedTime"));
    }
}
