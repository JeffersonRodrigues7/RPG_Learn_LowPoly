using UnityEngine;
using RPG.Health;

public class PlayerDeathRestart : MonoBehaviour
{
    [SerializeField] private GameObject defeatPanel;
    private HealthController playerHealthController;

    private void Awake()
    {
        // Procura automaticamente o HealthController no GameObject do jogador Robert
        playerHealthController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<HealthController>();

        // Certifique-se de que o componente HealthController foi encontrado
        if (playerHealthController == null)
        {
            Debug.LogError("Player HealthController not found on the player GameObject.");
            enabled = false; // Desativa o script se o HealthController não for encontrado
            return;
        }

        // Adiciona um método ao evento de morte no HealthController
        playerHealthController.OnDeath += HandlePlayerDeath;
        Debug.LogError("DAADADADA");
    }

  private void HandlePlayerDeath()
{
    // Procura automaticamente um Canvas na cena
    Canvas canvas = FindObjectOfType<Canvas>();

    if (canvas == null)
    {
        Debug.LogError("Canvas not found in the scene.");
        return;
    }

    // Procura o objeto DefeatPanel dentro do Canvas
    GameObject defeatPanelObject = canvas.transform.Find("DefeatPanel").gameObject;

    if (defeatPanelObject == null)
    {
        Debug.LogError("DefeatPanel not found inside the Canvas.");
        return;
    }

    // Ativa o painel de derrota
    defeatPanelObject.SetActive(true);
    Debug.LogError("CDEs");

    // Certifique-se de remover o ouvinte do evento após lidar com a morte do jogador
    if (playerHealthController != null)
    {
        playerHealthController.OnDeath -= HandlePlayerDeath;
    }
}

}
