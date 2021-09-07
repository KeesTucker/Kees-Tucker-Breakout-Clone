using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    [SerializeField]
    private GameObject brickGO;
    private GameObject[,] bricks;

    [SerializeField] [Range(2, 64)]
    private int width = 16;
    [SerializeField] [Range(2, 64)]
    private int height = 16;
    [SerializeField] [Range(0, 1)]
    private float brickStartHeight = 0.5f; //Percentage of vertical screen to fill with bricks.
    [SerializeField]
    Gradient colorGradient;

    private float gridScale; //Scale factor to fit grid to screen
    private float brickOffsetHeight; //Just use this to make sure bricks line up nicely with top of screen.

    private float halfWidth;
    private float halfHeight;
    private int numBricksStartHeight; //Holds percentage converted to number of blocks.

    private void Start()
    {
        CreateBricks();
    }

    private void CreateBricks() 
    {
        bricks = new GameObject[width, height];

        halfWidth = (width - 1f) / 2f;
        halfHeight = (height - 1f) / 2f;
        numBricksStartHeight = (int)(height * brickStartHeight);

        ScaleGrid();

        for (int x = 0; x < width; x++)
        {
            for (int y = numBricksStartHeight; y < height; y++)
            {
                bricks[x, y] = Instantiate(brickGO);
                bricks[x, y].transform.parent = transform;
                //Position bricks, make sure y positions are half of x as the bricks are half as high as they are wide.
                bricks[x, y].transform.position = new Vector2((x - halfWidth) * gridScale, (y - halfHeight) / 2f * gridScale + brickOffsetHeight);
                bricks[x, y].transform.localScale = new Vector2(gridScale, gridScale);

                bricks[x, y].transform.GetChild(0).GetComponent<SpriteRenderer>().color = colorGradient.Evaluate((float)(y - numBricksStartHeight) / height / brickStartHeight);
            }
        }
    }

    //Don't worry about this it just makes sure the grid is fit to the screen properly.
    private void ScaleGrid()
    {
        gridScale = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0)).x * 2 / width;
        brickOffsetHeight = (Camera.main.ViewportToWorldPoint(new Vector2(0, 1f)).y) - (gridScale * height / 4f);
    }
}
