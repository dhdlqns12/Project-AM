public class PlayerUnit : UnitBase
{
    private UnitMovement movement;
    private UnitCombat combat;

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
        combat = GetComponent<UnitCombat>();
    }

    protected override void OnInitialized()
    {
        movement?.Init(this);
        combat?.Init(this,movement);
    }

    protected override void OnDeath()
    {

        movement?.Stop();
    }
}