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
        [SerializeField] private float detectionRadius = 10f; // Raio de detecção do personagem
        [SerializeField] private float attackDistance = 15f; // Raio de detecção do personagem

        private AllyMovement allyMovement;
        private AllyAttack allyAttack;

        private Transform target; // Alvo atual do personagem

        private void Start()
        {
            allyMovement = GetComponentInParent<AllyMovement>(); // Obtém o componente de movimento do pai
            allyAttack = GetComponentInParent<AllyAttack>();
        }

        private void Update()
        {
            Debug.Log(target?.name);
            if (target != null)
            {
                // Verifica se a distância entre o personagem e o alvo está dentro da distância de ataque.
                if (Vector3.Distance(transform.position, target.transform.position) < attackDistance)
                {
                    allyAttack.startAttackAnimation(target); // Inicia a animação de ataque no componente de ataque.
                }

                allyMovement.lookAt(target.position);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Verifica se o objeto que entrou no campo de detecção tem a tag "Player" (jogador).
            if (other.CompareTag("Enemy"))
            {
                target = other.transform; // Define o jogador como alvo.
            }
        }

        private void OnTriggerExit(Collider other)
        {
                //target = null; // Remove o alvo.
        }

        // Função para desenhar um raio de detecção no Editor para fins de depuração.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius); // Desenha um raio de detecção em torno do objeto no Editor.
        }
    }
}