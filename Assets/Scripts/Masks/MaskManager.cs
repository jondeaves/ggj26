using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class MaskManager : MonoBehaviour
{
    private Dictionary<KeyCode, BaseMask> masks;
    private BaseMask activeMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        masks = new Dictionary<KeyCode, BaseMask>
        {
            { KeyCode.A, gameObject.AddComponent<FlipGravityMask>() },
            { KeyCode.S, gameObject.AddComponent<SmashMask>() }
        };
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in masks)
        {
            if (Input.GetKeyUp(item.Key))
            {
                ActivateMask(item.Value);
            }
        }

        if (activeMask != null)
        {
            activeMask.OnUpdate(gameObject);
        }
    }

    private void ActivateMask(BaseMask newMask)
    {
        if (activeMask == newMask)
        {
            // De-activate current mask
            if (activeMask.isActive)
            {
                activeMask.OnDeactivate(gameObject);
            } else
            {
                activeMask.OnActivate(gameObject);
            }
            
            return;
        }
        
        if (activeMask != null)
        {
            // De-activate current mask
            activeMask.OnDeactivate(gameObject);

        }

        // Activate new mask
        activeMask = newMask;
        activeMask.OnActivate(gameObject);
    }
}
