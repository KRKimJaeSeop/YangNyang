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

    [SerializeField]
    private Button btn1;
    [SerializeField]
    private Button btn2;
    [SerializeField]
    private Button btn3;
    [SerializeField]
    private Button btn4;
    [SerializeField]
    private Button btn5;


    private void Awake()
    {
        btn.onClick.AddListener(OnClickTestBtn);
        btn1.onClick.AddListener(OnClick1);
        btn2.onClick.AddListener(OnClick2);
        btn3.onClick.AddListener(OnClick3);
        btn4.onClick.AddListener(OnClick4);
        btn5.onClick.AddListener(OnClick5);

    }

    private void OnClickTestBtn()
    {
        //var value1= float.Parse(textField1.text);
        //var value2= float.Parse(textField2.text);
        //FieldObjectManager.Instance.SheepSpawnBuff(value1, value2);
        //Addressables.InstantiateAsync(textField1.text,new Vector2(-8,0), Quaternion.identity);
        //GameDataManager.Instance.Storages.User.IncreaseLevel(int.Parse(textField1.text));
        //GameManager.Instance.GameClear();
        FieldObjectManager.Instance.DespawnByInstanceID(int.Parse(textField1.text));
    }
    private void OnClick1()
    {
        //UIManager.Instance.OpenConfirmPanel(textField1.text, textField2.text);
        FieldObjectManager.Instance.StartSheepSpawn(false);
    }
    private void OnClick2()
    {
        FieldObjectManager.Instance.StartSheepSpawn(true);
        //UIManager.Instance.OpenNotificationPanel(textField1.text);
    }
    private void OnClick3()
    {
        FieldObjectManager.Instance.StopSheepSpawn();
        //UIManager.Instance.OpenResultPanel(textField1.text, textField2.text);
    }
    private void OnClick4()
    {
        //if (textField1.text == string.Empty)
        //{
        //    UIManager.Instance.OpenLoading();
        //}
        //else
        //{
        //    UIManager.Instance.CloseLoading();

        //}
        GameManager.Instance.TestEnter();
    }
    private void OnClick5()
    {
        //if (textField1.text == string.Empty)
        //{
        //    UIManager.Instance.OpenWaiting();
        //}
        //else
        //{
        //    UIManager.Instance.CloseWaiting();

        //}
        GameManager.Instance.TestExit();

    }

}
