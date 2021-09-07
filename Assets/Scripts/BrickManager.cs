using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    [SerializeField]
    private GameObject brickGO;
    private GameObject[,] bricks;

    [SerializeField][Range(0, 2f)]
    private float gridScale = 1f;
    [SerializeField][Range(2, 64)]
    private int width = 16;
    [SerializeField][Range(2, 64)]
    private int height = 16;
    [SerializeField][Range(0, 1)]
    private float brickStartHeight = 0.5f;

    private float halfWidth;
    private float halfHeight;
    private int numBrickStartHeight;

    private void Start()
    {
        CreateBricks();
    }

    private void CreateBricks() 
    {
        bricks = new GameObject[width, height];

        halfWidth = (width - 1f) / 2f;
        halfHeight = (height - 1f) / 2f;
        numBrickStartHeight = (int)(height * brickStartHeight);

        for (int x = 0; x < width; x++)
        {
            for (int y = numBrickStartHeight; y < height; y++)
            {
                bricks[x, y] = Instantiate(brickGO);
                bricks[x, y].transform.parent = transform;
                bricks[x, y].transform.position = new Vector2(x - halfWidth, y - halfHeight) * gridScale;
                bricks[x, y].transform.localScale = new Vector2(gridScale, gridScale);
            }
        }
    }

    private void DestroyBricks()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = numBrickStartHeight; y < height; y++)
            {
                Destroy(bricks[x, y]);
            }
        }

        bricks = new GameObject[width, height];
    }
}
