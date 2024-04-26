using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public float totalTime = 120f; // Tổng thời gian đếm ngược
    public Image countdownImage; // UI Image để hiển thị thời gian đếm ngược

    public float currentTime; // Thời gian hiện tại
    public bool isCountingDown; // Trạng thái đang đếm ngược

    public GameObject panel;

    private void Start()
    {
        currentTime = totalTime;
        isCountingDown = true;
        UpdateUI();
    }

    private void Update()
    {
        if (isCountingDown)
        {
            currentTime -= Time.deltaTime;
            //Debug.Log("counting " + currentTime);
            UpdateUI();

            if (currentTime <= 0f)
            {
                // Hành động khi đếm ngược kết thúc
                CountdownFinished();
            }
        }
    }

    private void UpdateUI()
    {
        countdownImage.fillAmount = currentTime / totalTime;
    }
    public float getCurrentTime()
    {
        return currentTime;
    }

    public float GetTimeInterval()
    {
        return (totalTime - currentTime) * Time.timeScale;
    }

    public void CountdownFinished()
    {
        isCountingDown = false;
        
        panel.SetActive(true);
        // Hành động khi đếm ngược kết thúc
        Debug.Log("Countdown finished!");

    }

    public void SetState(bool isCount)
    {
        isCountingDown = isCount;
    }

}