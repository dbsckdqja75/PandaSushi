
public class BuffFastRider : Buff
{
    public override void Init(BuffIcon newIcon)
    {
        globalState.riderSpeedMultiple = 3f;
        
        base.Init(newIcon);
    }

    public override void Finish()
    {
        globalState.riderSpeedMultiple = 1f;
        
        base.Finish();
    }
}
