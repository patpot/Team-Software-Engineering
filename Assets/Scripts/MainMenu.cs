using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Function designed to transition the scene from the main menu to the game itself upon pressing the 'play' button.
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Function designed to allow game to quit upon pressing the 'quit' button on the main menu.
    public void QuitGame()
    {
        Debug.Log("Game has quit successfully.");
        Application.Quit();
    }
}
