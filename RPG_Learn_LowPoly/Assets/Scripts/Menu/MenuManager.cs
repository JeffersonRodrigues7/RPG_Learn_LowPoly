using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Text feedbackText;

    public void Play()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void Settings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }
    public void Exit()
    {
        Debug.Log("Sair");
        Application.Quit();
    }
}
