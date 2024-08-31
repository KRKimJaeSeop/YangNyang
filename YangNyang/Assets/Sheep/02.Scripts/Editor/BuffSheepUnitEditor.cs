using System;
using UnityEngine;
using UnityEditor;

public class BuffSheepUnitEditor : ChildAssetEditorWindow
{
    [Serializable]
    public class ShowProperty
    {
        public bool file = true;
        public bool properties = true;
    }

    private ShowProperty _show = new ShowProperty();

    private BuffSheepTableUnit _tbUnit;



    void OnGUI()
    {
        base.CheckAndLoadAssetWithID(BuffSheepTableUnit.ASSET_PATH);


        if (_tbUnit == null)
        {
            EditorGUILayout.HelpBox("�����Ͱ� �����ϴ�.", MessageType.Error);
            return;
        }

        // window ��ũ�� ����
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);

        UpdateFileMenu();
        CommonEditorUI.DrawSeparator(Color.black);
        if (_soUnit != null)
        {
            _soUnit.Update();

            UpdatePropertiesr();
            CommonEditorUI.DrawSeparator(Color.black);
            //UpdateEnemies();
            //CommonEditorUI.DrawSeparator(Color.black);

            _soUnit.ApplyModifiedProperties(); // Remember to apply modified properties
        }


        // window ��ũ�� ����
        EditorGUILayout.EndScrollView();
    }


    protected override bool LoadAsset(string path)
    {
        _tbUnit = AssetDatabase.LoadAssetAtPath(path, typeof(BuffSheepTableUnit)) as BuffSheepTableUnit;
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
    private void UpdatePropertiesr()
    {
        _show.properties = GUILayout.Toggle(_show.properties, "[Properties]");
        if (_show.properties == false)
            return;

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            // ---- _soStage�� ��� �Ӽ��� �����ش�.
            SerializedProperty iterator = _soUnit.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                // ���� ������Ƽ ����.
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
