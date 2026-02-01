using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Camera mainCamera;
    private float lastCameraPositionX;
    private float cameraHalfWidth;

    [SerializeField] private ParallaxLayer[] backgroundLayers;

    private void Awake()
    {
        mainCamera = Camera.main;
        // Calculate the half-width of the camera's view for boundary detection
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        CalculateImageWidth();
    }

    private void Start()
    {
        lastCameraPositionX = mainCamera.transform.position.x;
    }

    private void LateUpdate()
    {

        float currentCameraPositionX = mainCamera.transform.position.x;
        float distanceToMove = currentCameraPositionX - lastCameraPositionX;

        float cameraLeftEdge = currentCameraPositionX - cameraHalfWidth;
        float cameraRightEdge = currentCameraPositionX + cameraHalfWidth;

        foreach (ParallaxLayer layer in backgroundLayers)
        {
            layer.Move(distanceToMove);
            layer.LoopBackground(cameraLeftEdge, cameraRightEdge);
        }

        lastCameraPositionX = currentCameraPositionX;
    }

    private void CalculateImageWidth()
    {
        foreach (ParallaxLayer layer in backgroundLayers)
        {
            layer.CalculateImageWidth();
        }
    }
}
