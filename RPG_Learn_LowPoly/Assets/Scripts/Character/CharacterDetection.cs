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
        [SerializeField] private float detectionRadius = 10f; // Raio de detec��o do personagem
        [SerializeField] private float attackDistance = 1f; // Dist�ncia de ataque
        [SerializeField] private bool chaseEnemyBehavior = true; // Define se o personagem deve perseguir o inimigo

        private SphereCollider detectionCollider; // Collider de detec��o

        private CharacterMovement characterMovement; // Refer�ncia ao componente de movimento do personagem
        private CharacterAttack characterAttack; // Refer�ncia ao componente de ataque do personagem

        private Transform target; // Alvo atual do personagem

        // Propriedades para definir os valores do raio de detec��o, dist�ncia de ataque e comportamento de persegui��o.
        public float DetectionRadius { set { detectionRadius = value; } }
        public float AttackDistance { set { attackDistance = value; } }
        public bool ChaseEnemyBehavior { set { chaseEnemyBehavior = value; } }

        private void Start()
        {
            characterMovement = GetComponentInParent<CharacterMovement>(); // Obt�m o componente de movimento do pai
            characterAttack = GetComponentInParent<CharacterAttack>(); // Obt�m o componente de ataque do pai

            detectionCollider = GetComponent<SphereCollider>(); // Obt�m o Collider de esfera associado a este objeto

            if (detectionCollider != null)
            {
                detectionCollider.radius = detectionRadius; // Define o raio do Collider de detec��o com base no valor definido
            }
        }

        private void Update()
        {
            if (target != null)
            {
                // Verifica se a dist�ncia entre o personagem e o alvo est� dentro da dist�ncia de ataque.
                if (Vector3.Distance(transform.position, target.transform.position) < attackDistance)
                {
                    characterAttack.startAttackAnimation(target); // Inicia a anima��o de ataque no componente de ataque.
                }

                if (!chaseEnemyBehavior) //Se for um inimigo que fica parado, ele vai ficar sempre olhando pro alvo
                {
                    characterMovement.lookAt(target.position);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Verifica se o objeto que entrou no campo de detec��o tem a tag "Player" (jogador).
            if (other.CompareTag("Player"))
            {
                target = other.transform; // Define o jogador como alvo.

                if (chaseEnemyBehavior)
                {
                    characterMovement.startChase(target); // Inicia a persegui��o se o comportamento de persegui��o estiver ativado.
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Verifica se o objeto que saiu do campo de detec��o tem a tag "Player" (jogador) e se o comportamento de persegui��o est� ativado.
            if (other.CompareTag("Player") && chaseEnemyBehavior)
            {
                target = null; // Remove o alvo.

                if (chaseEnemyBehavior)
                {
                    characterMovement.stopChase(); // Interrompe a persegui��o se o comportamento de persegui��o estiver ativado.
                }
            }
        }

        // Fun��o para desenhar um raio de detec��o no Editor para fins de depura��o.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius); // Desenha um raio de detec��o em torno do objeto no Editor.
        }
    }
}