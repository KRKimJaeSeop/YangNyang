public class BuffSheep : StandardSheep
{
    protected override void WorkComplete()
    {
        base.WorkComplete();
        UIManager.Instance.OpenConfirmPanel("������", "��������?", null,
            (result) =>
            {
                var confirmResult = result as UIConfirmPanel.Results;
                if(confirmResult !=null && confirmResult.isConfirm)
                {
                    
                }
                else
                {

                }

            });
    }

}
