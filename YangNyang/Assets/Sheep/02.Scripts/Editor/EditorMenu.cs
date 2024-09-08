using UnityEditor;
using UnityEngine;

public class EditorMenu : MonoBehaviour
{
    private const string SETTINGS_PATH = "Assets/Sheep/00.Settings/";
    private const string DATA_PATH = "Assets/Sheep/99.Data/";
    private const string TABLES_PATH = "Assets/Sheep/99.Data/Tables/";


    [MenuItem("[Sheep]/Start Data", false, 11)]
    private static void SelectStartData()
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath(TABLES_PATH + "StartDatas/StartupData.asset", typeof(StartData));

    }

    [MenuItem("[Sheep]/Currency Editor", false, 101)]
    private static void OpenCurrencyEditor()
    {
        // ������ ������ ����
        var window = EditorWindow.GetWindow<CurrencyEditor>(false, "Currency Editor");
    }

    [MenuItem("[Sheep]/Sheep/Sheep Editor", false, 102)]
    private static void OpenSheepEditor()
    {
        // ������ ������ ����
        var window = EditorWindow.GetWindow<SheepEditor>(false, "Sheep Editor");
    }

    [MenuItem("[Sheep]/Sheep/Sheep Spawn Rate Editor", false, 103)]
    private static void OpenSheepSpawnRateCallEditor()
    {
        // ������ ������ ����
        var window = EditorWindow.GetWindow<SheepSpawnRateEditor>(false, "Sheep Spawn Rate Editor");
    }

    [MenuItem("[Sheep]/Day Status Editor", false, 201)]
    private static void OpenDayStatusTable()
    {
        // ������ ������ ����
        var window = EditorWindow.GetWindow<DayStatusEditor>(false, "Day Status Editor");
    }
    [MenuItem("[Sheep]/Research Editor", false, 202)]
    private static void OpenResearchTable()
    {
        // ������ ������ ����
        var window = EditorWindow.GetWindow<ResearchEditor>(false, "Research Editor");
    }
    [MenuItem("[Sheep]/Dialog Editor", false, 301)]
    private static void OpenDialogEditor()
    {
        // ������ ������ ����
        var window = EditorWindow.GetWindow<DialogEditor>(false, "DialogEditor Editor");
    }

    [MenuItem("[Sheep]/Storage Editor", false, 9999)]
    private static void OpenEditorWindow()
    {
        // ������ ������ ����
        var window = EditorWindow.GetWindow<StorageEditor>(false, "Storage Editor");
        //window.Show();
    }

}
