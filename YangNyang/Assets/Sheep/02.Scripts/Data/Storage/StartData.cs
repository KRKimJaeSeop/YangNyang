using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "StartupData.asset", menuName = "[Sheep]/Data/StartData")]
public class StartData : ScriptableObject
{
    [SerializeField]
    private UserStorage.StorageData _user;
    public UserStorage.StorageData User { get { return _user; } }

    [SerializeField]
    private CurrencyStorage.StorageData _currency;
    public CurrencyStorage.StorageData Currency { get { return _currency; } }




    public void ExportAll()
    {
     

#if UNITY_EDITOR
        string folderPath = EditorUtility.SaveFolderPanel("Export startup files to folder", "", "");
        if (folderPath.Length > 0)
        {
            File.WriteAllText($"{folderPath}/{nameof(_user)}.json", JsonUtility.ToJson(_user, true));            
            File.WriteAllText($"{folderPath}/{nameof(_currency)}.json", JsonUtility.ToJson(_currency, true));            
        }
#endif
    }

}
