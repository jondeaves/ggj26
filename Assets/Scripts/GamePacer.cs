using TMPro;
using UnityEditor;
using UnityEngine;

public class GamePacer : MonoBehaviour
{
    [SerializeField] private Transform player;

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
