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
        SceneManager.LoadScene("GravitationScene");
    }
    public void Wall()
    {
        SceneManager.LoadScene("WallScene");
    }
}
