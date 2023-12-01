using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Character.Attack;
using RPG.Character.Movement;
using RPG.Ally.Attack;
using RPG.Ally.Movement;

namespace RPG.Character.Detection
{
    public class CharacterDetection : MonoBehaviour
    {
        [Header("CharacterData")]
        [SerializeField] private float detectionRadius = 10f; // Raio de detec��o do personagem
        [SerializeField] private float attackDistance = 1f; // Dist�ncia de ataque
        [SerializeField] private bool chaseEnemyBehavior = true; // Define se o personagem deve perseguir o inimigo

        private CharacterMovement characterMovement; // Refer�ncia ao componente de movimento do personagem
        private CharacterAttack characterAttack; // Refer�ncia ao componente de ataque do personagem

        private Transform target; // Alvo atual do personagem

        // Propriedades para definir os valores do raio de detec��o, dist�ncia de ataque e comportamento de persegui��o.
        public float DetectionRadius { set { detectionRadius = value; } }
        public float AttackDistance { set { attackDistance = value; } }
        public bool ChaseEnemyBehavior { set { chaseEnemyBehavior = value; } }

        private void Start()
        {
            characterMovement = GetComponent<CharacterMovement>(); // Obt�m o componente de movimento do pai
            characterAttack = GetComponent<CharacterAttack>(); // Obt�m o componente de ataque do pai
        }

        private void Update()
        {
            if (target != null)
            {
                float distance = Vector3.Distance(target.position, transform.position);

                if (distance > detectionRadius)
                {
                    target = null; // Remove o alvo.
                    if (!chaseEnemyBehavior) characterMovement.lookAt(target); // Se for arqueiro, ele s� vai parar de olhar
                    if (chaseEnemyBehavior) characterMovement.stopChase(); // Interrompe a persegui��o se o comportamento de persegui��o estiver ativado.
                }

            }

            if (target == null)
            {
                Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius);

                List<Collider> filteredColliders = new List<Collider>();

                foreach (Collider col in enemies)
                {
                    // Verificar se o collider tem a tag "Player" ou "Ally"
                    if (col.CompareTag("Player") || col.CompareTag("Ally"))
                    {
                        filteredColliders.Add(col);
                    }
                }

                if (filteredColliders.Count > 0)
                {
                    target = FindClosestEnemy(filteredColliders.ToArray());

                    if (chaseEnemyBehavior)
                    {
                        characterMovement.startChase(target); // Inicia a persegui��o se o comportamento de persegui��o estiver ativado.
                    }
                }
            }

            if (target != null)
            {
                // Verifica se a dist�ncia entre o personagem e o alvo est� dentro da dist�ncia de ataque.
                if (Vector3.Distance(transform.position, target.transform.position) < attackDistance)
                {
                    characterAttack.startAttackAnimation(target); // Inicia a anima��o de ataque no componente de ataque.
                }

                if (!chaseEnemyBehavior) // Se for um inimigo que fica parado, ele vai ficar sempre olhando pro alvo
                {
                    characterMovement.lookAt(target);
                }
            }
        }

        //Fun��o que retorna o inimgio mais pr�ximo de um array de colliders
        private Transform FindClosestEnemy(Collider[] enemies)
        {
            Transform closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider enemy in enemies)
            {
                float currentDistance = Vector3.Distance(transform.position, enemy.transform.position);

                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closestEnemy = enemy.transform;
                }
            }

            return closestEnemy;
        }

        // Quando o inimigo � atingido ele aumenta seu raio de detec��o
        public void reactToProjectile(Transform _target)
        {
            if (!target) // Se n�o tivemos um target, significa que ele levou um golpe sem estar em batalha, logo precisamos aumentar seu raio de detec��o
            {
                float newDetectionRadius = Vector3.Distance(_target.position, transform.position);
                detectionRadius = newDetectionRadius + 5f;

                if (!chaseEnemyBehavior) // se for arqueiro
                    attackDistance = detectionRadius;
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
