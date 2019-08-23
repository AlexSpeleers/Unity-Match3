using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject[] dots;
    public GameObject tilePrefab;
    private BackGroundTile[,] allTiles;
    public GameObject[,] alldots;

    private IEnumerator coroutine;

    // Start is called before the first frame update
    void Awake()
    {
        allTiles = new BackGroundTile[width, height];
        alldots = new GameObject[width, height];
    }

    private void Start()
    {
        coroutine = SetUp(0.01f);
        StartCoroutine(coroutine);
    }
    IEnumerator SetUp(float waitTime)//create the game board
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i, j);
                GameObject backGroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity)as GameObject;//set sells for the dots under one GO in hierarchy
                backGroundTile.transform.parent = this.transform;
                backGroundTile.name = $"({i} {j})";
                yield return new WaitForSeconds(waitTime);
                //instatiating dots under cells
                int dotToUse = Random.Range(0, dots.Length);
                int maxIterations = 0;
                while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                }
                maxIterations = 0;
                GameObject dot = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = $"({i} {j})";
                alldots[i, j] = dot;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (alldots[column - 1, row].tag == piece.tag && alldots[column - 2, row].tag == piece.tag)//left right
            {
                return true;
            }
            if (alldots[column, row - 1].tag == piece.tag && alldots[column, row - 2].tag == piece.tag)//up down
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (alldots[column, row - 1].tag == piece.tag && alldots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (alldots[column - 1, row].tag == piece.tag && alldots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }
    #region destroy candies
    private void DestroyMatchesAt(int column, int row)
    {
        if (alldots[column, row].GetComponent<Dot>().isMatched)
        {
            Destroy(alldots[column, row]);
            alldots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (alldots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
    }
    #endregion
}
