using CollectionPanel;
using UnityEngine;
using UnityEngine.Events;


public class UICollectionPanel : UIPanel
{
    [SerializeField, Header("[UICollectionPanel]")]
    private ScrollerView view;


    protected override void Awake()
    {
        base.Awake();
        view.Preload();

    }
    public override void Open(Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        base.Open(canvas,cbClose);

        view.Show();
    }


}