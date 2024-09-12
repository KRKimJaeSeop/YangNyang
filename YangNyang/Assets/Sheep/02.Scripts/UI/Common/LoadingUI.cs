using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField]
    private Slider _gauge;




    private void OnEnable()
    {
        AddressableManager.Instance.OnProgressUpdate += UpdateProgressBar;
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
