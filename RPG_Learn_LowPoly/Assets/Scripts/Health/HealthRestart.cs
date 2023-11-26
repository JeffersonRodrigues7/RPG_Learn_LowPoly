using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthRestart : MonoBehaviour
{
    // Método chamado quando o botão de restart é clicado
    public void RestartGame()
    {
        // Obtém o índice da cena atual
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Recarrega a cena atual
        SceneManager.LoadScene(currentSceneIndex);
    }
}
