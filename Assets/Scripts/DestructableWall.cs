using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class DestructableWall : MonoBehaviour
{
	// Debugging
	public bool b_TriggerDestroy = false;

	private List<GameObject> m_Children = new List<GameObject>();
    private bool b_IsDestroying = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		foreach (Transform child in transform)
		{
			m_Children.Add(child.gameObject);
		}
	}

    // Update is called once per frame
    void Update()
    {
        if (b_TriggerDestroy)
        {
            this.DestroyWall();
		}

		// While destroying, randomize some rotation
		if (b_IsDestroying) { 
			foreach (GameObject childObj in m_Children)
			{
				childObj.transform.Rotate(0.0f, 0, 0.1f * Random.Range(-3, 3), Space.Self);
			}
		}
	}

	public void TriggerDestruction()
	{
		this.DestroyWall();
	}

    void DestroyWall()
    {
		b_TriggerDestroy = false;
		b_IsDestroying = true;

		gameObject.GetComponent<BoxCollider2D>().enabled = false;

        foreach (GameObject childObj in m_Children)
        {
			float gravityScale = Random.Range(0.5f, 1.5f);
            childObj.GetComponent<BoxCollider2D>().enabled = false;
            
			// When first destroyed, randomize positional movement while enabling gravity
            childObj.GetComponent<Rigidbody2D>().gravityScale = gravityScale;
            childObj.GetComponent<Rigidbody2D>().AddForce(new Vector2(-150f*Random.Range(0, 3), 150f * Random.Range(0, 5)));
			childObj.GetComponent<Rigidbody2D>().mass = 5;

        }

	}
}
