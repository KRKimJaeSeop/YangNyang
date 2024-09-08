using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class DialogEditor : EditorWindow
{
    private const string EDITORPREFS_CURRENCY_EDITOR = "Sheep_DialogEditor";

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

    private DialogTable _table;
    private SerializedObject _soTable;
    private DialogTableUnit _tbUnit;
    private ReorderableList _reorderable;

    private Dialog.Type _newDialogType = Dialog.Type.None;


    void OnEnable()
    {
        if (EditorPrefs.HasKey(EDITORPREFS_CURRENCY_EDITOR))
        {
            string strData = EditorPrefs.GetString(EDITORPREFS_CURRENCY_EDITOR);
            _editorData = JsonUtility.FromJson<EditorDataProperty>(strData);

            LoadAsset(_editorData.tablePath);
        }
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

            UpdateTable();
            CommonEditorUI.DrawSeparator(Color.black);

            _soTable.ApplyModifiedProperties(); // Remember to apply modified properties
        }

        // window ��ũ�� ����
        EditorGUILayout.EndScrollView();
    }

    private void SaveEditorData()
    {
        string strData = JsonUtility.ToJson(_editorData);
        EditorPrefs.SetString(EDITORPREFS_CURRENCY_EDITOR, strData);
    }


    #region File Menu
    bool LoadAsset(string path)
    {
        _table = AssetDatabase.LoadAssetAtPath(path, typeof(DialogTable)) as DialogTable;
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
        string absPath = EditorUtility.OpenFilePanel("Select Dialog Table", "", "asset");
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
            if (GUILayout.Button("Open Dialog Table Asset"))
            {
                OpenTable();
            }
            if (_table != null)
            {
                if (GUILayout.Button("Select Dialog Table Asset"))
                {
                    EditorGUIUtility.PingObject(_table);
                }
            }
        }
    }
    #endregion


    #region Table
    void SetList(DialogTable table, SerializedObject so)
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
                EditorGUI.LabelField(rect, "<Dialogs>");
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
                    var tbUnit = element.objectReferenceValue as DialogTableUnit;

                    rect.x += (rect.width + 20);
                    rect.width = 80;
                    if (GUI.Button(rect, "Edit"))
                    {
                        DialogUnitEditor unitWindow = CreateWindow<DialogUnitEditor>($"{tbUnit.name}");
                        unitWindow.Show();
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
                    _tbUnit = element.objectReferenceValue as DialogTableUnit;

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
                    }
                }
            };
    }

    private void UpdateTable()
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
        using (new EditorGUILayout.HorizontalScope())
        {
            _newDialogType = (Dialog.Type)EditorGUILayout.EnumPopup("Dialog Type", _newDialogType);
            using (new EditorGUI.DisabledScope(_newDialogType == Dialog.Type.None))
            {
                if (GUILayout.Button("Create"))
                {
                    if (_table.AddNewUnit(_newDialogType.ToString(), _newDialogType) != null)
                        EditorUtility.SetDirty(_table);
                    else
                        EditorGUILayout.HelpBox("������ �����Ͽ����ϴ�.", MessageType.Error);
                }
            }
        }
        if (_newDialogType != Dialog.Type.None)
        {
            EditorGUILayout.HelpBox("����: �̹� �����ϰ� �ִ� Ÿ���� �ִٸ� ���ο� ���Ϸ� ��ü�˴ϴ�."
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
    #endregion
}
