using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StorageEditor : EditorWindow
{
    public const string EDITORPREFS_STORAGE_EDITOR = "Sheep_StorageEditor";
    [Serializable]
    public class ShowProperty
    {
        public bool currency = true; 
        public bool preferences = true;
        public bool user = true;
    }
    [Serializable]
    public class EditorDataProperty
    {
        public ShowProperty show = new ShowProperty();
    }

    private StorageContainer _storageContainer;

    private SerializedObject _soTarget;
    private Vector2 _scrollPos;
    private EditorDataProperty _editorData = new EditorDataProperty();


    // ---- clones
    public PreferenceStorage.StorageData clonePreference = null;
    public UserStorage.StorageData cloneUser = null;
    public CurrencyStorage.StorageData cloneCurrency = null;
    // ----

    private void OnEnable()
    {
        if (EditorPrefs.HasKey(EDITORPREFS_STORAGE_EDITOR))
        {
            string strData = EditorPrefs.GetString(EDITORPREFS_STORAGE_EDITOR);
            _editorData = JsonUtility.FromJson<EditorDataProperty>(strData);
        }
        else
        {
            SaveEditorData();
        }

        // "target" can be any class derrived from ScriptableObject 
        // (could be EditorWindow, MonoBehaviour, etc)
        ScriptableObject target = this;
        _soTarget = new SerializedObject(target);
    }

    private void OnGUI()
    {
        if (_storageContainer == null || !_storageContainer.IsLoaded)
        {
            EditorGUILayout.HelpBox("Data Hasn't been Loaded", MessageType.Error);
            if (GUILayout.Button("Load Data"))
            {
                _storageContainer = GameObject.FindObjectOfType<GameDataManager>().Storages;
                if (_storageContainer != null)
                {
                    _storageContainer.RegisterStorages();
                    _storageContainer.Load();
                    RefreshAllData();
                }
            }
            return;
        }

        // window 스크롤 시작
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);

        if (_soTarget != null)
            _soTarget.Update();

        UpdatePreferences();
        CommonEditorUI.DrawSeparator();
        UpdateUser();
        CommonEditorUI.DrawSeparator();
        UpdateCurrency();
        CommonEditorUI.DrawSeparator();

        if (_soTarget != null)
            _soTarget.ApplyModifiedProperties(); // Remember to apply modified properties

        // window 스크롤 종료
        EditorGUILayout.EndScrollView();
    }


    private void SaveEditorData()
    {
        string strData = JsonUtility.ToJson(_editorData);
        EditorPrefs.SetString(EDITORPREFS_STORAGE_EDITOR, strData);
    }

    #region EditorMenu
    private void RefreshAllData()
    {
        RefreshPreferences();
        RefreshUser();
        RefreshCurrency();
    }
    #endregion

    #region Preferences
    private void RefreshPreferences()
    {
        PreferenceStorage.StorageData data = _storageContainer.Preference.Data;
        clonePreference = data.Clone() as PreferenceStorage.StorageData;
        GUI.FocusControl(null);
    }

    private void UpdatePreferences()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            _editorData.show.preferences = GUILayout.Toggle(_editorData.show.preferences, "[Preferences]");

            if (check.changed)
                SaveEditorData();
        }
        if (_editorData.show.preferences == false)
            return;


        SerializedProperty property = _soTarget.FindProperty("clonePreference");
        if (property == null)
            return;
        EditorGUILayout.PropertyField(property, true);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Refresh"))
            {
                RefreshPreferences();
            }
            if (GUILayout.Button("Apply"))
            {
                _storageContainer.Preference.Overwrite(clonePreference);
            }
            if (GUILayout.Button("Save"))
            {
                _storageContainer.Preference.Save();
            }
        }
        if (GUILayout.Button("Clear data"))
        {
            _storageContainer.Preference.Clear();
            RefreshPreferences();
        }
    }
    #endregion


    #region User
    private void RefreshUser()
    {
        UserStorage.StorageData data = _storageContainer.User.Data;
        cloneUser = data.Clone() as UserStorage.StorageData;
        GUI.FocusControl(null);
    }

    private void UpdateUser()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            _editorData.show.user = GUILayout.Toggle(_editorData.show.user, "[User]");

            if (check.changed)
                SaveEditorData();
        }
        if (_editorData.show.user == false)
            return;

        SerializedProperty property = _soTarget.FindProperty("cloneUser");
        if (property == null)
            return;
        EditorGUILayout.PropertyField(property, true);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Refresh"))
            {
                RefreshUser();
            }
            if (GUILayout.Button("Apply"))
            {
                _storageContainer.User.Overwrite(cloneUser);
            }
            if (GUILayout.Button("Save"))
            {
                _storageContainer.User.Save();
            }
        }
        if (GUILayout.Button("Clear data"))
        {
            _storageContainer.User.Clear();
            RefreshUser();
        }
    }
    #endregion


    #region Currency
    private void RefreshCurrency()
    {
        CurrencyStorage.StorageData data = _storageContainer.Currency.Data;
        cloneCurrency = data.Clone() as CurrencyStorage.StorageData;
        GUI.FocusControl(null);
    }

    private void UpdateCurrency()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            _editorData.show.currency = GUILayout.Toggle(_editorData.show.currency, "[Currency]");

            if (check.changed)
                SaveEditorData();
        }
        if (_editorData.show.currency == false)
            return;

        SerializedProperty property = _soTarget.FindProperty("cloneCurrency");
        if (property == null)
            return;
        EditorGUILayout.PropertyField(property, true);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Refresh"))
            {
                RefreshCurrency();
            }
            if (GUILayout.Button("Apply"))
            {
                _storageContainer.Currency.Overwrite(cloneCurrency);
            }
            if (GUILayout.Button("Save"))
            {
                _storageContainer.Currency.Save();
            }
        }
        if (GUILayout.Button("Clear data"))
        {
            _storageContainer.Currency.Clear();
            RefreshCurrency();
        }
    }
    #endregion
}
