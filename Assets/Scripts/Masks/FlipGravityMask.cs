
using UnityEngine;

public class FlipGravityMask : BaseMask
{
    private float baseGravityScale;

    public void Awake()
    {
        baseGravityScale = Physics2D.gravity.y;
    }

    public override void OnActivate(GameObject player)
    {
        // Flip gravity
        Physics2D.gravity = new Vector2(0, -baseGravityScale);

        // Give the player a little 'nudge' in the right direction
        player.GetComponent<Rigidbody2D>().AddForceY(1000f);

        base.OnActivate(player);
    }

    public override void OnDeactivate(GameObject player)
    {
        // Return gravity to normal
        Physics2D.gravity = new Vector2(0, baseGravityScale);

        // Give the player a little 'nudge' in the right direction
        player.GetComponent<Rigidbody2D>().AddForceY(-1000f);

        base.OnDeactivate(player);
    }
}
