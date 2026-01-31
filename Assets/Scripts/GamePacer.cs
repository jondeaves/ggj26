using TMPro;
using UnityEditor;
using UnityEngine;

public class GamePacer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime = 0.3f;

    private Vector3 currentVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, player.transform.position.x, 1f),
            transform.position.y,
            transform.position.z
        );
    }
}
