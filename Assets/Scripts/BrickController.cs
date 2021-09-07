using UnityEngine;
using Mirror;

public class BrickController : NetworkBehaviour
{
    [SerializeField]
    private Color armouredColor = Color.gray; //Colour of armoured bricks.
    [SerializeField]
    private Gradient colorGradient;

    [SyncVar]
    [HideInInspector]
    public int brickLevel = 0;
    [SyncVar]
    [HideInInspector]
    public bool armouredBrick = false;
    [SyncVar]
    [HideInInspector]
    public float halfHeight;

    private void Start()
    {
        if (isClient)
        {
            SetBrickColor();
        }
    }
    private void SetBrickColor()
    {
        if (armouredBrick)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = armouredColor; //Set armoured colour.
        }
        else
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = colorGradient.Evaluate(brickLevel / halfHeight); //Set colour based on level
        }
    }

    [ClientRpc]
    public void RpcSetBrickColor(bool armoured, float gradientInterpolater)
    {
        if (armoured)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = armouredColor; //Set armoured colour.
        }
        else
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = colorGradient.Evaluate(gradientInterpolater); //Set colour based on level
        }
    }

    [ClientRpc]
    public void RpcSetNoArmourColour()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = colorGradient.Evaluate(brickLevel / halfHeight); //Set colour based on level
    }
}
