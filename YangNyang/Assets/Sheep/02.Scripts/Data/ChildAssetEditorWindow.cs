using UnityEditor;
using UnityEngine;

/// <summary>
/// 에셋에 대한 자식 윈도우.
/// </summary>
public abstract class ChildAssetEditorWindow : EditorWindow
{
    protected SerializedObject _soUnit;
    protected Vector2 _scrollPos;
    protected string _assetPath;
    protected abstract bool LoadAsset(string path);

    /// <summary>
    /// 에디터가 빌드, 플레이 될 때 SerializedObject가 없어지므로
    /// 다시 로드 하도록 한다.
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