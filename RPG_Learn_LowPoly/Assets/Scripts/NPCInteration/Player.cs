using UnityEngine;

public class Player : MonoBehaviour {

    public Transform npc;

    DialogueSystem dialogueSystem;
   

    private void Awake() {
        dialogueSystem = FindObjectOfType<DialogueSystem>();    

    }

    void Update() {
        float distance = Vector3.Distance(transform.position,npc.position);
        if(distance <2.0f) {
            if(Input.GetKeyDown(KeyCode.E) && dialogueSystem.state == STATE.DISABLED) {
                dialogueSystem.Next();
                //dialogueSystem.Next();
            }
        }

    }

}