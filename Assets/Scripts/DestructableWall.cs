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

		// While destroying, randomize some rotation and positional movement while enabling gravity
		if (b_IsDestroying) { 
			foreach (GameObject childObj in m_Children)
			{
				childObj.transform.Rotate(0.0f, 0, 0.1f * Random.Range(-1, 1), Space.Self);
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

		foreach (GameObject childObj in m_Children)
        {
			float gravityScale = Random.Range(0.5f, 1.5f);
			childObj.GetComponent<Rigidbody2D>().gravityScale = gravityScale;
			childObj.GetComponent<Rigidbody2D>().AddForce(new Vector2(-50f*Random.Range(0, 3), 50f * Random.Range(0, 5)));
			childObj.GetComponent<BoxCollider2D>().enabled = false;
		}

	}
}
