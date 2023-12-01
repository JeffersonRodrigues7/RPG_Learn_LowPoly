using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TypeTextAnimation : MonoBehaviour
{

    // Delegado de ação para notificar quando a animação de digitação é concluída
    public Action TypeFinished;

    public float typeDelay = 0.05f; // Atraso entre os caracteres durante a animação de digitação
    public TextMeshProUGUI textObject; // Objeto de texto onde a animação é exibida

    public string fullText; // Texto completo a ser digitado

    Coroutine coroutine; // Referência à coroutine em execução

    void Start()
    {

    }

    // Inicia a animação de digitação
    public void StartTyping()
    {
        coroutine = StartCoroutine(TypeText());
    }

    // Corrotina para realizar a animação de digitação
    IEnumerator TypeText()
    {

        textObject.text = fullText;
        textObject.maxVisibleCharacters = 0;

        // Itera sobre cada caractere do texto, revelando gradualmente durante a animação
        for (int i = 0; i <= textObject.text.Length; i++)
        {
            textObject.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typeDelay);
        }

        // Chama o delegado TypeFinished quando a animação é concluída
        TypeFinished?.Invoke();

    }

    // Função para pular a animação e mostrar imediatamente todo o texto
    public void Skip()
    {

        StopCoroutine(coroutine);
        textObject.maxVisibleCharacters = textObject.text.Length;

    }

}
