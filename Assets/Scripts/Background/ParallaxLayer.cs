using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [SerializeField] private Transform background;
    [SerializeField] private float parallaxMultiplier; // 0 = Static, 1 = Matches camera movement speed
    [SerializeField] private float imageWidthOffset = 10; // Adjusts tiny gaps/seams between tiled sprites
    private float imageFullWidth;
    private float imageHalfWidth;
    
    public void CalculateImageWidth()
    {
        if (background != null)
        {
            // Automatically get the image width from the SpriteRenderer bounds
            imageFullWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
            imageHalfWidth = imageFullWidth / 2;
        }
    }

    public void Move(float distanceToMove)
    {
        // Move the background based on the parallax intensity
        background.position += Vector3.right * (distanceToMove * parallaxMultiplier);
    }

    public void LoopBackground(float cameraLeftEdge, float cameraRightEdge)
    {
        // Calculate the actual left and right boundaries of the sprite
        float imageRightEdge = (background.position.x + imageHalfWidth) - imageWidthOffset;
        float imageLeftEdge = (background.position.x - imageHalfWidth) + imageWidthOffset;

        // SEAMLESS LOOP: If the sprite exits the left side, teleport it to the right
        if (imageRightEdge < cameraLeftEdge)
        {
            background.position += Vector3.right * imageFullWidth;
        }
        // If the sprite exits the right side, move it to the left
        else if (imageLeftEdge > cameraRightEdge)
        {
            
            background.position += Vector3.left * imageFullWidth;
        }
    }
}
