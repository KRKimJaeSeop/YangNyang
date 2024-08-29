using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "StartupData.asset", menuName = "[Sheep]/Data/StartData")]
public class StartData : ScriptableObject
{
    [SerializeField]
    private CurrencyStorage.StorageData currency;
    public CurrencyStorage.StorageData Currency { get { return currency; } }
    public void ExportAll()
    {
     

#if UNITY_EDITOR
        string folderPath = EditorUtility.SaveFolderPanel("Export startup files to folder", "", "");
        if (folderPath.Length > 0)
        {
            File.WriteAllText($"{folderPath}/{nameof(currency)}.json", JsonUtility.ToJson(currency, true));            
        }
#endif
    }

}
