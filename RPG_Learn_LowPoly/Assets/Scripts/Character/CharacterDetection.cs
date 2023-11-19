using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Character.Attack;
using RPG.Character.Movement;

namespace RPG.Character.Detection
{
    public class CharacterDetection : MonoBehaviour
    {
        [Header("CharacterData")]
        [SerializeField] private float detectionRadius = 10f; // Raio de detecção do personagem
        [SerializeField] private float attackDistance = 1f; // Distância de ataque
        [SerializeField] private bool chaseEnemyBehavior = true; // Define se o personagem deve perseguir o inimigo

        private SphereCollider detectionCollider; // Collider de detecção

        private CharacterMovement characterMovement; // Referência ao componente de movimento do personagem
        private CharacterAttack characterAttack; // Referência ao componente de ataque do personagem

        private Transform target; // Alvo atual do personagem

        // Propriedades para definir os valores do raio de detecção, distância de ataque e comportamento de perseguição.
        public float DetectionRadius { set { detectionRadius = value; } }
        public float AttackDistance { set { attackDistance = value; } }
        public bool ChaseEnemyBehavior { set { chaseEnemyBehavior = value; } }

        private void Start()
        {
            characterMovement = GetComponentInParent<CharacterMovement>(); // Obtém o componente de movimento do pai
            characterAttack = GetComponentInParent<CharacterAttack>(); // Obtém o componente de ataque do pai

            detectionCollider = GetComponent<SphereCollider>(); // Obtém o Collider de esfera associado a este objeto

            if (detectionCollider != null)
            {
                detectionCollider.radius = detectionRadius; // Define o raio do Collider de detecção com base no valor definido
            }
        }

        private void Update()
        {
            if (target != null)
            {
                // Verifica se a distância entre o personagem e o alvo está dentro da distância de ataque.
                if (Vector3.Distance(transform.position, target.transform.position) < attackDistance)
                {
                    characterAttack.startAttackAnimation(target); // Inicia a animação de ataque no componente de ataque.
                }

                if (!chaseEnemyBehavior) //Se for um inimigo que fica parado, ele vai ficar sempre olhando pro alvo
                {
                    characterMovement.lookAt(target.position);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Verifica se o objeto que entrou no campo de detecção tem a tag "Player" (jogador).
            if (other.CompareTag("Player") || other.CompareTag("Ally"))
            {
                target = other.transform; // Define o jogador como alvo.

                if (chaseEnemyBehavior)
                {
                    characterMovement.startChase(target); // Inicia a perseguição se o comportamento de perseguição estiver ativado.
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Verifica se o objeto que saiu do campo de detecção tem a tag "Player" (jogador) e se o comportamento de perseguição está ativado.
            if (target && chaseEnemyBehavior)
            {
                target = null; // Remove o alvo.

                if (chaseEnemyBehavior)
                {
                    characterMovement.stopChase(); // Interrompe a perseguição se o comportamento de perseguição estiver ativado.
                }
            }
        }

        //Quando o inimigo é atingido ele aumenta seu raio de detecção
        public void reactToProjectile(Transform _target)
        {
            if (!target) //Se não tivemos um target, significa que ele levou um golpe sem estar em batalha, logo precisamos aumentar seu raio de detecção
            {
                float newDetectionRadius = Vector3.Distance(_target.position, transform.position);
                detectionRadius = newDetectionRadius + 5f;
                detectionCollider.radius = detectionRadius;

                if (!chaseEnemyBehavior)//se for arqueiro
                    attackDistance = detectionRadius;
            }
        }

        // Função para desenhar um raio de detecção no Editor para fins de depuração.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius); // Desenha um raio de detecção em torno do objeto no Editor.
        }
    }
}