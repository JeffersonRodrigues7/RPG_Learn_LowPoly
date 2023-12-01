using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{

    Image background;
    TextMeshProUGUI nameText;
    TextMeshProUGUI talkText;

    public float speed = 10f; // Velocidade de transição da barra de fundo
    bool open = false; // Indica se o diálogo está aberto

    void Awake()
    {
        // Obtém referências aos componentes Image e TextMeshProUGUI ao acordar
        background = transform.GetChild(0).GetComponent<Image>();
        nameText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        talkText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Atualiza a barra de fundo com base no estado de abertura
        if (open)
        {
            background.fillAmount = Mathf.Lerp(background.fillAmount, 1, speed * Time.deltaTime);
        }
        else
        {
            background.fillAmount = Mathf.Lerp(background.fillAmount, 0, speed * Time.deltaTime);
        }
    }

    public void SetName(string name)
    {
        // Define o nome do falante na interface do usuário
        nameText.text = name;
    }

    public void Enable()
    {
        // Inicia a animação de abertura da interface do usuário
        background.fillAmount = 0;
        open = true;
    }

    public void Disable()
    {
        // Desativa a interface do usuário e limpa os textos
        open = false;
        nameText.text = "";
        talkText.text = "";
    }

}
