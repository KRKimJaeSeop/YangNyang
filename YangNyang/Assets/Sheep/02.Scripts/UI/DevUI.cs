using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class DevUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField textField1;
    [SerializeField]
    private TMP_InputField textField2;

    [SerializeField]
    private Button btn;

    private void Awake()
    {
        btn.onClick.AddListener(OnClickTestBtn);
    }

    private void OnClickTestBtn()
    {
        var value1= float.Parse(textField1.text);
        var value2= float.Parse(textField2.text);
        FieldObjectManager.Instance.SheepSpawnBuff(value1, value2);
    }

}
