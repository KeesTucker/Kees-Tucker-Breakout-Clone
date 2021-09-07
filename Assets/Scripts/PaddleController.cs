using UnityEngine;

public class PaddleController : MonoBehaviour
{
    private float height;
    private float xEdge; //Edge of screen in world space.

    private void Start()
    {
        height = -Camera.main.ViewportToWorldPoint(new Vector3(0, 1f, 0)).y + 1.5f;
        //Find x coordinate at the right most part of the screen and then subtract half width of platform, we use this to clamp the x position of platform.
        xEdge = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0, 0)).x - (transform.localScale.x / 2f);
    }

    private void Update()
    {
        //Find mouse position in world.
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Clamp x so platform can't move off the screen and move position to x coordinate of mouse.
        transform.position = new Vector3(Mathf.Clamp(mousePos.x, -xEdge, xEdge), height, 0);
    }
}
