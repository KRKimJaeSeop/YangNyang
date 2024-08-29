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
    [Tooltip("ProjectFolder/Assets 안의 패스. ex) '/ProjectName/99.Data/Tables/ExportLanguage.json'")]
    [SerializeField] protected string pathJson; // 프로젝트디렉토리/Assets 안의 패스
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
    /// Application.dataPath(프로젝트디렉토리/Assets) 안에 있는 json 파일을 읽어온다.
    /// 엑셀테이블 경우에는 바로 배열로 정렬되어 있기에 {"list": } 형식으로 바꾸어 준다.
    /// 참조 : https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity/36244111#36244111
    /// </summary>
    /// <param name="path">Application.dataPath(프로젝트디렉토리/Assets)안의 패스</param>
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
    /// 중복 체크.(무결성 체크 할 때 사용한다.)
    /// </summary>
    protected (bool success, int duplicatedIdx) CheckDuplicateCount(List<int> numbers)
    {
        var hashSet = new HashSet<int>();
        for (int i = 0; i < numbers.Count; i++)
        {
            if (!hashSet.Add(numbers[i]))
            {
                Debug.LogWarning($"{GetType()}::{nameof(CheckDuplicateCount)} - 중복 번호 발견. idx={i}, number={numbers[i]}");
                return (false, i);
            }
        }
        return (true, -1);
    }

    // custom editor에서 해당 프로퍼티를 보여주지 않으려고 할때 사용한다.
    static public bool IsTableObjectProperty(string name)
    {
        return (name.Equals("m_Script")
            || name.Equals("loadJson")
            || name.Equals("pathJson")
            || name.Equals("lastLoadedTime"));
    }
}
