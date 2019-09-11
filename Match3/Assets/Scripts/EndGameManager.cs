using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameType
{
    Moves, Time
}

[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}
public class EndGameManager : MonoBehaviour
{
    public GameObject movesLabel, timeLabel;
    public TMP_Text counter;
    public EndGameRequirements requirements;
    public int currentCounterValue;
    private float timerSeconds;
    private void Start()
    {
        SetupGame();
    }
    void SetupGame()
    {
        currentCounterValue = requirements.counterValue;
        if (requirements.gameType == GameType.Moves)
        {
            movesLabel.transform.localScale = Vector3.one;
            timeLabel.transform.localScale = Vector3.zero;
        }
        else
        {
            timerSeconds = 1f;
            movesLabel.transform.localScale = Vector3.zero;
            timeLabel.transform.localScale = Vector3.one;
        }
        counter.text = "" + currentCounterValue;
    }
    public void DecreaseCounterValue()
    {
        
         currentCounterValue--;
         counter.text = "" + currentCounterValue;
        if (currentCounterValue <= 0)
        {
            Debug.Log("u lost");
            currentCounterValue = 0;
            counter.text = "" + currentCounterValue;
        }
    }
    private void Update()
    {
        if (requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
