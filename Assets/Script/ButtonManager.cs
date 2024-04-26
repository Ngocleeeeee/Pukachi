using Assets.Script.AutoMatch;
using Assets.Script.Classic;
using Assets.Script.Gravity;
using Assets.Script.WallMode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WALL_MODE = Assets.Script.WallMode;

public class ButtonManager : MonoBehaviour
{
    public CellActionAI cell_AI;
    public CountdownTimer countdown_Timer;
    public GameObject pause, panelSetting; // Reference to the first button GameObject
    public GameObject resume;
    public Text numberHint, numberReset;
    public int  hintCount = 5, resetCount = 2;
    

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
        panelSetting.SetActive(true);
    }
    public void _OptionsClose()
    {
        panelSetting.SetActive(false);
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

    //Play -------------------------------
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

    //classic-------------------------------
    public void resetMatrixClassic()
    {
        BaseClassic newB = new BaseClassic();
        if (resetCount > 0)
        {
            resetCount--;
            numberReset.text = "Reset(" + resetCount.ToString() + ")";
            newB.ResetMatrix();
        }
    }
    public void HintClassic()
    {
        CellActionClassic cell_AI = new CellActionClassic();
        if (hintCount > 0)
        {
            hintCount--;
            numberHint.text = "Hint(" + hintCount.ToString() + ")";
            cell_AI.Hint();
        }
    }

    //Wall--------------------------------
    public void resetMatrixWall()
    {
        BaseWall newB = new BaseWall();
        if (BaseWall.ResetCnt > 0)
        {
            newB.ResetMatrix();
            BaseWall.ResetCnt--;
            BaseWall.SetButton(0, "Reset (" + BaseWall.ResetCnt.ToString() + ")");
        }
    }
    public void AutoWall()
    {

    }
    public void HintWall()
    {
        BaseWall newB = new BaseWall();
        if (BaseWall.HintCnt > 0)
        {
            newB.showHint();
                BaseWall.HintCnt--;
                BaseWall.SetButton(1, "Hint (" + BaseWall.HintCnt.ToString() + ")");
        }
    }
    //Gravity---------------------------------
    public void resetMatrixGravity()
    {
        BaseGravity newB = new BaseGravity();
        if (resetCount > 0)
        {
            resetCount--;
            numberReset.text = "Reset(" + resetCount.ToString() + ")";
            newB.ResetMatrix();
        }
    }
    public void HintGravity()
    {
        CellActionGravity cell_AI = new CellActionGravity();
        if (hintCount > 0)
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
    //Others----------------------------------
    public void Stop()
    {
        // Kiểm tra xem cell_AI đã được gán hay chưa
        if (cell_AI != null)
        {
            cell_AI.PauseAutoMatching();
            countdown_Timer.SetState(false);
            //Debug.Log(cell_AI.isAutoMatching.ToString());
            pause.SetActive(false);
            resume.SetActive(true);
        }
    }

    public void Resume()
    {
        // Kiểm tra xem cell_AI đã được gán hay chưa
        if (cell_AI != null)
        {
            countdown_Timer.SetState(true);
            cell_AI.ResumeAutoMatching();
            pause.SetActive(true);
            resume.SetActive(false);
        }
    }

    public void SetCellAI(CellActionAI cellAI)
    {
        cell_AI = cellAI;
    }

    public void Exit()
    {
        Application.Quit();
    }

}
