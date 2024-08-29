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
        // 에디터 윈도우 생성
        var window = EditorWindow.GetWindow<CurrencyEditor>(false, "Currency Editor");
    }

    [MenuItem("[Sheep]/Sheep/Sheep Editor", false, 102)]
    static void OpenSheepEditor()
    {
        // 에디터 윈도우 생성
        var window = EditorWindow.GetWindow<SheepEditor>(false, "Sheep Editor");
    }

    [MenuItem("[Sheep]/Sheep/Sheep Spawn Rate Editor", false, 103)]
    static void OpenSheepSpawnRateCallEditor()
    {
        // 에디터 윈도우 생성
        var window = EditorWindow.GetWindow<SheepSpawnRateEditor>(false, "Sheep Spawn Rate Editor");
        //window.minSize = WINDOWS_MIN_SIZE;
    }

}
