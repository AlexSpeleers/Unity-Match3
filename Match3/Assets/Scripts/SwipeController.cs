using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    public Vector2 firstPos;
    private Vector2 secondPos;
    public Vector2 moveDir;
    public bool isControlling = false;
   
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isControlling = true;
            firstPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            isControlling = false;
            secondPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            moveDir = new Vector2(secondPos.x - firstPos.x, secondPos.y - firstPos.y);
        }
    }
}
