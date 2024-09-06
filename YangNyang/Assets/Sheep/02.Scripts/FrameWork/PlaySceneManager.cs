using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySceneManager : Singleton<PlaySceneManager>
{
    private void Awake()
    {
        InitializeScene();
    }
    private void InitializeScene()
    {
        Application.targetFrameRate = 60;
        GameDataManager.Instance.Initialize();
        FieldObjectManager.Instance.Initialize();

        UIManager.Instance.Preload();

        UIManager.Instance.OpenMainPanel();
    }
}
