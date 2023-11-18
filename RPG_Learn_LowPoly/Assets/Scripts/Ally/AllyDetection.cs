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
        private SphereCollider detectionCollider; // Collider de detecção

        private Transform target; // Alvo atual do personagem

        private void Start()
        {
            allyMovement = GetComponentInParent<AllyMovement>(); // Obtém o componente de movimento do pai
            allyAttack = GetComponentInParent<AllyAttack>();

            detectionCollider = GetComponent<SphereCollider>(); // Obtém o Collider de esfera associado a este objeto

            if (detectionCollider != null)
            {
                detectionCollider.radius = detectionRadius; // Define o raio do Collider de detecção com base no valor definido
            }
        }

        private void Update()
        {
            //Debug.Log(target?.name);
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
            if(target == null)
            {
                //Debug.Log("Capturando inimigo");
                // Verifica se o objeto que entrou no campo de detecção tem a tag "Player" (jogador).
                if (other.CompareTag("Enemy"))
                {
                    target = other.transform; // Define o jogador como alvo.
                }
            }

        }

        private void OnTriggerExit(Collider other)
        {
            
            //allyMovement.stopLookAt();
        }

        // Função para desenhar um raio de detecção no Editor para fins de depuração.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius); // Desenha um raio de detecção em torno do objeto no Editor.
        }
    }
}