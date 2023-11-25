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
        [SerializeField] public float detectionRadius = 10f; // Raio de detec��o do personagem
        [SerializeField] private float attackDistance = 1f; // Dist�ncia de ataque
        [SerializeField] private bool chaseEnemyBehavior = true; // Define se o personagem deve perseguir o inimigo

        private BossMovement BossMovement; // Refer�ncia ao componente de movimento do personagem
        private BossAttack BossAttack; // Refer�ncia ao componente de ataque do personagem

        private Transform target; // Alvo atual do personagem

        // Propriedades para definir os valores do raio de detec��o, dist�ncia de ataque e comportamento de persegui��o.
        public float DetectionRadius { set { detectionRadius = value; } }
        public float AttackDistance { set { attackDistance = value; } }
        public bool ChaseEnemyBehavior { set { chaseEnemyBehavior = value; } }

        private void Start()
        {
            BossMovement = GetComponent<BossMovement>(); // Obt�m o componente de movimento do pai
            BossAttack = GetComponent<BossAttack>(); // Obt�m o componente de ataque do pai
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {

            // Verifica se a dist�ncia entre o personagem e o alvo est� dentro da dist�ncia de ataque.
            if (Vector3.Distance(transform.position, target.transform.position) < attackDistance)
            {
                BossAttack.startAttackAnimation(target); // Inicia a anima��o de ataque no componente de ataque.
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