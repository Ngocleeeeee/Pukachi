using UnityEngine;

public class PanelManeger : MonoBehaviour
{
    public GameObject panelOptions, panelOver, panelGuide;
    public CountdownTimer classicTimer;
    public void Options()
    {
        classicTimer.isCountingDown = false;
        panelGuide.SetActive(false);
        panelOver.SetActive(false);
        panelOptions.SetActive(true);
    }
    public void Guide()
    {
        classicTimer.isCountingDown = false;
        panelOver.SetActive(false);
        panelOptions.SetActive(false);
        panelGuide.SetActive(true);
    }
    public void Over()
    {
        
    }
    public void backToGame()
    {
        if (classicTimer.currentTime <= 230f)
        {
            panelGuide.SetActive(false);
            panelOptions.SetActive(false) ;
            panelOver.SetActive(true) ;
        }
        else
        {
            classicTimer.isCountingDown = true;
            panelGuide.SetActive(false);
            panelOptions.SetActive(false);
            panelOver.SetActive(false);
        }
    }
}
