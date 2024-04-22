using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public float totalTime = 240f; // Tổng thời gian đếm ngược
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

            if (currentTime <= 230f)
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

    public void CountdownFinished()
    {
        panel.SetActive(true);
        isCountingDown = false;
        // Hành động khi đếm ngược kết thúc
        Debug.Log("Countdown finished!");

    }
}