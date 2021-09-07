using UnityEngine;
using Mirror;

public class BrickManager : NetworkBehaviour
{
    [SerializeField]
    private GameObject brickGO;
    private GameObject[,] bricks;

    [SerializeField] [Range(2, 64)]
    private int width = 16;
    [Range(2, 64)]
    public int height = 16;
    [Range(0, 1)]
    public float brickStartHeight = 0.5f; //Percentage of vertical screen to fill with bricks.
    
    private float gridScale; //Scale factor to fit grid to screen
    private float brickHeightOffset; //Just use this to make sure bricks line up nicely with top of screen.

    private float halfWidth;
    private float halfHeight;
    private int numBricksStartHeight; //Holds percentage converted to number of blocks.

    public override void OnStartServer()
    {
        CreateBricks();
    }

    public void CreateBricks() //Brick manager also uses this to reset bricks
    {
        if (isServer)
        {
            bricks = new GameObject[width, height];

            halfWidth = (width - 1f) / 2f;
            halfHeight = (height - 1f) / 2f;
            numBricksStartHeight = (int)(height * brickStartHeight); //Calculates at what y coordinate should we start creating bricks

            ScaleGrid(); //Makes sure grid is fit to the screen

            for (int x = 0; x < width; x++)
            {
                for (int y = numBricksStartHeight; y < height; y++)
                {
                    bricks[x, y] = Instantiate(brickGO);
                    bricks[x, y].transform.parent = transform;
                    //Position bricks, make sure y positions are half of x as the bricks are half as high as they are wide. We divide y by 2 as bricks are half as high as they are wide.
                    bricks[x, y].transform.position = new Vector2((x - halfWidth) * gridScale, (y - halfHeight) / 2f * gridScale + brickHeightOffset);
                    bricks[x, y].transform.localScale = new Vector2(gridScale, gridScale);
                    NetworkServer.Spawn(bricks[x, y]);

                    BrickController controller = bricks[x, y].GetComponent<BrickController>();
                    controller.brickLevel = y - numBricksStartHeight; //Set brick level based on brick height.
                    controller.halfHeight = halfHeight;
                    if (Random.Range(0f, 1f) > 0.8f) //20% chance to generate an armoured brick, armoured bricks take 1 extra hit.
                    {
                        controller.armouredBrick = true;
                        controller.RpcSetBrickColor(true, 0);
                    }
                    else
                    {
                        controller.RpcSetBrickColor(false, (y - numBricksStartHeight) / halfHeight);
                    }
                }
            }
        }
    }

    public void DestroyBricks() //Brick manager also uses this to reset bricks
    {
        if (isServer)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = numBricksStartHeight; y < height; y++)
                {
                    NetworkServer.Destroy(bricks[x, y]);
                }
            }
            bricks = new GameObject[width, height];
        }
    }

    //Don't worry about this it just makes sure the grid is fit to the screen properly.
    private void ScaleGrid()
    {
        gridScale = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0)).x * 2 / width;
        brickHeightOffset = (Camera.main.ViewportToWorldPoint(new Vector2(0, 1f)).y) - (gridScale * height / 4f);
    }
}
