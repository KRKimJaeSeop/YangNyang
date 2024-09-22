public class BuffSheep : StandardSheep
{
    protected override void WorkComplete()
    {
        base.WorkComplete();
        UIManager.Instance.OpenNotificationPanel("임시 30초동안 축복을 받습니다~");
        FieldObjectManager.Instance.SheepSpawnBuff(60, 30);
        UIManager.Instance.StartBuffCountdown();
    }

}
