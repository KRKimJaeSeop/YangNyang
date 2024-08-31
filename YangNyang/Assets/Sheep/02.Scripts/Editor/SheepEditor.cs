using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class SheepEditor : EditorWindow
{
    private const string EDITORPREFS_SHEEP_EDITOR = "Sheep_SheepEditor";

    [Serializable]
    public class ShowProperty
    {
        public bool file = true;
        public bool properties = true;
        public bool table = true;
    }
    [Serializable]
    private class EditorDataProperty
    {
        public string tablePath = string.Empty;
        public ShowProperty show = new ShowProperty();
    }

    private Vector2 _scrollPos;
    private EditorDataProperty _editorData = new EditorDataProperty();

    private SheepTable _table;
    private SerializedObject _soTable;
    private SheepTableUnit _tbUnit;
    private ReorderableList _reorderable;

    private Sheep.Type _currType = Sheep.Type.None;
    private string _newUnitName = string.Empty;

    //[MenuItem("Sample/Sample Editor", false, 3)]
    //static void ShowEditorWindow()
    //{
    //    // ������ ������ ����
    //    SheepEditorWindow window = (SheepEditorWindow)GetWindow(typeof(SheepEditorWindow));
    //}

    void OnEnable()
    {
        if (EditorPrefs.HasKey(EDITORPREFS_SHEEP_EDITOR))
        {
            string strData = EditorPrefs.GetString(EDITORPREFS_SHEEP_EDITOR);
            _editorData = JsonUtility.FromJson<EditorDataProperty>(strData);

            LoadAsset(_editorData.tablePath);
        }

        //// "target" can be any class derrived from ScriptableObject 
        //// (could be EditorWindow, MonoBehaviour, etc)
        //ScriptableObject target = this;
        //_soTarget = new SerializedObject(target);
    }

    void OnGUI()
    {
        // window ��ũ�� ����
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);

        UpdateFileMenu();
        CommonEditorUI.DrawSeparator(Color.black);
        if (_table != null)
        {
            _soTable.Update();

            UpdatePlayModeProperties();
            CommonEditorUI.DrawSeparator(Color.black);
            UpdateSheepTable();
            CommonEditorUI.DrawSeparator(Color.black);

            _soTable.ApplyModifiedProperties(); // Remember to apply modified properties
        }

        // window ��ũ�� ����
        EditorGUILayout.EndScrollView();
    }

    private void SaveEditorData()
    {
        string strData = JsonUtility.ToJson(_editorData);
        EditorPrefs.SetString(EDITORPREFS_SHEEP_EDITOR, strData);
    }


    #region File Menu
    bool LoadAsset(string path)
    {
        _table = AssetDatabase.LoadAssetAtPath(path, typeof(SheepTable)) as SheepTable;
        if (_table != null)
        {
            // set serialized object
            _soTable = new SerializedObject(_table);

            // set reorderable list
            SetList(_table, _soTable);
        }
        else
        {
            Debug.LogError($"{GetType()}::{nameof(LoadAsset)} - Failed load table. path={path}");
            return false;
        }
        return true;
    }
    void OpenSheepListTable()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Sheep Table", "", "asset");
        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            if (LoadAsset(relPath))
            {
                _editorData.tablePath = relPath;
                SaveEditorData();
            }
        }
    }
    private void UpdateFileMenu()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            _editorData.show.file = GUILayout.Toggle(_editorData.show.file, "[File]");

            if (check.changed)
                SaveEditorData();
        }
        if (_editorData.show.file == false)
            return;

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Open Sheep Table Asset"))
            {
                OpenSheepListTable();
            }
            if (_table != null)
            {
                if (GUILayout.Button("Select Sheep Table Asset"))
                {
                    //EditorUtility.FocusProjectWindow();
                    //Selection.activeObject = _tbSheep;
                    EditorGUIUtility.PingObject(_table);
                }
            }
        }
    }
    #endregion

    #region Properties
    private void UpdatePlayModeProperties()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            _editorData.show.properties = GUILayout.Toggle(_editorData.show.properties, "[Properties]");

            if (check.changed)
                SaveEditorData();
        }
        if (_editorData.show.properties == false)
            return;

        UpdateSheepProperties();
    }

    private void UpdateSheepProperties()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            //EditorGUILayout.PropertyField(_soTable.FindProperty("adventureRequiredTrainingID"), true);
            //EditorGUILayout.PropertyField(_soTable.FindProperty("adventureRequiredLanguageID"), true);
            //EditorGUILayout.PropertyField(_soTable.FindProperty("adventureBGM"), true);
            //EditorGUILayout.PropertyField(_soTable.FindProperty("adventureContinuePenaltyRate"), true);
            //EditorGUILayout.PropertyField(_soTable.FindProperty("adventureContinuePrice"), true);

            if (check.changed)
                EditorUtility.SetDirty(_table);
        }
    }
    #endregion

    #region Sheep Table
    void SetList(SheepTable table, SerializedObject so)
    {
        _tbUnit = null;

        /// <summary>
        /// ReorderableList 
        /// - https://unityindepth.tistory.com/56
        /// - https://m.blog.naver.com/PostView.nhn?blogId=hammerimpact&logNo=220775710045&proxyReferer=https%3A%2F%2Fwww.google.com%2F
        /// </summary>
        // set ReorderableList
        _reorderable = new ReorderableList(so, so.FindProperty("list"), true, true, true, true);
        _reorderable.drawHeaderCallback =
            (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "<Sheeps>");
            };
        _reorderable.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                float totalWidth = rect.width;
                rect.y += EditorGUIUtility.standardVerticalSpacing;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width = 80;
                var element = _reorderable.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.LabelField(rect, $"Index {index}");
                rect.x += rect.width;
                rect.width = totalWidth - rect.x - 100;
                EditorGUI.PropertyField(rect, element, GUIContent.none); // GUIContent.none : ���� ���� ������ �ʴ´�.
                if (element.objectReferenceValue != null) // ������Ʈ ���� �ִٸ�.
                {
                    var tbUnit = element.objectReferenceValue as SheepTableUnit;

                    rect.x += (rect.width + 20);
                    rect.width = 80;
                    if (GUI.Button(rect, "Edit"))
                    {
                        CreateChildWindow(tbUnit);
                    }
                }
            };
        _reorderable.onSelectCallback =
            (ReorderableList list) =>
            {
                Debug.Log($"{GetType()}::{nameof(SetList)} - onSelectCallback idx={list.index}");
                var element = _reorderable.serializedProperty.GetArrayElementAtIndex(list.index);
                if (element.objectReferenceValue != null) // ������Ʈ ���� �ִٸ�.
                {
                    _tbUnit = element.objectReferenceValue as SheepTableUnit;

                    //EditorWindow win = GetWindow<SheepLayoutUnitEditorWindow>(true, "SheepLayout Unit Editor");
                    //win.SendEvent(EditorGUIUtility.CommandEvent("Paste"));
                }
            };
        _reorderable.onChangedCallback =
            (ReorderableList list) =>
            {
                Debug.Log($"{GetType()}::{nameof(SetList)} - onChangedCallback idx={list.index}, size={_reorderable.serializedProperty.arraySize}");
                // ����Ʈ �׸��� �������� �� ����Ʈ�� �ƹ��͵� ���ٸ� �������Ƿ� ������ üũ.
                if (_reorderable.serializedProperty.arraySize > 0)
                {
                    var element = _reorderable.serializedProperty.GetArrayElementAtIndex(list.index);
                    if (element.objectReferenceValue != null) // ������Ʈ ���� �ִٸ�.
                    {
                        //// refresh
                        //ResetSheepLayoutSerializedObject();
                    }
                }
            };
    }

    private void UpdateSheepTable()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            _editorData.show.table = GUILayout.Toggle(_editorData.show.table, "[Table]");

            if (check.changed)
                SaveEditorData();
        }
        if (_editorData.show.table == false)
            return;


        GUILayout.Label("<Create>");
        _currType = (Sheep.Type)EditorGUILayout.EnumPopup("Sheep Type", _currType);
        using (new EditorGUILayout.HorizontalScope())
        {
            _newUnitName = EditorGUILayout.TextField("Asset Name", _newUnitName);
            using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(_newUnitName)))
            {
                if (GUILayout.Button("Create"))
                {
                    if (_table.AddNewUnit(_currType, _newUnitName) != null)
                        EditorUtility.SetDirty(_table);
                    else
                        EditorGUILayout.HelpBox("������ �����Ͽ����ϴ�.", MessageType.Error);

                    _newUnitName = string.Empty;
                }
            }
        }
        if (!string.IsNullOrWhiteSpace(_newUnitName))
        {
            EditorGUILayout.HelpBox("����: �̹� �����ϰ� �ִ� �̸��� ���ٸ� ���ο� ���Ϸ� ��ü�˴ϴ�."
            , MessageType.Warning);
        }

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            //_soTable.Update();
            _reorderable.DoLayoutList();
            //_soTable.ApplyModifiedProperties(); // Remember to apply modified properties

            if (check.changed)
            {
                EditorUtility.SetDirty(_table);
                //Debug.Log("changed");
            }
        }

        //CommonEditorUI.DrawSeparator(Color.gray);
        GUILayout.Label("<Change ID>");
        if (_tbUnit)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label($"ID: {_tbUnit.id}    Name: {_tbUnit.name}");
                if (GUILayout.Button("ID ����"))
                {
                    _tbUnit.id = _table.GenerateNewID();
                    EditorUtility.SetDirty(_tbUnit);
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("���õǾ� �ִ� �������� �����ϴ�.", MessageType.Info);
        }
    }

    void CreateChildWindow(SheepTableUnit tbUnit)
    {
        // �������� â�� �� �� �ֵ��� CreateWindow�� ����Ѵ�.
        // Unit â���� Ÿ��Ʋ string���� �Ľ��� �� �ֵ��� �Ѵ�.
        ChildAssetEditorWindow window = null;
        switch (tbUnit.Type)
        {
            case Sheep.Type.None:
                break;          
            case Sheep.Type.Standard:
                window = CreateWindow<StandardSheepUnitEditor>($"{tbUnit.name}&{tbUnit.id}");
                break;
            case Sheep.Type.Buff:
                window = CreateWindow<StandardSheepUnitEditor>($"{tbUnit.name}&{tbUnit.id}");
                break;
            default:
                break;
        }

        if (window != null)
            window.Show();
        else
            Debug.LogError($"��ȿ���� �ʴ� �湮������(Sheep Type) �Դϴ�. tbUnit.type={tbUnit.Type}");
    }
    #endregion
}
