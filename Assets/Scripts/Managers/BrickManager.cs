using UnityEngine;
using Mirror;

public class BrickManager : NetworkBehaviour
{
    [SerializeField]
    private BreakoutManager gameManager;

    [SerializeField]
    private GameObject brickGO;
    private GameObject[,] bricks;

    [SerializeField] 
    [Range(2, 64)]
    private int width = 10;
    [SerializeField]
    [Range(2, 64)]
    private int height = 10;
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

    //Server side
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
                    CreateBrick(x, y);
                }
            }
        }
    }

    //Create a brick at the specified grid coordinates
    private void CreateBrick(int x, int y)
    {
        gameManager.numBricks++;

        bricks[x, y] = Instantiate(brickGO);
        bricks[x, y].transform.parent = transform;
        //Position bricks, make sure y positions are half of x as the bricks are half as high as they are wide. We divide y by 2 as bricks are half as high as they are wide.
        bricks[x, y].transform.position = new Vector2((x - halfWidth) * gridScale, (y - halfHeight) / 2f * gridScale + brickHeightOffset);
        bricks[x, y].transform.localScale = new Vector2(gridScale, gridScale);
        NetworkServer.Spawn(bricks[x, y]);

        BrickColorGenerator generator = bricks[x, y].GetComponent<BrickColorGenerator>();
        generator.brickLevel = y - numBricksStartHeight; //Set brick level based on brick height.
        generator.halfHeight = halfHeight;
        if (Random.Range(0f, 1f) > 0.8f) //20% chance to generate an armoured brick, armoured bricks take 1 extra hit.
        {
            generator.armouredBrick = true; //Set the sync var for new clients
            generator.RpcSetBrickColor(true, 0);
        }
        else
        {
            generator.RpcSetBrickColor(false, (y - numBricksStartHeight) / halfHeight);
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

    //Just makes sure the grid is fit to the screen properly. Only on server, clients resize their cams to the grid.
    private void ScaleGrid()
    {
        gridScale = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0)).x * 2 / width;
        brickHeightOffset = (Camera.main.ViewportToWorldPoint(new Vector2(0, 1f)).y) - (gridScale * height / 4f);
    }
}
