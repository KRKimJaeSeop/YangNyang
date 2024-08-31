using UnityEditor;
using UnityEngine;

/// <summary>
/// ���¿� ���� �ڽ� ������.
/// </summary>
public abstract class ChildAssetEditorWindow : EditorWindow
{
    protected SerializedObject _soUnit;
    protected Vector2 _scrollPos;
    protected string _assetPath;
    protected abstract bool LoadAsset(string path);

    /// <summary>
    /// �����Ͱ� ����, �÷��� �� �� SerializedObject�� �������Ƿ�
    /// �ٽ� �ε� �ϵ��� �Ѵ�.
    /// </summary>
    protected virtual void OnEnable()
    {
        if (!string.IsNullOrEmpty(_assetPath))
            LoadAsset(_assetPath);
    }

    protected virtual void CheckAndLoadAssetWithID(string path)
    {
        if (string.IsNullOrEmpty(_assetPath))
        {
            Debug.Log($"{GetType()}::{nameof(CheckAndLoadAssetWithID)}: title = {this.titleContent.text}");
            string[] values = this.titleContent.text.Split('&');
            string name = values[0];

            _assetPath = $"{path}{name}.asset";
            LoadAsset(_assetPath);
        }
    }

    protected virtual void CheckAndLoadAsset(string path)
    {
        if (string.IsNullOrEmpty(_assetPath))
        {
            Debug.Log($"{GetType()}::{nameof(CheckAndLoadAssetWithID)}: title = {this.titleContent.text}");
            string[] values = this.titleContent.text.Split('&');
            string name = values[0];

            _assetPath = $"{path}{name}.asset";
            LoadAsset(_assetPath);
        }
    }


}