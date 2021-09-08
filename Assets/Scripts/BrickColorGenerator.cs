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
    public int brickLevel; //Height level of brick, equivalent to colour.
    [SyncVar]
    [HideInInspector]
    public bool armouredBrick = false; //Takes 2 hits to destroy if true
    [SyncVar]
    [HideInInspector]
    public float halfHeight; //Half brick grid height.

    public override void OnStartClient()
    {
        //Update all brick colours when joining as a client. Use the sync vars.
        SetBrickColor(armouredBrick, brickLevel / halfHeight);
    }

    [ClientRpc]
    public void RpcSetBrickColor(bool armouredBrick, float colorHeight)
    {
        //Use the values from the sync vars
        SetBrickColor(armouredBrick, colorHeight);
    }

    [ClientRpc]
    public void RpcSetNormalBrickColor()
    {
        SetBrickColor(false, brickLevel / halfHeight);
    }

    [Client]
    public void SetBrickColor(bool armouredBrick, float colorHeight)
    {
        if (!float.IsNaN(colorHeight))
        {
            if (armouredBrick)
            {
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = armouredColor; //Set armoured colour.
            }
            else
            {
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = colorGradient.Evaluate(colorHeight); //Set colour based on level
            }
        }
    }
}
