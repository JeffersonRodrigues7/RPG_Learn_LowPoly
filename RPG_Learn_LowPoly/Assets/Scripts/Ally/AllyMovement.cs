using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using RPG.Player.Attack;
using System;

namespace RPG.Ally.Movement
{
    // Classe responsável pelo movimento do aliado em relação ao líder
    public class AllyMovement : MonoBehaviour
    {
        // Referência ao líder e distâncias mínimas para andar e correr
        [Header("Basic")]
        [SerializeField] private Transform leader;
        [SerializeField] private float minDistanceToWalk = 5.0f; // Distância para começar a andar em direção ao líder
        [SerializeField] private float minDistanceToRun = 10.0f; // Distância para começar a correr em direção ao líder
        [Space]

        // Velocidades de movimento
        [Header("Movimentacao")]
        [SerializeField] private float walkSpeed = 5.0f; // Velocidade ao andar
        [SerializeField] private float runSpeed = 10.0f; // Velocidade ao correr
        [Space]

        // Variáveis de debug
        [Header("Debug")]
        [SerializeField] private bool isWalking = false;
        [SerializeField] private bool isRunning = false;

        private Animator animator; // Componente Animator
        private NavMeshAgent navMeshAgent; // Componente NavMeshAgent

        private float currentDistanceToLeader;

        private int walkingHash;
        private int runningHash;

        private float currentSpeed;

        private bool isLookingTarget = false;

        private void Awake()
        {
            // Inicializa os componentes e variáveis necessárias quando o objeto é criado
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            // Converte as strings dos nomes das animações em hash para melhor performance
            walkingHash = Animator.StringToHash("isWalking");
            runningHash = Animator.StringToHash("isRunning");
        }

        // Chamado a cada frame
        void Update()
        {
            // Se estiver olhando para um alvo, retorna sem processar o movimento
            if (isLookingTarget) return;
            // Se o líder não estiver definido, retorna sem processar o movimento
            if (leader == null) return;

            // Calcula a distância até o líder
            currentDistanceToLeader = Vector3.Distance(transform.position, leader.position);

            // Decide se deve correr, andar ou parar com base na distância até o líder
            if (currentDistanceToLeader >= minDistanceToRun) // Correndo
            {
                navMeshAgent.SetDestination(leader.position);
                updateMoveAnimation(true, true);
            }
            else if (currentDistanceToLeader >= minDistanceToWalk) // Andando
            {
                navMeshAgent.SetDestination(leader.position);
                updateMoveAnimation(true, false);
            }
            else if (navMeshAgent.hasPath) // Parado
            {
                navMeshAgent.ResetPath();
                updateMoveAnimation(false, false);
            }
        }

        // Atualiza a animação de movimento
        private void updateMoveAnimation(bool _isWalking, bool _isRunning)
        {
            isWalking = _isWalking;
            isRunning = _isRunning;

            // Define a velocidade atual com base se está correndo ou andando
            currentSpeed = isRunning ? runSpeed : walkSpeed;
            navMeshAgent.speed = currentSpeed;

            // Atualiza os parâmetros do Animator
            animator.SetBool(walkingHash, isWalking);
            animator.SetBool(runningHash, isRunning);
        }

        // Faz o aliado olhar na direção de uma posição específica
        public void lookAt(Vector3 position)
        {
            // Impede o movimento durante a rotação
            isLookingTarget = true;
            navMeshAgent.ResetPath();
            updateMoveAnimation(false, false);
            // Faz o aliado olhar para a posição especificada
            transform.LookAt(position);
        }

        // Desativa o estado de olhar para um alvo
        public void desactiveAttack()
        {
            isLookingTarget = false;
        }
    }
}
