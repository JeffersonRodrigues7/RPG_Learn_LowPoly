using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject painelMenuPrincipal;
    [SerializeField] private GameObject painelSettings;

    public void Play()
    {
        SceneManager.LoadScene("Gameplay");
    }

    
    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenSettings()
    {
        
        painelSettings.SetActive(true);
    }

    public void CloseSettings()
    {
        painelSettings.SetActive(false);
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
