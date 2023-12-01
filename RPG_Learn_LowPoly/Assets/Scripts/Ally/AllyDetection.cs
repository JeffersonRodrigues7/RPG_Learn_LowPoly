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
    // Classe respons�vel pela detec��o de inimigos e controle de ataque do aliado
    public class AllyDetection : MonoBehaviour
    {
        [SerializeField] private float detectionRadius = 10f; // Raio de detec��o do personagem
        [SerializeField] private LayerMask enemyLayer; // Layer dos inimigos

        // Componentes necess�rios
        private AllyMovement allyMovement;
        private AllyAttack allyAttack;

        // Array para armazenar colisores dos inimigos detectados
        private Collider[] enemies;
        private Transform target; // Alvo atual do personagem

        // Inicializa��o
        private void Start()
        {
            // Obt�m os componentes necess�rios do pai
            allyMovement = GetComponent<AllyMovement>();
            allyAttack = GetComponent<AllyAttack>();
        }

        // Chamado a cada frame
        private void Update()
        {
            // Se estiver atacando � dist�ncia, olha para o alvo e retorna
            if (allyAttack.IsRangedAttacking)
            {
                if (target)
                    allyMovement.lookAt(target.position);

                return;
            }

            // Procura por inimigos dentro do raio de detec��o
            enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

            // Se inimigos foram encontrados
            if (enemies.Length > 0)
            {
                // Inicia o ataque � dist�ncia
                allyAttack.IsRangedAttacking = true;
                // Encontra o inimigo mais pr�ximo
                target = FindClosestEnemy(enemies);
                // Inicia a anima��o de ataque no componente de ataque
                allyAttack.startAttackAnimation(target);
            }
        }

        // Encontra o inimigo mais pr�ximo
        private Transform FindClosestEnemy(Collider[] enemies)
        {
            Transform closestEnemy = null;
            float closestDistance = Mathf.Infinity;

            // Itera sobre os inimigos encontrados
            foreach (Collider enemy in enemies)
            {
                float currentDistance = Vector3.Distance(transform.position, enemy.transform.position);

                // Se o inimigo atual estiver mais pr�ximo, atualiza o mais pr�ximo
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    closestEnemy = enemy.transform;
                }
            }

            return closestEnemy;
        }

        // Aumenta o raio de detec��o quando atingido por um proj�til
        public void reactToProjectile()
        {
            detectionRadius = detectionRadius + 5f;
        }

        // Desenha um raio de detec��o no Editor para fins de depura��o
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            // Desenha um raio de detec��o em torno do objeto no Editor
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
