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

    private void Awake()
    {
        _nextBtn.onClick.AddListener(OnClickNextBtn);
        _skipBtn.onClick.AddListener(OnClickSkipBtn);
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
    public void CloseGameObject()
    {
        gameObject.SetActive(false);
    }
    private void OnClickNextBtn()
    {
        DialogManager.Instance.OnClickNext();
    }
    private void OnClickSkipBtn()
    {
        DialogManager.Instance.OnClickSkip();
    }
}
