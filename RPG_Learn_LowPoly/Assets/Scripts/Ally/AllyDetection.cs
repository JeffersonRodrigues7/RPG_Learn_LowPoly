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
    // Classe responsável pela detecção de inimigos e controle de ataque do aliado
    public class AllyDetection : MonoBehaviour
    {
        [SerializeField] private float detectionRadius = 10f; // Raio de detecção do personagem
        [SerializeField] private LayerMask enemyLayer; // Layer dos inimigos

        // Componentes necessários
        private AllyMovement allyMovement;
        private AllyAttack allyAttack;

        // Array para armazenar colisores dos inimigos detectados
        private Collider[] enemies;
        private Transform target; // Alvo atual do personagem

        // Inicialização
        private void Start()
        {
            // Obtém os componentes necessários do pai
            allyMovement = GetComponent<AllyMovement>();
            allyAttack = GetComponent<AllyAttack>();
        }

        // Chamado a cada frame
        private void Update()
        {
            // Se estiver atacando à distância, olha para o alvo e retorna
            if (allyAttack.IsRangedAttacking)
            {
                if (target)
                    allyMovement.lookAt(target.position);

                return;
            }

            // Procura por inimigos dentro do raio de detecção
            enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

            // Se inimigos foram encontrados
            if (enemies.Length > 0)
            {
                // Inicia o ataque à distância
                allyAttack.IsRangedAttacking = true;
                // Encontra o inimigo mais próximo
                target = FindClosestEnemy(enemies);
                // Inicia a animação de ataque no componente de ataque
                allyAttack.startAttackAnimation(target);
            }
        }

        // Encontra o inimigo mais próximo
        private Transform FindClosestEnemy(Collider[] enemies)
        {
            Transform closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            // Itera sobre os inimigos encontrados
            foreach (Collider enemy in enemies)
            {
                float currentDistance = Vector3.Distance(transform.position, enemy.transform.position);

                // Se o inimigo atual estiver mais próximo, atualiza o mais próximo
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closestEnemy = enemy.transform;
                }
            }

            return closestEnemy;
        }

        // Aumenta o raio de detecção quando atingido por um projétil
        public void reactToProjectile()
        {
            detectionRadius = detectionRadius + 5f;
        }

        // Desenha um raio de detecção no Editor para fins de depuração
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            // Desenha um raio de detecção em torno do objeto no Editor
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
