using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeWalls : MonoBehaviour
{
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;
    [SerializeField] private GameObject top;
    [SerializeField] private GameObject bottom;

    //This is unimportant, I just wanted the colliders to resize to the screen dimensions
    void Start()
    {
        Vector2 screenBounds = Camera.main.ViewportToWorldPoint(new Vector2(1f, 1f));

        //Set positions
        right.transform.position = new Vector3(screenBounds.x + 0.5f, -0.5f, 0);
        left.transform.position = new Vector3(-screenBounds.x - 0.5f, -0.5f, 0);
        top.transform.position = new Vector3(0, screenBounds.y + 0.5f, 0);
        bottom.transform.position = new Vector3(0, -screenBounds.y - 1.5f, 0);

        right.GetComponent<BoxCollider>().size = new Vector3(1, screenBounds.y * 2 + 1, 1);
        left.GetComponent<BoxCollider>().size = new Vector3(1, screenBounds.y * 2 + 1, 1);
        top.GetComponent<BoxCollider>().size = new Vector3(screenBounds.x * 2, 1, 1);
        bottom.GetComponent<BoxCollider>().size = new Vector3(screenBounds.x * 2, 1, 1);
    }
}
