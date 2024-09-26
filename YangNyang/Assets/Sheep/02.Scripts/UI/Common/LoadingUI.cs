using DG.Tweening;
using MoreMountains.Feedbacks;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField]
    private Slider _gauge;

    [SerializeField]
    private TextMeshProUGUI _versionText;


    [SerializeField]
    private MMF_Player _feedback_Meow;

    private void OnEnable()
    {
        _feedback_Meow.PlayFeedbacks();
        AddressableManager.Instance.OnProgressUpdate += UpdateProgressBar;
        _versionText.text = $"V{Application.version}  ";

    }

    private void OnDisable()
    {
        AddressableManager.Instance.OnProgressUpdate -= UpdateProgressBar;
    }

    private void UpdateProgressBar(float progress)
    {
        _gauge.DOValue(progress, 1f);
    }

    public void Close()
    {
        StartCoroutine(FakeLoading());
    }

    IEnumerator FakeLoading()
    {
        _gauge.DOValue(_gauge.maxValue, 1f);
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }
}
