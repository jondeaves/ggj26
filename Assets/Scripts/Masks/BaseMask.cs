using UnityEngine;

public class BaseMask : MonoBehaviour
{
    [SerializeField] private float maskDuration = 3f;
    private float activeTimer = 0f;

    public bool isActive = false;

    public virtual void OnActivate(GameObject player)
    {

        activeTimer = 0f;
        isActive = true;
        
    }

    public virtual void OnDeactivate(GameObject player)
    {

        activeTimer = 0f;
        isActive = false;
    }

    public virtual void OnUpdate(GameObject player)
    {
        /*
        activeTimer += Time.deltaTime;
        if (activeTimer >= maskDuration)
        {
            OnDeactivate(player);
        }
        */
    }
}
