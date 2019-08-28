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
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.alldots[i - 1, j];
                        GameObject rightDot = board.alldots[i + 1, j];
                        if (leftDot != null && rightDot != null)
                        {
                            if (leftDot.tag == curDot.tag && rightDot.tag == curDot.tag)
                            {
                                if (curDot.GetComponent<Dot>().isRowBomb 
                                    || leftDot.GetComponent<Dot>().isRowBomb 
                                    || rightDot.GetComponent<Dot>().isRowBomb)
                                {
                                    curMatches.Union(GetRowPieces(j));
                                }

                                if (curDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    curMatches.Union(GetColumnPieces(i));
                                }

                                if (leftDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    curMatches.Union(GetColumnPieces(i - 1));
                                }

                                if (rightDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    curMatches.Union(GetColumnPieces(i + 1));
                                }

                                if (!curMatches.Contains(leftDot))
                                {
                                    curMatches.Add(leftDot);
                                }
                                leftDot.GetComponent<Dot>().isMatched = true;
                                if (!curMatches.Contains(rightDot))
                                {
                                    curMatches.Add(rightDot);
                                }
                                rightDot.GetComponent<Dot>().isMatched = true;
                                if (!curMatches.Contains(curDot))
                                {
                                    curMatches.Add(curDot);
                                }
                                curDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.alldots[i, j + 1];
                        GameObject downDot = board.alldots[i, j - 1];
                        if (upDot != null && downDot != null)
                        {
                            if (upDot.tag == curDot.tag && downDot.tag == curDot.tag)
                            {
                                if (curDot.GetComponent<Dot>().isColumnBomb
                                    || upDot.GetComponent<Dot>().isColumnBomb
                                    || downDot.GetComponent<Dot>().isColumnBomb)
                                {
                                    curMatches.Union(GetColumnPieces(i));
                                }

                                if (curDot.GetComponent<Dot>().isRowBomb)
                                {
                                    curMatches.Union(GetRowPieces(j));
                                }

                                if (upDot.GetComponent<Dot>().isRowBomb)
                                {
                                    curMatches.Union(GetRowPieces(j + 1));
                                }

                                if (downDot.GetComponent<Dot>().isRowBomb)
                                {
                                    curMatches.Union(GetRowPieces(j - 1));
                                }

                                if (!curMatches.Contains(upDot))
                                {
                                    curMatches.Add(upDot);
                                }
                                upDot.GetComponent<Dot>().isMatched = true;
                                if (!curMatches.Contains(downDot))
                                {
                                    curMatches.Add(downDot);
                                }
                                downDot.GetComponent<Dot>().isMatched = true;
                                if (!curMatches.Contains(curDot))
                                {
                                    curMatches.Add(curDot);
                                }
                                curDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
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
                //decide what kind of bomb to make
                int typeOfBomb = Random.Range(0, 100);
                if (typeOfBomb < 50)
                {
                    //rowbomb
                    board.curDot.MakeRowBomb();
                }
                else if (typeOfBomb >= 50)
                {
                    //columnbonb
                    board.curDot.MakeColumnBomb();
                }
            }
            //is the other piece matched
            else if (board.curDot.otherDot != null)
            {

            }
        }
    }
}