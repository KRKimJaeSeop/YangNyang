using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogOverlayUI : MonoBehaviour
{
    [SerializeField]
    private Button _nextBtn;
    [SerializeField]
    private Button _skipBtn;

    [SerializeField]
    private TextMeshProUGUI _nextBtnText;
    [SerializeField]
    private string _nextBtnTextOrigin;

    [SerializeField]
    private MMF_Player _feedback_PressBtn;

    private void Awake()
    {
        _nextBtn.onClick.AddListener(OnClickNextBtn);
        _skipBtn.onClick.AddListener(OnClickSkipBtn);
    }
    private void OnEnable()
    {
        SetActiveNextBtn(true);
    }

    public void SetActiveNextBtn(bool isActive)
    {
        _nextBtn.interactable = isActive;
        if (isActive)
        {
            _nextBtnText.text = _nextBtnTextOrigin;

        }
        else
        {
            _nextBtnText.text = "";
        }
    }

    public void CloseGameObject()
    {
        gameObject.SetActive(false);
    }
    private void OnClickNextBtn()
    {
        SetActiveNextBtn(false);
        DialogManager.Instance.OnClickNext();
        _feedback_PressBtn.PlayFeedbacks();
    }
    private void OnClickSkipBtn()
    {
        DialogManager.Instance.OnClickSkip();
        _feedback_PressBtn.PlayFeedbacks();
    }
}
