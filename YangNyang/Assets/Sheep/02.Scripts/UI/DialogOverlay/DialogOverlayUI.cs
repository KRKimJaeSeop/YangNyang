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
    private Animator _anim;

    [SerializeField]
    private TextMeshProUGUI _nextBtnText;
    private string _nextBtnTextOrigin;

    [SerializeField]
    private MMF_Player _feedback_PressBtn;

    private void Awake()
    {
        _nextBtn.onClick.AddListener(OnClickNextBtn);
        _skipBtn.onClick.AddListener(OnClickSkipBtn);
        _nextBtnTextOrigin = _nextBtnText.text;
    }


    public void Open()
    {
        gameObject.SetActive(true);
        _anim.Play("Enter");
    }

    public void Close()
    {
        _anim.Play("Exit");
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
        DialogManager.Instance.OnClickNext();
        _feedback_PressBtn.PlayFeedbacks();
        SetActiveNextBtn(false);
    }
    private void OnClickSkipBtn()
    {
        DialogManager.Instance.OnClickSkip();
        _feedback_PressBtn.PlayFeedbacks();
    }
}
