using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorMenu : MonoBehaviour
{
    private const string SETTINGS_PATH = "Assets/Sheep/00.Settings/";
    private const string DATA_PATH = "Assets/Sheep/99.Data/";
    private const string TABLES_PATH = "Assets/Sheep/99.Data/Tables/";


    [MenuItem("[Sheep]/Currency Editor", false, 101)]
    static void OpenCurrencyEditor()
    {
        // ������ ������ ����
        var window = EditorWindow.GetWindow<CurrencyEditor>(false, "Currency Editor");
    }

}
