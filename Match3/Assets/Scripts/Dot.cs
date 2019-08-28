using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;


    private FindMatches findMatches;
    private Board board;
    public GameObject otherDot;
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 tempPos;
    [Header("Swipe")]
    public float swipeAngle = 0f;
    public float swipeResist = 1f;
    [Header("Powerup")]
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;
    private void Awake()
    {
        isColumnBomb = false;
        isRowBomb = false;
        board = FindObjectOfType<Board>();
       // targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousRow = row;
        //previousColumn = column;
    }
    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isColorBomb = true;
            GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, 0.2f);
        }*/
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > 0.1)//move dot to the target
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, 0.6f);
            if (board.alldots[column, row] != this.gameObject)
            {
                board.alldots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
        }
        if (Mathf.Abs(targetY - transform.position.y) > 0.1)//move dot to the target
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, 0.6f);
            if (board.alldots[column, row] != this.gameObject)
            {
                board.alldots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
    }

    private void OnMouseDown()
    {
        if (board.curState == GameState.move)
        {
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if (board.curState == GameState.move)
        {
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > swipeResist || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;//geometry
            MovePieces();
            board.curState = GameState.wait;
            board.curDot = this;
        }
        else
        {
            board.curState = GameState.move;
        }
    }
    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)//right swipe
        {
            otherDot = board.alldots[column + 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)//up swipe
        {
            otherDot = board.alldots[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)//left swipe
        {
            otherDot = board.alldots[column - 1, row];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)//down swipe
        {
            otherDot = board.alldots[column, row - 1];
            previousRow = row;
            previousColumn = column;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMove());
    }

    public IEnumerator CheckMove()
    {
        if (isColorBomb)
        {
            //this piece is a color bomb, and the other piece is the colo to destroy
            findMatches.MatchPiecesOfColor(otherDot.tag);
            isMatched = true;
        }
        else if (otherDot.GetComponent<Dot>().isColorBomb)
        {
            //the othe piece is a color bomb, and this piece has the color to destroy
            findMatches.MatchPiecesOfColor(this.gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
        }
        yield return new WaitForSeconds(0.5f);
        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                board.curDot = null;
                board.curState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
            //otherDot = null;
        }
    }
    void FindMatches()
    {
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.alldots[column - 1, row];
            GameObject rightDot1 = board.alldots[column + 1, row];
            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject uptDot1 = board.alldots[column, row + 1];
            GameObject downDot1 = board.alldots[column, row - 1];
            if (uptDot1 != null && downDot1 != null)
            {
                if (uptDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    uptDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }
}
