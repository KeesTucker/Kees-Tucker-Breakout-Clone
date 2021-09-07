using UnityEngine;
using Mirror;

public class BrickColorGenerator : NetworkBehaviour
{
    [SerializeField]
    private Color armouredColor = Color.gray; //Colour of armoured bricks.
    [SerializeField]
    private Gradient colorGradient; //Gradient to calculate normal bricks colour.

    [SyncVar]
    [HideInInspector]
    public int brickLevel = 0; //Height level of brick, equivalent to colour.
    [SyncVar]
    [HideInInspector]
    public bool armouredBrick = false; //Takes 2 hits to destroy if true
    [SyncVar]
    [HideInInspector]
    public float halfHeight; //Half brick grid height.

    private void Start()
    {
        //Update all brick colours when joining as a client. Our sync vars will be updated.
        if (isClient)
        {
            SetBrickColor();
        }
    }

    //These set color functions are very ugly, I want to unify them but I need the sync vars for clients to connect with. Still thinking on it.
    //[WIP]

    //Client side
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

    //Used on initial generation of bricks, the sync vars can't update in time so I send the data in the function args.
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

    //Used when changing a brick from armoured to unarmoured, again the armoured sync var can't update fast enough so this just always sets colour to normal
    [ClientRpc]
    public void RpcSetNormalBrickColour()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = colorGradient.Evaluate(brickLevel / halfHeight); //Set colour based on level
    }
}
