using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakoutManager : MonoBehaviour
{
    private const int POINTSPERBLOCK = 100;

    [SerializeField]
    TMPro.TMP_Text scoreText;

    private int score;

    public void IncreaseScore()
    {
        score += POINTSPERBLOCK;
        scoreText.text = "Score: " + score.ToString();
    }
}
