
public class BuffFastPlayer : Buff
{
    public override void Init(BuffIcon newIcon)
    {
        globalState.playerSpeedMultiple = 2f;
        
        base.Init(newIcon);
    }

    public override void Finish()
    {
        globalState.playerSpeedMultiple = 1f;
        
        base.Finish();
    }
}
