using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int pointsForGoal;
    public int pointsCollected;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;

    private void Start()
    {
        SetupGoals();
    }
    void SetupGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            //create new goal panel at the goalintro position
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform);
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].pointsForGoal;


            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].pointsForGoal;
        }
    }
    public void UpdateGoals()
    {
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = "" + levelGoals[i].pointsCollected + "/" + levelGoals[i].pointsForGoal;
            if (levelGoals[i].pointsCollected >= levelGoals[i].pointsForGoal)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoals[i].pointsForGoal + "/" + levelGoals[i].pointsForGoal;
            }
        }
        if (goalsCompleted >= levelGoals.Length)
        {
            Debug.Log("u won!");
        }
    }

    public void CompareGoal(string goalToCompare)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].pointsCollected++;
            }
        }
    }
}
