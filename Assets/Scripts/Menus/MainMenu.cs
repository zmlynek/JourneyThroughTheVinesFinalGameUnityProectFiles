using UnityEngine;
using UnityEngine.SceneManagement;

//This class is used to execute methods for buttons on the main menu
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        //Switches to the next scene in the build
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        //Quit Game
        //Debug.Log("Quit");

        Application.Quit(); //Gives error in unity editor
        
    }
}
