public class BuffSheep : StandardSheep
{
    protected override void WorkComplete()
    {
        base.WorkComplete();
        UIManager.Instance.OpenNotificationPanel("�ӽ� 30�ʵ��� �ູ�� �޽��ϴ�~");
        FieldObjectManager.Instance.SheepSpawnBuff(60, 30);
        UIManager.Instance.StartBuffCountdown();
    }

}
