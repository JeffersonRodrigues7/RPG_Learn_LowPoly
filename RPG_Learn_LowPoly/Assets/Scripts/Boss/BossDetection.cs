using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Boss.Attack;
using RPG.Boss.Movement;
using RPG.Ally.Attack;
using RPG.Ally.Movement;

namespace RPG.Boss.Detection
{
    public class BossDetection : MonoBehaviour
    {
        [Header("BossData")]
        [SerializeField] private float detectionRadius = 10f; // Raio de detecção do personagem
        [SerializeField] private float attackDistance = 1f; // Distância de ataque
        [SerializeField] private bool chaseEnemyBehavior = true; // Define se o personagem deve perseguir o inimigo

        private BossMovement BossMovement; // Referência ao componente de movimento do personagem
        private BossAttack BossAttack; // Referência ao componente de ataque do personagem

        private Transform target; // Alvo atual do personagem

        // Propriedades para definir os valores do raio de detecção, distância de ataque e comportamento de perseguição.
        public float DetectionRadius { set { detectionRadius = value; } }
        public float AttackDistance { set { attackDistance = value; } }
        public bool ChaseEnemyBehavior { set { chaseEnemyBehavior = value; } }

        private void Start()
        {
            BossMovement = GetComponent<BossMovement>(); // Obtém o componente de movimento do pai
            BossAttack = GetComponent<BossAttack>(); // Obtém o componente de ataque do pai
        }

        private void Update()
        {
            if(target != null)
            {
                float distance = Vector3.Distance(target.position, transform.position);

                if (distance > detectionRadius)
                {
                    target = null; // Remove o alvo.
                    if (!chaseEnemyBehavior) BossMovement.lookAt(target); //Se for arqueiro, ele só vai parar de olhar
                    if (chaseEnemyBehavior) BossMovement.stopChase(); // Interrompe a perseguição se o comportamento de perseguição estiver ativado.
                }
                
            }

            if(target == null)
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
                        BossMovement.startChase(target); // Inicia a perseguição se o comportamento de perseguição estiver ativado.
                    }
                }
            }

            if (target != null)
            {
                // Verifica se a distância entre o personagem e o alvo está dentro da distância de ataque.
                if (Vector3.Distance(transform.position, target.transform.position) < attackDistance)
                {
                    BossAttack.startAttackAnimation(target); // Inicia a animação de ataque no componente de ataque.
                }

                if (!chaseEnemyBehavior) //Se for um inimigo que fica parado, ele vai ficar sempre olhando pro alvo
                {
                    BossMovement.lookAt(target);
                }
            }
        }

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

        //Quando o inimigo é atingido ele aumenta seu raio de detecção
        public void reactToProjectile(Transform _target)
        {
            if (!target) //Se não tivemos um target, significa que ele levou um golpe sem estar em batalha, logo precisamos aumentar seu raio de detecção
            {
                float newDetectionRadius = Vector3.Distance(_target.position, transform.position);
                detectionRadius = newDetectionRadius + 5f;

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