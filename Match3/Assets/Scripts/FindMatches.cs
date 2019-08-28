using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> curMatches = new List<GameObject>();

    void Awake()
    {
        board = FindObjectOfType<Board>();
    }
    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> curDots = new List<GameObject>();
        if (dot1.isRowBomb)
        {
            curMatches.Union(GetRowPieces(dot1.row));
        }

        if (dot2.isRowBomb)
        {
            curMatches.Union(GetRowPieces(dot2.row));
        }

        if (dot3.isRowBomb)
        {
            curMatches.Union(GetRowPieces(dot3.row));
        }
        return curDots;
    }
    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> curDots = new List<GameObject>();
        if (dot1.isColumnBomb)
        {
            curMatches.Union(GetColumnPieces(dot1.column));
        }

        if (dot2.isColumnBomb)
        {
            curMatches.Union(GetColumnPieces(dot2.column));
        }

        if (dot3.isColumnBomb)
        {
            curMatches.Union(GetColumnPieces(dot3.column));
        }
        return curDots;
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!curMatches.Contains(dot))
        {
            curMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject curDot = board.alldots[i, j];
                if (curDot != null)
                {
                    Dot curDotDot = curDot.GetComponent<Dot>();
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.alldots[i - 1, j];
                        GameObject rightDot = board.alldots[i + 1, j];
                        if (leftDot != null && rightDot != null)
                        {
                            Dot rightDotDot = rightDot.GetComponent<Dot>();
                            Dot leftDotDot = leftDot.GetComponent<Dot>();

                            if (leftDot != null && rightDot != null)
                            {
                                if (leftDot.tag == curDot.tag && rightDot.tag == curDot.tag)
                                {
                                    curMatches.Union(IsRowBomb(leftDotDot, curDotDot, rightDotDot));
                                    curMatches.Union(IsColumnBomb(leftDotDot, curDotDot, rightDotDot));
                                    GetNearbyPieces(leftDot, curDot, rightDot);
                                }
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.alldots[i, j + 1];
                        GameObject downDot = board.alldots[i, j - 1];
                        if (upDot != null && downDot != null)
                        {
                            Dot upDotDot = upDot.GetComponent<Dot>();
                            Dot downDotDot = downDot.GetComponent<Dot>();
                            if (upDot != null && downDot != null)
                            {
                                if (upDot.tag == curDot.tag && downDot.tag == curDot.tag)
                                {
                                    curMatches.Union(IsColumnBomb(upDotDot, curDotDot, downDotDot));
                                    curMatches.Union(IsRowBomb(upDotDot, curDotDot, downDotDot));
                                    GetNearbyPieces(upDot, curDot, downDot);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; i < board.height; j++)
            {
                //Check if that piece exists
                if (board.alldots[i, j] != null)
                {
                    //check the tag on thaat dot
                    if (board.alldots[i, j].tag == color)
                    {
                        //set that dot to be matched
                        board.alldots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }
    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.alldots[column, i] != null)
            {
                dots.Add(board.alldots[column, i]);
                board.alldots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.alldots[i, row] != null)
            {
                dots.Add(board.alldots[i, row]);
                board.alldots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }

    public void CheckBombs()
    {
        //did the player smth?
        if (board.curDot != null)
        {
            //is the piece player moved matched?
            if (board.curDot.isMatched)
            {
                //make it unmatched
                board.curDot.isMatched = false;
                if ((board.curDot.swipeAngle > -45 && board.curDot.swipeAngle <= 45)
                    || (board.curDot.swipeAngle < -135 || board.curDot.swipeAngle >= 135))
                {
                    board.curDot.MakeRowBomb();
                }
                else
                {
                    board.curDot.MakeColumnBomb();
                }
            }
            //is the other piece matched
            else if (board.curDot.otherDot != null)
            {
                Dot otherDot = board.curDot.otherDot.GetComponent<Dot>();
                //is othedot Matched
                if (otherDot.isMatched)
                {
                    otherDot.isMatched = false;
                    if ((board.curDot.swipeAngle > -45 && board.curDot.swipeAngle <= 45)
                        || (board.curDot.swipeAngle < -135 || board.curDot.swipeAngle >= 135))
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }
}