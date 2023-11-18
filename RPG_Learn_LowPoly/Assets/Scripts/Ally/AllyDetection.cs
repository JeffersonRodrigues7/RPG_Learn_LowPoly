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
        [SerializeField] private LayerMask enemyLayer; // Layer dos inimigos

        private AllyMovement allyMovement;
        private AllyAttack allyAttack;

        private Collider[] enemies;
        private Transform target; // Alvo atual do personagem



        private void Start()
        {
            allyMovement = GetComponentInParent<AllyMovement>(); // Obtém o componente de movimento do pai
            allyAttack = GetComponentInParent<AllyAttack>();
        }

        private void Update()
        {
            if (allyAttack.IsRangedAttacking)
            {
                if(target)
                    allyMovement.lookAt(target.position);

                return;
            }

            enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

            if (enemies.Length > 0)
            {
                allyAttack.IsRangedAttacking = true;
                target = FindClosestEnemy(enemies);
                allyAttack.startAttackAnimation(target); // Inicia a animação de ataque no componente de ataque.
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

        // Função para desenhar um raio de detecção no Editor para fins de depuração.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius); // Desenha um raio de detecção em torno do objeto no Editor.
        }
    }
}