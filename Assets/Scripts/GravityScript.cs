using UnityEngine;

public class GravityScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
			Physics2D.gravity = new Vector2(0, -Physics2D.gravity.y);
		}
    }
}
