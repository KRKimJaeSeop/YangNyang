using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    public enum Code
    {
        None = 0,

        // ---- Common
        Confirm = 1,
        Notification = 2,
        Result = 3,

        // ---- PlayScene
        Main = 101,
        Sell = 201,
        Collection = 301,
        CollectionDetail = 302,
        Option = 401,
        Language = 402,
        Credit = 403,
        WatchDialog = 404,
        Research = 501,
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
    [SerializeField, ReadOnly, Tooltip("�����ִ� �г� ����Ʈ")]
    private List<OpenedPanel> _openedPanelInfos = new List<OpenedPanel>();

    [Serializable]
    class PrefabInformation
    {
        public Code code;
        public GameObject prefab;
        [SerializeField, Tooltip("�������� ���� �Ǵ� �г� ����")]
        public bool isMultipleOpen;
    }
    [Serializable]
    class PanelContainer
    {
        public Canvas canvas;
        public List<PrefabInformation> list;
    }


    [Header("[Settings]")]
    [SerializeField, Tooltip("��� ȭ��")]
    private GameObject goWaiting;
    [SerializeField, Tooltip("�ε� ȭ��")]
    private LoadingUI goLoading;
    [SerializeField, Tooltip("���̵� �� �ƿ� �ִϸ��̼�")]
    private DialogOverlayUI DialogOverlayUI;
    [SerializeField, Tooltip("���̵� �� �ƿ� �ִϸ��̼�")]
    private UIFadeInOut FadeInOut;
    [SerializeField]
    private List<PanelContainer> panelContainers;

    #region Unity/Event 
    private void Awake()
    {
        if (_instance != null)
        {
            // instance �� �ִµ� �̹� �ٸ� ������Ʈ���� �ε� �Ǿ� �ִٸ� destroy.
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
        if (!DialogManager.Instance.IsPlaying && NumberOpenPanels > 0)
        {
            // �˾��� ���� �ݾ� �� �� ���� �˾��� �ִٸ� �ߴ�.
            if (CloseLastOpenedPanel() == true)
                return;


            OpenConfirmPanel("�� ����", "QuitText", null,
                (results) =>
                {
                    var confirmResult = results as UIConfirmPanel.Results;
                    if (confirmResult != null && confirmResult.isConfirm)
                    {

#if UNITY_EDITOR
                        // Debug.Log("�� ���� Ȯ�ε�.");
                        UnityEditor.EditorApplication.isPlaying = false;
#else

        Application.Quit();

#endif
                    }
                });

        }


    }

    #endregion

    public int NumberOpenPanels { get { return _openedPanelInfos.Count; } }


    /// <summary>
    /// �����ִ� ��� �г��� �ݴ´�.
    /// </summary>
    public void CloseAll()
    {
        for (int i = _openedPanelInfos.Count - 1; i >= 0; i--)
        {
            var panelInfo = _openedPanelInfos[i];
            if (panelInfo.code != (int)Code.Main)
            {
                panelInfo.panel.Close();
            }
        }
    }

    /// <summary>
    /// �����ִ� ������ �г��� �ݴ´�.
    /// </summary>
    /// <returns></returns>
    public bool CloseLastOpenedPanel()
    {
        if (_openedPanelInfos.Count > 0)
        {
            var panelInfo = _openedPanelInfos[_openedPanelInfos.Count - 1];
            if (panelInfo.code != (int)Code.Main)
            {
                panelInfo.panel.Close();
                return true;
            }
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
    /// ���� �ڵ��� �˾� ���� �� ó�� ã�� �г��� �����Ѵ�. 
    /// ���� : ���� �˾��� ������ ���� ������ ��� ������� �ʵ��� �Ѵ�. 
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
        Debug.LogError($"{GetType()}::{nameof(GetPanelInfo)}: �ش� �г��� ����. Code({code})");
        return (null, null);
    }

    #region Common
    public void OpenWaiting()
    {
        if (goLoading != null)
            goWaiting.SetActive(true);
    }
    public void CloseWaiting()
    {
        if (goWaiting != null)
            goWaiting.SetActive(false);
    }
    public void OpenLoading()
    {
        if (goLoading != null)
            goLoading.gameObject.SetActive(true);
    }
    public void CloseLoading()
    {
        if (goLoading != null)
            goLoading.Close();
    }

    /// <summary>
    /// ����->����.
    /// </summary>
    public void FadeIn(Action callback = null)
    {
        if (FadeInOut != null)
            FadeInOut.FadeIn(callback);
    }

    /// <summary>
    /// ����->����.
    /// </summary>
    public void FadeOut(Action callback = null)
    {
        if (FadeInOut != null)
            FadeInOut.FadeOut(callback);
    }



    public UIConfirmPanel OpenConfirmPanel(string title,
        string content,
        Canvas canvas = null,
        UnityAction<object> cbClose = null)
    {
        // �巡�� �����ε�. ���� ������ ���� �ϴ� ������
        //InputController.Instance.ReleaseInputStates();

        var panelCode = Code.Confirm;
        var panelInfo = GetPanelInfo(panelCode);
        var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
        var component = go.GetComponent<UIConfirmPanel>();
        var openInfo = AddPanel((int)panelCode, component);
        component.Open(title, content, panelInfo.canvas,
             (results) =>
             {
                 RemovePanel(openInfo);
                 ObjectPool.Instance.Push(panelInfo.prefabInfo.prefab.name, go, true);
                 cbClose?.Invoke(results);
                 Debug.Log($"{GetType()}::{nameof(OpenConfirmPanel)}: Closed. _openPanels.Count={_openedPanelInfos.Count}");
             });
        Debug.Log($"{GetType()}::{nameof(OpenConfirmPanel)}: _openPanels.Count={_openedPanelInfos.Count}");
        return component;
    }

    public UINotificationPanel OpenNotificationPanel(string content, Action<object> cbClose = null)
    {
        // �巡�� �����ε�. ���� ������ ���� �ϴ� ������
        //InputController.Instance.ReleaseInputStates();

        var panelCode = Code.Notification;
        var panelInfo = GetPanelInfo(panelCode);
        var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
        var component = go.GetComponent<UINotificationPanel>();
        var openInfo = AddPanel((int)panelCode, component);
        component.Open(content, panelInfo.canvas,
             (results) =>
             {
                 RemovePanel(openInfo);
                 ObjectPool.Instance.Push(panelInfo.prefabInfo.prefab.name, go, true);
                 cbClose?.Invoke(results);
                 Debug.Log($"{GetType()}::{nameof(OpenNotificationPanel)}: Closed. _openPanels.Count={_openedPanelInfos.Count}");
             });
        Debug.Log($"{GetType()}::{nameof(OpenNotificationPanel)}: _openPanels.Count={_openedPanelInfos.Count}");
        return component;
    }

    public UIResultPanel OpenResultPanel(string title,
        string content, Action<object> cbClose = null)
    {
        // �巡�� �����ε�. ���� ������ ���� �ϴ� ������
        //InputController.Instance.ReleaseInputStates();

        var panelCode = Code.Result;
        var panelInfo = GetPanelInfo(panelCode);
        var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
        var component = go.GetComponent<UIResultPanel>();
        var openInfo = AddPanel((int)panelCode, component);
        component.Open(title, content, panelInfo.canvas,
             (results) =>
             {
                 RemovePanel(openInfo);
                 ObjectPool.Instance.Push(panelInfo.prefabInfo.prefab.name, go, true);
                 cbClose?.Invoke(results);
                 Debug.Log($"{GetType()}::{nameof(OpenResultPanel)}: Closed. _openPanels.Count={_openedPanelInfos.Count}");
             });
        Debug.Log($"{GetType()}::{nameof(OpenResultPanel)}: _openPanels.Count={_openedPanelInfos.Count}");
        return component;
    }
    #endregion

    #region Main
    /// <summary>
    /// ���� �г� ����.
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

    public void StartBuffCountdown()
    {
        var panel = GetOpenedPanelInfo((int)Code.Main);
        (panel.panel as UIMainPanel).StartBuffCountdown();
    }

    #endregion

    #region Collection
    public UICollectionPanel OpenCollectionPanel(Action<object> cbClose = null)
    {
        // �巡�� �����ε�. ���� ������ ���� �ϴ� ������
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

    public UICollectionDetailPanel OpenCollectionDetailPanel(int id, bool isUnolck, Action<object> cbClose = null)
    {
        // �巡�� �����ε�. ���� ������ ���� �ϴ� ������
        //InputController.Instance.ReleaseInputStates();

        var panelCode = Code.CollectionDetail;
        var panelInfo = GetPanelInfo(panelCode);
        var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
        var component = go.GetComponent<UICollectionDetailPanel>();
        var openInfo = AddPanel((int)panelCode, component);
        component.Open(id, isUnolck, panelInfo.canvas,
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
    /// �ɼ� �г� ����.
    /// </summary>
    public UIOptionPanel OpenOptionPanel(Action<object> cbClose = null)
    {
        // �巡�� �����ε�. ���� ������ ���� �ϴ� ������
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

    public UILanguageSelectPanel OpenLanguageSelectPanel(Action<object> cbClose = null)
    {
        // �巡�� �����ε�. ���� ������ ���� �ϴ� ������
        //InputController.Instance.ReleaseInputStates();

        var panelCode = Code.Language;
        var panelInfo = GetPanelInfo(panelCode);
        var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
        var component = go.GetComponent<UILanguageSelectPanel>();
        var openInfo = AddPanel((int)panelCode, component);
        component.Open(panelInfo.canvas,
             (results) =>
             {
                 RemovePanel(openInfo);
                 ObjectPool.Instance.Push(panelInfo.prefabInfo.prefab.name, go, true);
                 cbClose?.Invoke(results);
                 Debug.Log($"{GetType()}::{nameof(OpenLanguageSelectPanel)}: Closed. _openPanels.Count={_openedPanelInfos.Count}");
             });
        Debug.Log($"{GetType()}::{nameof(OpenLanguageSelectPanel)}: _openPanels.Count={_openedPanelInfos.Count}");
        return component;
    }

    public UICreditPanel OpenCreditPanel(Action<object> cbClose = null)
    {
        // �巡�� �����ε�. ���� ������ ���� �ϴ� ������
        //InputController.Instance.ReleaseInputStates();

        var panelCode = Code.Credit;
        var panelInfo = GetPanelInfo(panelCode);
        var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
        var component = go.GetComponent<UICreditPanel>();
        var openInfo = AddPanel((int)panelCode, component);
        component.Open(panelInfo.canvas,
             (results) =>
             {
                 RemovePanel(openInfo);
                 ObjectPool.Instance.Push(panelInfo.prefabInfo.prefab.name, go, true);
                 cbClose?.Invoke(results);
                 Debug.Log($"{GetType()}::{nameof(OpenCreditPanel)}: Closed. _openPanels.Count={_openedPanelInfos.Count}");
             });
        Debug.Log($"{GetType()}::{nameof(OpenCreditPanel)}: _openPanels.Count={_openedPanelInfos.Count}");
        return component;
    }
    public UIWatchDialogPanel OpenWatchDialogPanel(Action<object> cbClose = null)
    {
        // �巡�� �����ε�. ���� ������ ���� �ϴ� ������
        //InputController.Instance.ReleaseInputStates();

        var panelCode = Code.WatchDialog;
        var panelInfo = GetPanelInfo(panelCode);
        var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
        var component = go.GetComponent<UIWatchDialogPanel>();
        var openInfo = AddPanel((int)panelCode, component);
        component.Open(panelInfo.canvas,
             (results) =>
             {
                 RemovePanel(openInfo);
                 ObjectPool.Instance.Push(panelInfo.prefabInfo.prefab.name, go, true);
                 cbClose?.Invoke(results);
                 Debug.Log($"{GetType()}::{nameof(OpenWatchDialogPanel)}: Closed. _openPanels.Count={_openedPanelInfos.Count}");
             });
        Debug.Log($"{GetType()}::{nameof(OpenWatchDialogPanel)}: _openPanels.Count={_openedPanelInfos.Count}");
        return component;
    }
    #endregion

    #region Sell
    /// <summary>
    /// ���� �г� ����.
    /// </summary>
    public UISellPanel OpenSellPanel(Action<object> cbClose = null)
    {
        // �巡�� �����ε�. ���� ������ ���� �ϴ� ������
        //InputController.Instance.ReleaseInputStates();

        var panelCode = Code.Sell;
        var panelInfo = GetPanelInfo(panelCode);
        var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
        var component = go.GetComponent<UISellPanel>();
        var openInfo = AddPanel((int)panelCode, component);
        component.Open(panelInfo.canvas,
             (results) =>
             {
                 RemovePanel(openInfo);
                 ObjectPool.Instance.Push(panelInfo.prefabInfo.prefab.name, go, true);
                 cbClose?.Invoke(results);
                 Debug.Log($"{GetType()}::{nameof(OpenSellPanel)}: Closed. _openPanels.Count={_openedPanelInfos.Count}");
             });
        Debug.Log($"{GetType()}::{nameof(OpenSellPanel)}: _openPanels.Count={_openedPanelInfos.Count}");
        return component;
    }
    #endregion

    #region Research
    /// <summary>
    /// ���� �г� ����.
    /// </summary>
    public UIResearchPanel OpenResearchPanel(Action<object> cbClose = null)
    {
        // �巡�� �����ε�. ���� ������ ���� �ϴ� ������
        //InputController.Instance.ReleaseInputStates();

        var panelCode = Code.Research;
        var panelInfo = GetPanelInfo(panelCode);
        var go = GetPanelObject(panelInfo.canvas, panelInfo.prefabInfo.prefab.name);
        var component = go.GetComponent<UIResearchPanel>();
        var openInfo = AddPanel((int)panelCode, component);
        component.Open(panelInfo.canvas,
             (results) =>
             {
                 RemovePanel(openInfo);
                 ObjectPool.Instance.Push(panelInfo.prefabInfo.prefab.name, go, true);
                 cbClose?.Invoke(results);
                 Debug.Log($"{GetType()}::{nameof(OpenResearchPanel)}: Closed. _openPanels.Count={_openedPanelInfos.Count}");
             });
        Debug.Log($"{GetType()}::{nameof(OpenResearchPanel)}: _openPanels.Count={_openedPanelInfos.Count}");
        return component;
    }
    #endregion


    #region Dialog
    public void OpenDialog()
    {
        if (DialogOverlayUI != null)
        {
            DialogOverlayUI.gameObject.SetActive(true);
        }

    }
    public void SetActiveDialogNextBtn()
    {
        if (DialogOverlayUI != null)
        {
            DialogOverlayUI.SetActiveNextBtn(true);
        }
    }
    public void CloseDialog()
    {
        if (DialogOverlayUI != null)
        {
            DialogOverlayUI.gameObject.SetActive(false);
        }

    }
    #endregion
}


