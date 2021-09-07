using UnityEngine;
using Mirror;

public class ResizeWalls : NetworkBehaviour
{
    private const float HALF_COLLIDER_THICNKESS = Constants.COLLIDER_THICKNESS / 2f;

    [SyncVar]
    private Vector2 screenBounds; //Edges of screen on positive axis

    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;
    [SerializeField] private GameObject top;
    [SerializeField] private GameObject bottom;

    public override void OnStartServer()
    {
        screenBounds = new Vector2(Constants.CAM_SIZE * Camera.main.aspect, Constants.CAM_SIZE);
    }

    //Resize colliders to the screen dimensions
    public override void OnStartClient()
    {
        Vector2 screenSizes = screenBounds * 2f;

        //Set positions
        right.transform.position = new Vector3(screenBounds.x + HALF_COLLIDER_THICNKESS, -HALF_COLLIDER_THICNKESS, 0);
        left.transform.position = new Vector3(-screenBounds.x - HALF_COLLIDER_THICNKESS, -HALF_COLLIDER_THICNKESS, 0);
        top.transform.position = new Vector3(0, screenBounds.y + HALF_COLLIDER_THICNKESS, 0);
        bottom.transform.position = new Vector3(0, -screenBounds.y - Constants.RED_ZONE_OFFSET - HALF_COLLIDER_THICNKESS, 0);

        //Set collider sizes
        right.GetComponent<BoxCollider>().size = new Vector3(Constants.COLLIDER_THICKNESS, screenSizes.y + Constants.RED_ZONE_OFFSET, Constants.COLLIDER_THICKNESS);
        left.GetComponent<BoxCollider>().size = new Vector3(Constants.COLLIDER_THICKNESS, screenSizes.y + Constants.RED_ZONE_OFFSET, Constants.COLLIDER_THICKNESS);
        top.GetComponent<BoxCollider>().size = new Vector3(screenSizes.x, Constants.COLLIDER_THICKNESS, Constants.COLLIDER_THICKNESS);
        bottom.GetComponent<BoxCollider>().size = new Vector3(screenSizes.x, Constants.COLLIDER_THICKNESS, Constants.COLLIDER_THICKNESS);
    }
}
