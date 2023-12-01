using UnityEngine;

public class NPCInteration : MonoBehaviour
{
    GameObject CanvasInventory; // Refer�ncia ao objeto CanvasInventory

    public Transform player; // Refer�ncia ao transform do jogador

    public DialogueSystem dialogueSystem; // Refer�ncia ao sistema de di�logo

    Vector3 posicaoInicial = Vector3.zero; // Posi��o inicial do jogador antes de iniciar o di�logo
    Quaternion rotacaoInicial = Quaternion.identity; // Rota��o inicial do jogador antes de iniciar o di�logo

    private void Awake()
    {
        //dialogueSystem = FindObjectOfType<DialogueSystem>(); // Descomentar se o DialogueSystem n�o estiver atribu�do manualmente
    }

    void Update()
    {
        // Ativa o CanvasInventory
        CanvasInventory.SetActive(true);

        // Verifica se o sistema de di�logo est� ativo
        if (dialogueSystem.state != STATE.DISABLED)
        {
            // Reseta a posi��o e rota��o do jogador e desativa o CanvasInventory
            player.position = posicaoInicial;
            player.rotation = rotacaoInicial;
            CanvasInventory.SetActive(false);
        }

        // Calcula a dist�ncia entre o NPC e o jogador
        float distance = Vector3.Distance(transform.position, player.position);

        // Verifica se o jogador est� perto do NPC
        if (distance < 2.0f)
        {
            // Verifica se o jogador pressionou a tecla "E" e o sistema de di�logo est� desativado
            if (Input.GetKeyDown(KeyCode.E) && dialogueSystem.state == STATE.DISABLED)
            {
                // Salva a posi��o e rota��o inicial do jogador, e inicia o pr�ximo bloco de di�logo
                posicaoInicial = player.position;
                rotacaoInicial = player.rotation;
                dialogueSystem.Next();
            }
        }
    }

    private void Start()
    {
        // Encontra o objeto CanvasInventory no in�cio
        CanvasInventory = GameObject.Find("CanvasInventory");
    }
}
