using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TypeTextAnimation : MonoBehaviour
{

    // Delegado de a��o para notificar quando a anima��o de digita��o � conclu�da
    public Action TypeFinished;

    public float typeDelay = 0.05f; // Atraso entre os caracteres durante a anima��o de digita��o
    public TextMeshProUGUI textObject; // Objeto de texto onde a anima��o � exibida

    public string fullText; // Texto completo a ser digitado

    Coroutine coroutine; // Refer�ncia � coroutine em execu��o

    void Start()
    {

    }

    // Inicia a anima��o de digita��o
    public void StartTyping()
    {
        coroutine = StartCoroutine(TypeText());
    }

    // Corrotina para realizar a anima��o de digita��o
    IEnumerator TypeText()
    {

        textObject.text = fullText;
        textObject.maxVisibleCharacters = 0;

        // Itera sobre cada caractere do texto, revelando gradualmente durante a anima��o
        for (int i = 0; i <= textObject.text.Length; i++)
        {
            textObject.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typeDelay);
        }

        // Chama o delegado TypeFinished quando a anima��o � conclu�da
        TypeFinished?.Invoke();

    }

    // Fun��o para pular a anima��o e mostrar imediatamente todo o texto
    public void Skip()
    {

        StopCoroutine(coroutine);
        textObject.maxVisibleCharacters = textObject.text.Length;

    }

}
