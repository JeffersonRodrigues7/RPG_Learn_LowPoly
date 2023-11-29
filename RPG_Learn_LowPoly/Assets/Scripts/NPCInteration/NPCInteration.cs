using UnityEngine;

public class NPCInteration : MonoBehaviour
{
    GameObject CanvasInventory; 

    public Transform player;

    public DialogueSystem dialogueSystem;

    Vector3 posicaoInicial = Vector3.zero;
    Quaternion rotacaoInicial = Quaternion.identity;

    private void Awake()
    {
        //dialogueSystem = FindObjectOfType<DialogueSystem>();    

    }

    void Update()
    {
        CanvasInventory.SetActive(true);
        if (dialogueSystem.state != STATE.DISABLED)
        {
            player.position = posicaoInicial;
            player.rotation = rotacaoInicial;
            CanvasInventory.SetActive(false);
        }
            
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < 2.0f)
        {
            if (Input.GetKeyDown(KeyCode.E) && dialogueSystem.state == STATE.DISABLED)
            {
                posicaoInicial = player.position;
                rotacaoInicial = player.rotation;
                dialogueSystem.Next();
            }
        }

    }

    private void Start()
    {
        CanvasInventory = GameObject.Find("CanvasInventory");
    }

}