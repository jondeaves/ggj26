
using Unity.VisualScripting;
using UnityEngine;

public class SmashMask : BaseMask
{
    private float speedBoost = 1.5f;

    public override void OnActivate(GameObject player) {
        base.OnActivate(player);

        player.GetComponent<Player>().boostMultiplier = speedBoost;
    }

    public override void OnDeactivate(GameObject player) {
        base.OnDeactivate(player);

		player.GetComponent<Player>().boostMultiplier = 1f;
	}

    public override void OnUpdate(GameObject player)
    {
        base.OnUpdate(player);

        // TODO: Continuously check for collisions with 'smashable' objects
    }
}
