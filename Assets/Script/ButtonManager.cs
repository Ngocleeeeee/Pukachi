using Assets.Script.WallMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
   public void StartScene()
    {
        SceneManager.LoadScene("ModeScene");
    }
    public void OptionScene()
    {
        SceneManager.LoadScene("OptionScene");
    }
    public void NewGame()
    {
        SceneManager.LoadScene("PlayScene");
    }
    public void Classic()
    {
        SceneManager.LoadScene("ClassicScene");
    }
    public void Gravitation()
    {
        SceneManager.LoadScene("Gravity");
    }
    public void Wall1()
    {
        SceneManager.LoadScene("Wall");
    }
    public void AutoMatch()
    {
        SceneManager.LoadScene("AUTOPlayScene");
    }
    public void resetMatrix()
    {
        Base newB = new Base();
        newB.ResetMatrix();
    }
    public void Hint()
    {
        Cell_AI cell_AI = new Cell_AI();
        cell_AI.Hint();
    }
}
