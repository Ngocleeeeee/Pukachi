using Assets.Script.Classic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public CountdownTimer countdownTimer; // Tham chiếu tới script CountdownTimer
    public Text scoreText, finalScoreText; // Text để hiển thị điểm
    private int score; // Điểm hiện tại

    private void Start()
    {
        score = 0;
        UpdateScoreText();
    }

    public void IncreaseScore()
    {
        if (countdownTimer.isCountingDown)
        {
                
            score += 1;
            score *= Mathf.CeilToInt(countdownTimer.getCurrentTime());
            UpdateScoreText();
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }
}