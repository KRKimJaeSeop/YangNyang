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

    [SerializeField]
    private UnlockSheepStorage.StorageData _unlockSheep;
    public UnlockSheepStorage.StorageData UnlockSheep { get { return _unlockSheep; } }


    [SerializeField]
    private UnlockDialogStorage.StorageData _unlockDialog;
    public UnlockDialogStorage.StorageData UnlockDialog { get { return _unlockDialog; } }

    public void ExportAll()
    {
     

#if UNITY_EDITOR
        string folderPath = EditorUtility.SaveFolderPanel("Export startup files to folder", "", "");
        if (folderPath.Length > 0)
        {
            File.WriteAllText($"{folderPath}/{nameof(_user)}.json", JsonUtility.ToJson(_user, true));            
            File.WriteAllText($"{folderPath}/{nameof(_currency)}.json", JsonUtility.ToJson(_currency, true));
            File.WriteAllText($"{folderPath}/{nameof(_unlockSheep)}.json", JsonUtility.ToJson(_unlockSheep, true));
            File.WriteAllText($"{folderPath}/{nameof(_unlockDialog)}.json", JsonUtility.ToJson(_unlockDialog, true));
        }
#endif
    }

}
