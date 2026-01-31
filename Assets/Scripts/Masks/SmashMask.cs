
using Unity.VisualScripting;
using UnityEngine;

public class SmashMask : BaseMask
{
    private float speedBoost = 1.5f;

    public override void OnActivate(GameObject player) {
        base.OnActivate(player);

        // TODO: Increase player speed slightly
    }

    public override void OnDeactivate(GameObject player) {
        base.OnDeactivate(player);
        
        // TODO: Slow player back down
    }

    public override void OnUpdate(GameObject player)
    {
        base.OnUpdate(player);

        // TODO: Continuously check for collisions with 'smashable' objects
    }
}
