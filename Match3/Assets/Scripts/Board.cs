using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    wait, move
}
public class Board : MonoBehaviour
{
    public GameState curState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject[] dots;
    public GameObject tilePrefab;
    public GameObject destroyFX;
    private BackGroundTile[,] allTiles;
    public GameObject[,] alldots;
    public Dot curDot;

    private FindMatches findMatches;
    private IEnumerator coroutine;

    // Start is called before the first frame update
    void Awake()
    {
        findMatches = FindObjectOfType<FindMatches>();
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
                Vector2 tempPos = new Vector2(i, j + offSet);//spawn pos
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
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;
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
            //how many elements are in the matched pieces list from findmatches?
            if (findMatches.curMatches.Count == 4 || findMatches.curMatches.Count == 7)
            {
                findMatches.CheckBombs();
            }
            GameObject particle = Instantiate(destroyFX,
                alldots[column, row].transform.position,
                Quaternion.identity);

            Destroy(particle, 0.5f);
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
        findMatches.curMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }
    #endregion

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (alldots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                { 
                    alldots[i, j].GetComponent<Dot>().row -= nullCount;
                    alldots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (alldots[i, j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                    alldots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }

        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (alldots[i, j] != null)
                {
                    if (alldots[i, j].GetComponent<Dot>().isMatched)
                        return true;
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.5f);
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
        }
        findMatches.curMatches.Clear();
        curDot = null;
        yield return new WaitForSeconds(0.5f);
        curState = GameState.move;
    }
}
