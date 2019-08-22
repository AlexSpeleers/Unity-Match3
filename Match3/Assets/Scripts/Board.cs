﻿using System.Collections;
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
                GameObject dot = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = $"({i} {j})";
                alldots[i, j] = dot;
            }
        }
    }
}
