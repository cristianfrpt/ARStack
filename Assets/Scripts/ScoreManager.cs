using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private static TextMeshProUGUI scoreText;
    private static int score;

    public static int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            scoreText.text = score.ToString();
        }
    }

    private void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        score = 0;
    }
}
