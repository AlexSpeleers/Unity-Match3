using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    wait, move
}

public enum TileKind
{
    Breakable, Blank, Normal
}
[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
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
    public TileType[] boardLayout;
    private bool[,] blankSpaces;
    public GameObject[,] alldots;
    public Dot curDot;

    private FindMatches findMatches;
    private IEnumerator coroutine;

    // Start is called before the first frame update
    void Awake()
    {
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        alldots = new GameObject[width, height];
    }

    private void Start()
    {
        coroutine = SetUp(0.01f);
        StartCoroutine(coroutine);
    }

    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }
    IEnumerator SetUp(float waitTime)//create the game board
    {
        GenerateBlankSpaces();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPos = new Vector2(i, j + offSet);//spawn pos
                    GameObject backGroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;//set sells for the dots under one GO in hierarchy
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
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (alldots[column - 1, row] != null && alldots[column - 2, row] != null)
            {
                if (alldots[column - 1, row].tag == piece.tag && alldots[column - 2, row].tag == piece.tag)//left right
                {
                    return true;
                }
            }
            if (alldots[column, row - 1] != null && alldots[column, row - 2] != null)
            {
                if (alldots[column, row - 1].tag == piece.tag && alldots[column, row - 2].tag == piece.tag)//up down
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (alldots[column, row - 1] != null && alldots[column, row - 2] != null)
                {
                    if (alldots[column, row - 1].tag == piece.tag && alldots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (alldots[column - 1, row] != null && alldots[column - 2, row] != null)
                {
                    if (alldots[column - 1, row].tag == piece.tag && alldots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }   
            }
        }
        return false;
    }
    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.curMatches[0].GetComponent<Dot>();
        if (firstPiece != null)
        {
            foreach (GameObject curPiece in findMatches.curMatches)
            {
                Dot dot = curPiece.GetComponent<Dot>();
                if (dot.row == firstPiece.row)
                {
                    numberHorizontal++;
                }
                if (dot.column == firstPiece.column)
                {
                    numberVertical++;
                }
            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);
    }

    private void CheckToMakeBombs()
    {
        if (findMatches.curMatches.Count == 4 || findMatches.curMatches.Count == 7)
        {
            findMatches.CheckBombs();
        }
        if (findMatches.curMatches.Count == 5 || findMatches.curMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                //make color bomb
                if (curDot != null)
                {
                    if (curDot.isMatched)
                    {
                        if (!curDot.isColorBomb)
                        {
                            curDot.isMatched = false;
                            curDot.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (curDot.otherDot != null)
                        {
                            Dot otherDot = curDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isColorBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //make a adjecement bomb
                if (curDot != null)
                {
                    if (curDot.isMatched)
                    {
                        if (!curDot.isAdjacentBomb)
                        {
                            curDot.isMatched = false;
                            curDot.MakeAdjacentBomb();
                        }
                    }
                    else
                    {
                        if (curDot.otherDot != null)
                        {
                            Dot otherDot = curDot.otherDot.GetComponent<Dot>();
                            if (otherDot.isMatched)
                            {
                                if (!otherDot.isAdjacentBomb)
                                {
                                    otherDot.isMatched = false;
                                    otherDot.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    #region destroy candies
    private void DestroyMatchesAt(int column, int row)
    {
        if (alldots[column, row].GetComponent<Dot>().isMatched)
        {
            //how many elements are in the matched pieces list from findmatches?
            if (findMatches.curMatches.Count == 4 || findMatches.curMatches.Count == 7)
            {
                CheckToMakeBombs();
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
        StartCoroutine(DecreaseRowCo2());
    }
    #endregion

    private IEnumerator DecreaseRowCo2()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if the current spot isn't blank and is empty
                if (!blankSpaces[i, j] && alldots[i, j] == null)
                {
                    //loop from the space above the top of the column
                    for (int k = j + 1; k < height; k++)
                    {
                        //if a dot exists
                        if (alldots[i, k] != null)
                        {
                            //move that dot to this empty space
                            alldots[i, k].GetComponent<Dot>().row = j;
                            //set that spot to null
                            alldots[i, k] = null;
                            break;
                        }                 
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCo());
    }
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
                if (alldots[i, j] == null && !blankSpaces[i, j])
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
