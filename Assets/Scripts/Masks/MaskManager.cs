using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaskManager : MonoBehaviour
{
    private Dictionary<KeyCode, BaseMask> masks;
    private BaseMask activeMask;

    private Dictionary<string, Image> maskIcons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        masks = new Dictionary<KeyCode, BaseMask>
        {
            { KeyCode.A, gameObject.AddComponent<FlipGravityMask>() },
            { KeyCode.S, gameObject.AddComponent<SmashMask>() }
        };

        maskIcons = new Dictionary<string, Image>();

		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MaskIcon"))
        {
            Image img = obj.GetComponent<Image>();

            if (img != null)
            {
                maskIcons.Add(obj.name, img);
            }
        }
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


		// Toggle active mask icon
		foreach (KeyValuePair<string, Image> maskIcon in maskIcons)
		{
			maskIcon.Value.color = new Color(255, 255, 255, activeMask != null && activeMask.GetType().Name == maskIcon.Key ? 1f : 0.5f);
		}

		foreach (Transform child in gameObject.transform)
		{
            if (child.gameObject.name.IndexOf("Mask") == -1)
                continue;

            child.gameObject.SetActive(activeMask && child.gameObject.name == activeMask.GetType().Name);
		}
	}

    private void ActivateMask(BaseMask newMask)
	{
		// If we have clicked the same mask as currently active
        // De-activate current mask, and finish there
		if (activeMask == newMask)
        {
			activeMask.OnDeactivate(gameObject);
			activeMask = null;

            return;
		}
        
        // If there is a mask active that isn't the new choice
        // Run through the old masks deactivate process
        if (activeMask != null)
        {
            // De-activate current mask
			activeMask.OnDeactivate(gameObject);
            activeMask = null;
        }

        // Activate new mask
		activeMask = newMask;
		activeMask.OnActivate(gameObject);
	}
}
