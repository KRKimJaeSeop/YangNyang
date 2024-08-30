using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorMenu : MonoBehaviour
{
    private const string SETTINGS_PATH = "Assets/Sheep/00.Settings/";
    private const string DATA_PATH = "Assets/Sheep/99.Data/";
    private const string TABLES_PATH = "Assets/Sheep/99.Data/Tables/";


    [MenuItem("[Sheep]/Start Data", false, 11)]
    static void SelectStartData()
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath(TABLES_PATH + "StartDatas/StartupData.asset", typeof(StartData));

    }

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
    }

    [MenuItem("[Sheep]/Day Status Editor", false, 201)]
    static void SelectDayStatusTable()
    {
        // 에디터 윈도우 생성
        var window = EditorWindow.GetWindow<DayStatusEditor>(false, "Day Status Editor");
    }
    [MenuItem("[Sheep]/Research Editor", false, 202)]
    static void SelectResearchTable()
    {
        // 에디터 윈도우 생성
        var window = EditorWindow.GetWindow<ResearchEditor>(false, "Research Editor");
    }

    [MenuItem("[Sheep]/Storage Editor", false, 9999)]
    private static void OpenEditorWindow()
    {
        // 에디터 윈도우 생성
        var window = EditorWindow.GetWindow<StorageEditor>(false, "Storage Editor");
        //window.Show();
    }

}
