using UnityEngine;

public class ParentBricks : MonoBehaviour
{
    public void Start()
    {
        //Just sets all the brick's parent's to the brick manager to tidy things up.
        transform.parent = GameObject.Find("BrickManager").transform;
    }
}
