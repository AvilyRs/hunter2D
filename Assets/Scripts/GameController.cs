using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private int score = 0;

    public TextMeshProUGUI scoreText;

    public static GameController instance;

    private void Awake()
    {
        instance = this;
    }

    public void GetCoin()
    {
        score += 1;
        scoreText.text = $"x{score}";
    }
}
