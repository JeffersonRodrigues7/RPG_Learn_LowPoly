using UnityEngine;

// Enumera��o que define os estados poss�veis do DialogueSystem
public enum STATE
{
    DISABLED, // Desabilitado, nenhum di�logo est� ocorrendo
    WAITING, // Aguardando a intera��o do jogador (pressionar Enter para avan�ar)
    TYPING // Digitando o texto do di�logo
}

public class DialogueSystem : MonoBehaviour
{

    public DialogueData dialogueData; // Dados do di�logo

    int currentText = 0; // �ndice do texto de di�logo atual
    bool finished = false; // Indica se todos os textos de di�logo foram exibidos

    TypeTextAnimation typeText; // Componente para anima��o de texto
    DialogueUI dialogueUI; // Componente para a interface do usu�rio do di�logo

    public STATE state; // Estado atual do DialogueSystem

    void Awake()
    {
        // Obt�m refer�ncias aos componentes necess�rios ao acordar
        typeText = FindObjectOfType<TypeTextAnimation>();
        dialogueUI = FindObjectOfType<DialogueUI>();

        // Define a fun��o OnTypeFinished como callback para o evento de t�rmino da anima��o de texto
        typeText.TypeFinished = OnTypeFinished;
    }

    void Start()
    {
        state = STATE.DISABLED; // Define o estado inicial como desabilitado
    }

    void Update()
    {
        // Verifica o estado atual e executa a fun��o correspondente
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
        // Configura a interface do usu�rio e inicia a anima��o de texto para o pr�ximo bloco de di�logo
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
        // Fun��o chamada quando a anima��o de texto � conclu�da
        state = STATE.WAITING;
    }

    void Waiting()
    {
        // Aguarda a entrada do jogador (Enter) para avan�ar para o pr�ximo bloco de di�logo ou finalizar o di�logo
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
        // Permite que o jogador pule a anima��o de texto (pressionando Enter)
        if (Input.GetKeyDown(KeyCode.Return))
        {
            typeText.Skip();
            state = STATE.WAITING;
        }
    }
}
