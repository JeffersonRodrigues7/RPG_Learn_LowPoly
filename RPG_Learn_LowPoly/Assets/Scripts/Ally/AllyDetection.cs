using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Character.Attack;
using RPG.Character.Movement;
using RPG.Ally.Movement;
using RPG.Ally.Attack;

namespace RPG.Ally.Detection
{
    public class AllyDetection : MonoBehaviour
    {
        [SerializeField] private float detectionRadius = 10f; // Raio de detec��o do personagem
        [SerializeField] private float attackDistance = 15f; // Raio de detec��o do personagem

        private AllyMovement allyMovement;
        private AllyAttack allyAttack;

        private Transform target; // Alvo atual do personagem

        private void Start()
        {
            allyMovement = GetComponentInParent<AllyMovement>(); // Obt�m o componente de movimento do pai
            allyAttack = GetComponentInParent<AllyAttack>();
        }

        private void Update()
        {
            Debug.Log(target?.name);
            if (target != null)
            {
                // Verifica se a dist�ncia entre o personagem e o alvo est� dentro da dist�ncia de ataque.
                if (Vector3.Distance(transform.position, target.transform.position) < attackDistance)
                {
                    allyAttack.startAttackAnimation(target); // Inicia a anima��o de ataque no componente de ataque.
                }

                allyMovement.lookAt(target.position);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Verifica se o objeto que entrou no campo de detec��o tem a tag "Player" (jogador).
            if (other.CompareTag("Enemy"))
            {
                target = other.transform; // Define o jogador como alvo.
            }
        }

        private void OnTriggerExit(Collider other)
        {
                //target = null; // Remove o alvo.
        }

        // Fun��o para desenhar um raio de detec��o no Editor para fins de depura��o.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius); // Desenha um raio de detec��o em torno do objeto no Editor.
        }
    }
}