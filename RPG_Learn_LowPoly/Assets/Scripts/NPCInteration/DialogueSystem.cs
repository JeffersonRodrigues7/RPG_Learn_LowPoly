using UnityEngine;

// Enumeração que define os estados possíveis do DialogueSystem
public enum STATE
{
    DISABLED, // Desabilitado, nenhum diálogo está ocorrendo
    WAITING, // Aguardando a interação do jogador (pressionar Enter para avançar)
    TYPING // Digitando o texto do diálogo
}

public class DialogueSystem : MonoBehaviour
{

    public DialogueData dialogueData; // Dados do diálogo

    int currentText = 0; // Índice do texto de diálogo atual
    bool finished = false; // Indica se todos os textos de diálogo foram exibidos

    TypeTextAnimation typeText; // Componente para animação de texto
    DialogueUI dialogueUI; // Componente para a interface do usuário do diálogo

    public STATE state; // Estado atual do DialogueSystem

    void Awake()
    {
        // Obtém referências aos componentes necessários ao acordar
        typeText = FindObjectOfType<TypeTextAnimation>();
        dialogueUI = FindObjectOfType<DialogueUI>();

        // Define a função OnTypeFinished como callback para o evento de término da animação de texto
        typeText.TypeFinished = OnTypeFinished;
    }

    void Start()
    {
        state = STATE.DISABLED; // Define o estado inicial como desabilitado
    }

    void Update()
    {
        // Verifica o estado atual e executa a função correspondente
        if (state == STATE.DISABLED) return;

        switch (state)
        {
            case STATE.WAITING:
                Waiting();
                break;
            case STATE.TYPING:
                Typing();
                break;
        }
    }

    public void Next()
    {
        // Configura a interface do usuário e inicia a animação de texto para o próximo bloco de diálogo
        if (currentText == 0)
        {
            dialogueUI.Enable();
        }

        dialogueUI.SetName(dialogueData.talkScript[currentText].name);

        typeText.fullText = dialogueData.talkScript[currentText++].text;

        if (currentText == dialogueData.talkScript.Count) finished = true;

        typeText.StartTyping();
        state = STATE.TYPING;
    }

    void OnTypeFinished()
    {
        // Função chamada quando a animação de texto é concluída
        state = STATE.WAITING;
    }

    void Waiting()
    {
        // Aguarda a entrada do jogador (Enter) para avançar para o próximo bloco de diálogo ou finalizar o diálogo
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!finished)
            {
                Next();
            }
            else
            {
                dialogueUI.Disable();
                state = STATE.DISABLED;
                currentText = 0;
                finished = false;
            }
        }
    }

    void Typing()
    {
        // Permite que o jogador pule a animação de texto (pressionando Enter)
        if (Input.GetKeyDown(KeyCode.Return))
        {
            typeText.Skip();
            state = STATE.WAITING;
        }
    }
}
