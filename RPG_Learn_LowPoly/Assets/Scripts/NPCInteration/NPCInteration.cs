using UnityEngine;

public class NPCInteration : MonoBehaviour
{
    GameObject CanvasInventory; // Referência ao objeto CanvasInventory

    public Transform player; // Referência ao transform do jogador

    public DialogueSystem dialogueSystem; // Referência ao sistema de diálogo

    Vector3 posicaoInicial = Vector3.zero; // Posição inicial do jogador antes de iniciar o diálogo
    Quaternion rotacaoInicial = Quaternion.identity; // Rotação inicial do jogador antes de iniciar o diálogo

    private void Awake()
    {
        //dialogueSystem = FindObjectOfType<DialogueSystem>(); // Descomentar se o DialogueSystem não estiver atribuído manualmente
    }

    void Update()
    {
        // Ativa o CanvasInventory
        CanvasInventory.SetActive(true);

        // Verifica se o sistema de diálogo está ativo
        if (dialogueSystem.state != STATE.DISABLED)
        {
            // Reseta a posição e rotação do jogador e desativa o CanvasInventory
            player.position = posicaoInicial;
            player.rotation = rotacaoInicial;
            CanvasInventory.SetActive(false);
        }

        // Calcula a distância entre o NPC e o jogador
        float distance = Vector3.Distance(transform.position, player.position);

        // Verifica se o jogador está perto do NPC
        if (distance < 2.0f)
        {
            // Verifica se o jogador pressionou a tecla "E" e o sistema de diálogo está desativado
            if (Input.GetKeyDown(KeyCode.E) && dialogueSystem.state == STATE.DISABLED)
            {
                // Salva a posição e rotação inicial do jogador, e inicia o próximo bloco de diálogo
                posicaoInicial = player.position;
                rotacaoInicial = player.rotation;
                dialogueSystem.Next();
            }
        }
    }

    private void Start()
    {
        // Encontra o objeto CanvasInventory no início
        CanvasInventory = GameObject.Find("CanvasInventory");
    }
}
