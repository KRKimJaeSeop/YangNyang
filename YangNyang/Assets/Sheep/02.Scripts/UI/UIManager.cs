using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public enum Code
    {
        None = 0,

        // ---- Common
        Confirm = 1,
        Notification = 2,

        // ---- PlayScene
        Main = 101,
        Shop = 201,
        Collection = 301,
        CollectionDetail = 302,
        Option = 401,
    }

    [Serializable]
    public class OpenedPanel
    {
        public int code;
        public UIPanel panel;
        public OpenedPanel(int code, UIPanel panel)
        {
            this.code = code;
            this.panel = panel;
        }
    }

    [Header("<Debug>")]
    [SerializeField, ReadOnly, Tooltip("열려있는 패널 리스트")]
    private List<OpenedPanel> _openedPanelInfos = new List<OpenedPanel>();

    [Serializable]
    class PrefabInformation
    {
        public Code code;
        public GameObject prefab;
        [SerializeField, Tooltip("여러개가 오픈 되는 패널 여부")]
        public bool isMultipleOpen;
    }
    [Serializable]
    class PanelContainer
    {
        public Canvas canvas;
        public List<PrefabInformation> list;
    }


    [Header("[Settings]")]
    [SerializeField, Tooltip("대기 화면")]
    private GameObject goWaiting;
    [SerializeField, Tooltip("로딩 화면")]
    private GameObject goLoading;
    [SerializeField]
    private List<PanelContainer> panelContainers;

    #region Unity/Event 
    private void Awake()
    {
        if (_instance != null)
        {
            // instance 가 있는데 이미 다른 오브젝트에서 로드 되어 있다면 destroy.
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

    }
    public void Preload()
    {
        foreach (var container in panelContainers)
        {
            foreach (var prefabInfo in container.list)
            {
                ObjectPool.Instance.LoadPoolItem(prefabInfo.prefab.name, prefabInfo.prefab, 1);
            }
        }
    }

    private void OnEnable()
    {
        InputController.OnAndroidEscape += this.OnAndroidEscape;
    }

    private void OnDisable()
    {
        InputController.OnAndroidEscape -= this.OnAndroidEscape;
    }
    private void OnAndroidEscape()
    {
        // 팝업을 먼저 닫아 본 후 닫힌 팝업이 있다면 중단.
        if (CloseLastOpenedPanel() == true)
            return;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        // 종료 확인 팝업
        //OpenConfirm(GameDataManager.Instance.Tables.LocalData.AppShutdown,
        //    null, null, null, null, (resultObj) =>
        //    {
        //        var result = resultObj as UIConfirmPanel.Results;
        //        if (result != null)
        //        {
        //            if (result.isConfirm)
        //            {
        //                UtilFunctions.Quit();
        //            }
        //        }
        //    });
    }

    #endregion

    public int NumberOpenPanels { get { return _openedPanelInfos.Count; } }


    /// <summary>
    /// 열려있는 모든 패널을 닫는다.
    /// </summary>
    public void CloseAll()
    {
        for (int i = _openedPanelInfos.Count - 1; i >= 0; i--)
        {
            _openedPanelInfos[i].panel.Close();
        }
    }

    /// <summary>
    /// 열려있는 마지막 패널을 닫는다.
    /// </summary>
    /// <returns></returns>
    public bool CloseLastOpenedPanel()
    {
        if (_openedPanelInfos.Count > 0)
        {
            _openedPanelInfos[_openedPanelInfos.Count - 1].panel.Close();
            return true;
        }
        return false;
    }

    public void Close(int code)
    {
        var info = GetOpenedPanelInfo(code);
        if (info == null)
        {
            Debug.LogWarning($"{GetType()}::{nameof(RemovePanel)}: not enough panel");
            return;
        }
        info.panel.Close();
    }

    private OpenedPanel GetOpenedPanelInfo(int code)
    {
        return _openedPanelInfos.Find(item => item.code == code);
    }
    private GameObject GetPanelObject(Canvas canvas, string name)
    {
        var go = ObjectPool.Instance.Pop(name);
        go.transform.SetParent(canvas.transform, false);
        go.transform.localPosition = Vector3.zero;
        return go;
    }

    private OpenedPanel AddPanel(int code, UIPanel panel)
    {
        //_openPanelInfos.Add(panel);
        var panelInfo = new OpenedPanel(code, panel);
        _openedPanelInfos.Add(panelInfo);
        return panelInfo;
    }
    private void RemovePanel(OpenedPanel info)
    {
        if (!_openedPanelInfos.Remove(info))
            Debug.LogWarning($"{GetType()}::{nameof(RemovePanel)}: not enough panel");
    }

    /// <summary>
    /// 같은 코드의 팝업 정보 중 처음 찾은 패널을 삭제한다. 
    /// 주의 : 같은 팝업이 여러개 오픈 가능한 경우 사용하지 않도록 한다. 
    /// </summary>
    /// <param name="code"></param>
    private void RemovePanel(int code)
    {
        var info = GetOpenedPanelInfo(code);
        if (info == null)
        {
            Debug.LogWarning($"{GetType()}::{nameof(RemovePanel)}: not enough panel");
            return;
        }
        RemovePanel(info);
    }

    private (Canvas canvas, PrefabInformation prefabInfo) GetPanelInfo(Code code)
    {
        foreach (var container in panelContainers)
        {
            var prefabInfo = container.list.Find(item => item.code == code);
            if (prefabInfo != null)
                return (container.canvas, prefabInfo);
        }
        Debug.LogError($"{GetType()}::{nameof(GetPanelInfo)}: 해당 패널이 없음. Code({code})");
        return (null, null);
    }

    #region Main
    /// <summary>
    /// 메인 패널 오픈.
    /// </summary>
    public UIMainPanel OpenMainPanel()
    {
        var panelCode = Code.Main;
        var panelInfo = GetPanelInfo(panelCode);
        if (panelInfo.prefabInfo.prefab != null)
        {
            var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
            var component = go.GetComponent<UIMainPanel>();
            var openInfo = AddPanel((int)panelCode, component);
            component.Open(panelInfo.canvas);
        }
        return null;
    }

    #endregion

    #region Collection
    public UICollectionPanel OpenCollectionPanel(Action<object> cbClose = null)
    {
        // 드래그 해제인데. 뭐가 문젠지 몰라서 일단 꺼보기
        //InputController.Instance.ReleaseInputStates();

        var panelCode = Code.Collection;
        var panelInfo = GetPanelInfo(panelCode);
        var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
        var component = go.GetComponent<UICollectionPanel>();
        var openInfo = AddPanel((int)panelCode, component);
        component.Open(panelInfo.canvas,
               (results) =>
               {
                   RemovePanel(openInfo);
                   ObjectPool.Instance.Push(panelInfo.prefabInfo.prefab.name, go, true);
                   cbClose?.Invoke(results);
                   Debug.Log($"{GetType()}::{nameof(OpenCollectionPanel)}: Closed. _openPanels.Count={_openedPanelInfos.Count}");
               });
        Debug.Log($"{GetType()}::{nameof(OpenCollectionPanel)}: _openPanels.Count={_openedPanelInfos.Count}");
        return component;
    }

    public UICollectionDetailPanel OpenCollectionDetailPanel(int id, Action<object> cbClose = null)
    {
        // 드래그 해제인데. 뭐가 문젠지 몰라서 일단 꺼보기
        //InputController.Instance.ReleaseInputStates();

        var panelCode = Code.CollectionDetail;
        var panelInfo = GetPanelInfo(panelCode);
        var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
        var component = go.GetComponent<UICollectionDetailPanel>();
        var openInfo = AddPanel((int)panelCode, component);
        component.Open(id,panelInfo.canvas,
               (results) =>
               {
                   RemovePanel(openInfo);
                   ObjectPool.Instance.Push(panelInfo.prefabInfo.prefab.name, go, true);
                   cbClose?.Invoke(results);
                   Debug.Log($"{GetType()}::{nameof(OpenCollectionDetailPanel)}: Closed. _openPanels.Count={_openedPanelInfos.Count}");
               });
        Debug.Log($"{GetType()}::{nameof(OpenCollectionDetailPanel)}: _openPanels.Count={_openedPanelInfos.Count}");
        return component;
    }


    #endregion

    #region Option
    /// <summary>
    /// 옵션 패널 오픈.
    /// </summary>
    public UIOptionPanel OpenOptionPanel(Action<object> cbClose = null)
    {
        // 드래그 해제인데. 뭐가 문젠지 몰라서 일단 꺼보기
        //InputController.Instance.ReleaseInputStates();

        var panelCode = Code.Option;
        var panelInfo = GetPanelInfo(panelCode);
        var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
        var component = go.GetComponent<UIOptionPanel>();
        var openInfo = AddPanel((int)panelCode, component);
        component.Open(panelInfo.canvas,
             (results) =>
             {
                 RemovePanel(openInfo);
                 ObjectPool.Instance.Push(panelInfo.prefabInfo.prefab.name, go, true);
                 cbClose?.Invoke(results);
                 Debug.Log($"{GetType()}::{nameof(OpenOptionPanel)}: Closed. _openPanels.Count={_openedPanelInfos.Count}");
             });
        Debug.Log($"{GetType()}::{nameof(OpenOptionPanel)}: _openPanels.Count={_openedPanelInfos.Count}");
        return component;
    }
    #endregion

    #region Shop
    /// <summary>
    /// 상점 패널 오픈.
    /// </summary>
    public UIShopPanel OpenShopPanel(Action<object> cbClose = null)
    {
        // 드래그 해제인데. 뭐가 문젠지 몰라서 일단 꺼보기
        //InputController.Instance.ReleaseInputStates();

        var panelCode = Code.Shop;
        var panelInfo = GetPanelInfo(panelCode);
        var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
        var component = go.GetComponent<UIShopPanel>();
        var openInfo = AddPanel((int)panelCode, component);
        component.Open(panelInfo.canvas,
             (results) =>
             {
                 RemovePanel(openInfo);
                 ObjectPool.Instance.Push(panelInfo.prefabInfo.prefab.name, go, true);
                 cbClose?.Invoke(results);
                 Debug.Log($"{GetType()}::{nameof(OpenShopPanel)}: Closed. _openPanels.Count={_openedPanelInfos.Count}");
             });
        Debug.Log($"{GetType()}::{nameof(OpenShopPanel)}: _openPanels.Count={_openedPanelInfos.Count}");
        return component;
    }
    #endregion


}


