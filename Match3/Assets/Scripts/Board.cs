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
    public GameObject breakableTilePrefab;
    public GameObject destroyFX;
    public TileType[] boardLayout;
    private bool[,] blankSpaces;
    private BackGroundTile[,] breakableTiles;
    public GameObject[,] alldots;
    public Dot curDot;

    public float refillDelay = 0.5f;
    private FindMatches findMatches;
    private IEnumerator coroutine;

    // Start is called before the first frame update
    void Awake()
    {
        breakableTiles = new BackGroundTile[width, height];
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

    public void GenerateBreakableTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //if a tile is a breakable tile
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                //creating breakable tile
                Vector2 tempPos = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPos, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackGroundTile>();
            }
        }
    }

    IEnumerator SetUp(float waitTime)//create the game board
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPos = new Vector2(i, j + offSet);//spawn pos
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backGroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;//set sells for the dots under one GO in hierarchy
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
            if (findMatches.curMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }

            //does a tile need to break?
            if (breakableTiles[column, row] != null)
            {
                //if it does - substrackt 1 hp
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
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
        yield return new WaitForSeconds(refillDelay * 0.4f);
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

        if (IsDeadlocked())
        {
            ShuffleBoard();
        }
        curState = GameState.move;
    }

    private void SwitchPieces(int column, int row, Vector2 dir)
    {
        //take the second piece and save it in the holder
        GameObject holder = alldots[column + (int)dir.x, row + (int)dir.y] as GameObject;
        //switching first dot to be the second pos
        alldots[column + (int)dir.x, row + (int)dir.y] = alldots[column, row];
        //set the first dot to be the second dot
        alldots[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (alldots[i, j] != null)
                {
                    //make sure first and second to the right are in the board
                    if (i < width - 2)
                    {
                        //check if the dots to the right and two to the right exist
                        if (alldots[i + 1, j] != null && alldots[i + 2, j] != null)
                        {
                            if (alldots[i + 1, j].tag == alldots[i, j].tag && alldots[i + 2, j].tag == alldots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        //check if dots above exist
                        if (alldots[i, j + 1] != null && alldots[i, j + 2] != null)
                        {
                            if (alldots[i, j + 1].tag == alldots[i, j].tag && alldots[i, j + 2].tag == alldots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 dir)
    {
        SwitchPieces(column, row, dir);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, dir);
            return true;
        }
        SwitchPieces(column, row, dir);
        return false;
    }

    private bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (alldots[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        //Add every piece to this list
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (alldots[i, j] != null)
                {
                    newBoard.Add(alldots[i, j]);
                }
            }
        }
        //for every spot on the board
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if this spot shouldn't be blank
                if (!blankSpaces[i, j])
                {
                    //pick a random number
                    int pieceToUse = Random.Range(9, newBoard.Count);
                    //assign columns and row for pieces
                    int maxIterations = 0;
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                    }
                    //container for the piece
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    maxIterations = 0;
                    piece.column = i;
                    piece.row = j;
                    //fiil the dot array with piece
                    alldots[i, j] = newBoard[pieceToUse];
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        if (IsDeadlocked())
        {
            ShuffleBoard();
        }
    }
    
}
