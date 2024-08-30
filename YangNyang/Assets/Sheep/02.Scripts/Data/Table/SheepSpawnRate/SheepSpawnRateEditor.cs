using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class SheepSpawnRateEditor : EditorWindow
{
    private const string EDITORPREFS_SHEEP_SPAWN_RATE_EDITOR = "Sheep_SheepSpawnRateEditor";

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

    private SheepSpawnRateTable _table;
    private SerializedObject _soTable;
    private SheepSpawnRateTableUnit _tbUnit;
    private ReorderableList _reorderable;

    private List<string> _categoryNames = new List<string>();
    private int _categoryIndex;

    private string _createUnitName = string.Empty;



    void OnEnable()
    {
        if (EditorPrefs.HasKey(EDITORPREFS_SHEEP_SPAWN_RATE_EDITOR))
        {
            string strData = EditorPrefs.GetString(EDITORPREFS_SHEEP_SPAWN_RATE_EDITOR);
            _editorData = JsonUtility.FromJson<EditorDataProperty>(strData);

            LoadAsset(_editorData.tablePath);
        }
    }

    void OnGUI()
    {
        // window 스크롤 시작
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);

        UpdateFileMenu();
        CommonEditorUI.DrawSeparator(Color.black);
        if (_table != null)
        {
            _soTable.Update();

            UpdateTable();
            CommonEditorUI.DrawSeparator(Color.black);

            _soTable.ApplyModifiedProperties();
        }

        // window 스크롤 종료
        EditorGUILayout.EndScrollView();
    }

    private void SaveEditorData()
    {
        string strData = JsonUtility.ToJson(_editorData);
        EditorPrefs.SetString(EDITORPREFS_SHEEP_SPAWN_RATE_EDITOR, strData);
    }


    #region File Menu
    bool LoadAsset(string path)
    {
        _table = AssetDatabase.LoadAssetAtPath(path, typeof(SheepSpawnRateTable)) as SheepSpawnRateTable;
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
    void OpenTable()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Sheep Spawn Rate Table", "", "asset");
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
            if (GUILayout.Button("Open Table Asset"))
            {
                OpenTable();
            }
            if (_table != null)
            {
                if (GUILayout.Button("Select Table Asset"))
                {
                    EditorGUIUtility.PingObject(_table);
                }
            }
        }
    }
    #endregion


    #region Table
    void SetList(SheepSpawnRateTable table, SerializedObject so)
    {
        _tbUnit = null;

        SerializedProperty listProperty = so.FindProperty("list");
        /// <summary>
        /// ReorderableList 
        /// - https://unityindepth.tistory.com/56
        /// - https://m.blog.naver.com/PostView.nhn?blogId=hammerimpact&logNo=220775710045&proxyReferer=https%3A%2F%2Fwww.google.com%2F
        /// </summary>
        // set ReorderableList
        string listName = "Sheep Spawn Rates List";
        _reorderable = new ReorderableList(so, listProperty, true, true, true, true);
        _reorderable.drawHeaderCallback =
            (Rect rect) =>
            {
                EditorGUI.LabelField(rect, $"<{listName}>");
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
                EditorGUI.PropertyField(rect, element, GUIContent.none); // GUIContent.none : 앞의 라벨을 붙이지 않는다.
                if (element.objectReferenceValue != null) // 오브젝트 값이 있다면.
                {
                    var tbUnit = element.objectReferenceValue as SheepSpawnRateTableUnit;

                    rect.x += (rect.width + 20);
                    rect.width = 80;
                    if (GUI.Button(rect, "Edit"))
                    {
                         var unitWindow = CreateWindow<SheepSpawnRateUnitEditor>($"{tbUnit.name}");
                        unitWindow.Show();
                    }
                }
            };
        _reorderable.onSelectCallback =
            (ReorderableList list) =>
            {
                Debug.Log($"{GetType()}::{nameof(SetList)} - onSelectCallback idx={list.index}");
                var element = _reorderable.serializedProperty.GetArrayElementAtIndex(list.index);
                if (element.objectReferenceValue != null) // 오브젝트 값이 있다면.
                {
                    var tbUnit = element.objectReferenceValue as SheepSpawnRateTableUnit;
                }
            };
        _reorderable.onChangedCallback =
            (ReorderableList list) =>
            {
                Debug.Log($"{GetType()}::{nameof(SetList)} - onChangedCallback idx={list.index}, size={_reorderable.serializedProperty.arraySize}");
                // 리스트 항목을 삭제했을 때 리스트에 아무것도 없다면 에러나므로 사이즈 체크.
                if (_reorderable.serializedProperty.arraySize > 0)
                {
                    var element = _reorderable.serializedProperty.GetArrayElementAtIndex(list.index);
                    if (element.objectReferenceValue != null) // 오브젝트 값이 있다면.
                    {
                        //// refresh
                    }
                }
            };
    }

    private void UpdateTable()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            _editorData.show.table = GUILayout.Toggle(_editorData.show.table, "[List]");
            if (check.changed)
                SaveEditorData();
        }
        if (_editorData.show.table == false)
            return;

        GUILayout.Label("<Create>");
        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                _createUnitName = EditorGUILayout.TextField("Asset Name", _createUnitName);
                using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(_createUnitName)))
                {
                    if (GUILayout.Button("Create"))
                    {

                        if (_table.AddNewUnit(_createUnitName) != null)
                            EditorUtility.SetDirty(_table);
                        else
                            EditorGUILayout.HelpBox("생성에 실패하였습니다.", MessageType.Error);

                        _createUnitName = string.Empty;
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(_createUnitName))
            {
                EditorGUILayout.HelpBox("주의: 이미 존재하고 있는 이름과 같다면 새로운 파일로 대체됩니다."
                , MessageType.Warning);
            }
        }


        using (var check = new EditorGUI.ChangeCheckScope())
        {
            _reorderable.DoLayoutList();

            if (check.changed)
            {
                EditorUtility.SetDirty(_table);
            }
        }


    }
    #endregion
}
