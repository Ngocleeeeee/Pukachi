using Assets.Script.AutoMatch;
using Assets.Script.Classic;
using Assets.Script.Gravity;
using Assets.Script.WallMode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public GameObject pause; // Reference to the first button GameObject
    public GameObject resume;
    public Text numberHint, numberReset;
    public int  hintCount = 5, resetCount = 5;

    // Start is called before the first frame update
    public void _MenuScene()
    {
        SceneManager.LoadScene("_MenuScene");
    }
    public void _ModeScene()
    {
        SceneManager.LoadScene("_ModeScene");
    }
    public void _OptionsScene()
    {
        SceneManager.LoadScene("_OptionsScene");
    }

    //Mode ---------------------
    public void _ModeClassic()
    {
        SceneManager.LoadScene("ModeClassic");
    }
    public void _ModeGravity()
    {
        SceneManager.LoadScene("ModeGravity");
    }
    public void _ModeWall()
    {
        SceneManager.LoadScene("ModeWall");
    }

    //Play -----------------
    public void _PlayClassic()
    {
        SceneManager.LoadScene("PlayClassic");
    }

    public void _PlayGravity()
    {
        SceneManager.LoadScene("PlayGravity");
    }
    public void _PlayWall()
    {
        SceneManager.LoadScene("PlayWall");
    }
    public void AutoMatch()
    {
        SceneManager.LoadScene("PlayAUTOPlayScene");
    }

    //classic--------------
    public void resetMatrixClassic()
    {
        BaseClassic newB = new BaseClassic();
        if (resetCount >= 0)
        {
            resetCount--;
            numberReset.text = "Reset(" + resetCount.ToString() + ")";
            newB.ResetMatrix();
        }
    }
    public void HintClassic()
    {
        CellActionClassic cell_AI = new CellActionClassic();
        if (hintCount >= 0)
        {
            hintCount--;
            numberHint.text = "Hint(" + hintCount.ToString() + ")";
            cell_AI.Hint();
        }
    }

    //Wall----------------
    public void resetMatrixWall()
    {
        BaseWall newB = new BaseWall();
        if (resetCount >= 0)
        {
            resetCount--;
            numberReset.text = "Reset(" + resetCount.ToString() + ")";
            newB.ResetMatrix();
        }
    }
    public void AutoWall()
    {
        BaseWall newB = new BaseWall();
        newB.MakeAuto();

    }
    public void HintWall()
    {
        CellActionWall cell_AI = new CellActionWall();
        if (hintCount >= 0)
        {
            hintCount--;
            numberHint.text = "Hint(" + hintCount.ToString() + ")";
            cell_AI.Hint();
        }
    }
    //Gravity
    public void resetMatrixGravity()
    {
        BaseGravity newB = new BaseGravity();
        if (resetCount >= 0)
        {
            resetCount--;
            numberReset.text = "Reset(" + resetCount.ToString() + ")";
            newB.ResetMatrix();
        }
    }
    public void HintGravity()
    {
        CellActionGravity cell_AI = new CellActionGravity();
        if (hintCount >= 0)
        {
            hintCount--;
            numberHint.text = "Hint(" + hintCount.ToString() + ")";
            cell_AI.Hint();
        }
    }
    public void AutoGravity()
    {
        BaseGravity newB = new BaseGravity();
        newB.MakeAuto();
        
    }
    public void Stop()
    {
        CellActionAI cell_AI = new CellActionAI();
        cell_AI.PauseAutoMatchAll();
        pause.SetActive(false);
        resume.SetActive(true);
    }
    public void Resume()
    {
        CellActionAI cell = new CellActionAI();
        cell.ResumeAutoMatchAll();
        pause.SetActive(true);
        resume.SetActive(false);
    }
    public void Exit()
    {
        Application.Quit();
    }

}
