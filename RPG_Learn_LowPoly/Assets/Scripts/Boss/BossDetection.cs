using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Boss.Attack;
using RPG.Boss.Movement;
using RPG.Ally.Attack;
using RPG.Ally.Movement;
using RPG.Character.Attack;

namespace RPG.Boss.Detection
{
    public class BossDetection : MonoBehaviour
    {
        [Header("BossData")]
        [SerializeField] public float detectionRadius = 10f; // Raio de detecção do personagem
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
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {

            // Verifica se a distância entre o personagem e o alvo está dentro da distância de ataque.
            if (Vector3.Distance(transform.position, target.transform.position) < attackDistance)
            {
                BossAttack.startAttackAnimation(target); // Inicia a animação de ataque no componente de ataque.
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