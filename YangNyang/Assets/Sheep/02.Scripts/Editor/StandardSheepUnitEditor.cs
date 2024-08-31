using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StandardSheepUnitEditor : ChildAssetEditorWindow
{
    [Serializable]
    public class ShowProperty
    {
        public bool file = true;
        public bool properties = true;
    }

    private ShowProperty _show = new ShowProperty();
    private SheepTableUnit _tbUnit;
    //private ReorderableList _reorderable;


    void OnGUI()
    {
        base.CheckAndLoadAsset(SheepTableUnit.ASSET_PATH);

        if (_tbUnit == null)
        {
            EditorGUILayout.HelpBox("데이터가 없습니다.", MessageType.Error);
            return;
        }

        // window 스크롤 시작
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);

        UpdateFileMenu();
        CommonEditorUI.DrawSeparator(Color.black);
        if (_soUnit != null)
        {
            _soUnit.Update();

            UpdateProperties();
            CommonEditorUI.DrawSeparator(Color.black);

            _soUnit.ApplyModifiedProperties(); // Remember to apply modified properties
        }


        // window 스크롤 종료
        EditorGUILayout.EndScrollView();
    }


    protected override bool LoadAsset(string path)
    {
        _tbUnit = AssetDatabase.LoadAssetAtPath(path, typeof(SheepTableUnit)) as SheepTableUnit;
        if (_tbUnit != null)
        {
            Debug.Log($"{GetType()}::{nameof(LoadAsset)} - loaded table");

            // set serialized object
            _soUnit = new SerializedObject(_tbUnit);

            //// set reorderable list
            //SetEnemyList(_tbUnit, _soUnit);
        }
        else
        {
            Debug.Log($"{GetType()}::{nameof(LoadAsset)} - Failed load table. path={path}");
            return false;
        }
        return true;
    }

    #region File Menu
    private void UpdateFileMenu()
    {
        _show.file = GUILayout.Toggle(_show.file, "[File]");
        if (_show.file == false)
            return;

        using (new EditorGUILayout.HorizontalScope())
        {
            if (_tbUnit != null)
            {
                if (GUILayout.Button($"Select {_tbUnit.name}.asset"))
                {
                    EditorGUIUtility.PingObject(_tbUnit);
                }
            }
        }
    }
    #endregion

    #region Properties
    private void UpdateProperties()
    {
        _show.properties = GUILayout.Toggle(_show.properties, "[Properties]");
        if (_show.properties == false)
            return;

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            // ---- _soStage내 모든 속성을 보여준다.
            SerializedProperty iterator = _soUnit.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                // 예외 프로퍼티 무시.
                if (BaseTable.IsTableObjectProperty(iterator.name))
                    continue;

                EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
            }

            if (check.changed)
                EditorUtility.SetDirty(_tbUnit);
        }
    }
    #endregion
}
