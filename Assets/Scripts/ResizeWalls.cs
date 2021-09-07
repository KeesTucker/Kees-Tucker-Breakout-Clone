using UnityEngine;
using Mirror;

public class ResizeWalls : NetworkBehaviour
{
    private const float COLLIDER_THICNKESS = 1f;
    private const float HALF_COLLIDER_THICNKESS = COLLIDER_THICNKESS / 2f;
    private const float RED_ZONE_OFFSET = 1f;

    [SyncVar]
    private Vector2 screenBounds; //Edges of screen on positive axis

    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;
    [SerializeField] private GameObject top;
    [SerializeField] private GameObject bottom;

    //This is unimportant, I just wanted the colliders to resize to the screen dimensions
    public override void OnStartServer()
    {
        screenBounds = Camera.main.ViewportToWorldPoint(new Vector2(1f, 1f));
    }

    public override void OnStartClient()
    {
        Vector2 screenSizes = screenBounds * 2f;

        //Set positions
        right.transform.position = new Vector3(screenBounds.x + HALF_COLLIDER_THICNKESS, -HALF_COLLIDER_THICNKESS, 0);
        left.transform.position = new Vector3(-screenBounds.x - HALF_COLLIDER_THICNKESS, -HALF_COLLIDER_THICNKESS, 0);
        top.transform.position = new Vector3(0, screenBounds.y + HALF_COLLIDER_THICNKESS, 0);
        bottom.transform.position = new Vector3(0, -screenBounds.y - RED_ZONE_OFFSET - HALF_COLLIDER_THICNKESS, 0);

        //Set collider sizes
        right.GetComponent<BoxCollider>().size = new Vector3(COLLIDER_THICNKESS, screenSizes.y + RED_ZONE_OFFSET, COLLIDER_THICNKESS);
        left.GetComponent<BoxCollider>().size = new Vector3(COLLIDER_THICNKESS, screenSizes.y + RED_ZONE_OFFSET, COLLIDER_THICNKESS);
        top.GetComponent<BoxCollider>().size = new Vector3(screenSizes.x, COLLIDER_THICNKESS, COLLIDER_THICNKESS);
        bottom.GetComponent<BoxCollider>().size = new Vector3(screenSizes.x, COLLIDER_THICNKESS, COLLIDER_THICNKESS);
    }
}
