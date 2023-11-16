using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Character.UI
{
    public class CharacterUI : MonoBehaviour
    {
        private Transform camTransform;

        private void Start()
        {
            camTransform = Camera.main.transform;
        }

        private void Update()
        {
            // Faz com que a barra de vida (Canvas) esteja sempre olhando para a câmera.
            transform.LookAt(transform.position + camTransform.rotation * Vector3.forward, camTransform.rotation * Vector3.up);
        }
    }

}

