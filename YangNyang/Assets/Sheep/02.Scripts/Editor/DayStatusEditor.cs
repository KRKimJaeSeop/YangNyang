using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class DayStatusEditor : EditorWindow
{
    private const string EDITORPREFS_DAY_STATUS_EDITOR = "Sheep_DayStatus";

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

    private DayStatusTable _table;
    private SerializedObject _soTable;
    private ReorderableList _reorderable;

    private List<string> _categoryNames = new List<string>();
    private int _categoryIndex;

    private string _createUnitName = string.Empty;
    void OnEnable()
    {
        if (EditorPrefs.HasKey(EDITORPREFS_DAY_STATUS_EDITOR))
        {
            string strData = EditorPrefs.GetString(EDITORPREFS_DAY_STATUS_EDITOR);
            _editorData = JsonUtility.FromJson<EditorDataProperty>(strData);

            LoadAsset(_editorData.tablePath);
        }
    }
    void OnGUI()
    {
        // window 스크롤 시작
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);

        UpdateFileMenu();
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
        EditorPrefs.SetString(EDITORPREFS_DAY_STATUS_EDITOR, strData);
    }


    #region File Menu
    bool LoadAsset(string path)
    {
        _table = AssetDatabase.LoadAssetAtPath(path, typeof(DayStatusTable)) as DayStatusTable;
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
        string absPath = EditorUtility.OpenFilePanel("Select Day Status Table", "", "asset");
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
    void SetList(DayStatusTable table, SerializedObject so)
    {
        SerializedProperty listProperty = so.FindProperty("list");

        // ReorderableList 설정
        string listName = "Day Status Table List";
        _reorderable = new ReorderableList(so, listProperty, true, true, true, true);
        _reorderable.drawHeaderCallback =
            (Rect rect) =>
            {
                EditorGUI.LabelField(rect, $"<{listName}>");
            };

        _reorderable.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = listProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    element,
                    GUIContent.none
                );

            };

        _reorderable.onSelectCallback = (ReorderableList list) =>
        {
            Debug.Log($"{GetType()}::{nameof(SetList)} - onSelectCallback idx={list.index}");
        };

        _reorderable.onChangedCallback = (ReorderableList list) =>
        {
            Debug.Log($"{GetType()}::{nameof(SetList)} - onChangedCallback idx={list.index}, size={list.count}");
            so.ApplyModifiedProperties();
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

        //GUILayout.Label("<Create>");
        using (new EditorGUILayout.HorizontalScope())
        {
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

