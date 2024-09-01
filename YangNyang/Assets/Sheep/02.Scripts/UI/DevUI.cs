using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class DevUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField textField1;
    [SerializeField]
    private TMP_InputField textField2;
    [SerializeField]
    private Image img;
    [SerializeField]
    private Button btn;

    private void Awake()
    {
        btn.onClick.AddListener(OnClickTestBtn);
    }

    private void OnClickTestBtn()
    {
        //var value1= float.Parse(textField1.text);
        //var value2= float.Parse(textField2.text);
        //FieldObjectManager.Instance.SheepSpawnBuff(value1, value2);
        //Addressables.InstantiateAsync(textField1.text,new Vector2(-8,0), Quaternion.identity);
        GameDataManager.Instance.Storages.User.IncreaseLevel(int.Parse(textField1.text));
    }

}
