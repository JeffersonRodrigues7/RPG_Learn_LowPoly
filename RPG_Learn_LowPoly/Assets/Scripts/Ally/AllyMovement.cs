using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using RPG.Player.Attack;
using System;

namespace RPG.Ally.Movement
{
    // Classe respons�vel pelo movimento do aliado em rela��o ao l�der
    public class AllyMovement : MonoBehaviour
    {
        // Refer�ncia ao l�der e dist�ncias m�nimas para andar e correr
        [Header("Basic")]
        [SerializeField] private Transform leader;
        [SerializeField] private float minDistanceToWalk = 5.0f; // Dist�ncia para come�ar a andar em dire��o ao l�der
        [SerializeField] private float minDistanceToRun = 10.0f; // Dist�ncia para come�ar a correr em dire��o ao l�der
        [Space]

        // Velocidades de movimento
        [Header("Movimentacao")]
        [SerializeField] private float walkSpeed = 5.0f; // Velocidade ao andar
        [SerializeField] private float runSpeed = 10.0f; // Velocidade ao correr
        [Space]

        // Vari�veis de debug
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
            // Inicializa os componentes e vari�veis necess�rias quando o objeto � criado
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            // Converte as strings dos nomes das anima��es em hash para melhor performance
            walkingHash = Animator.StringToHash("isWalking");
            runningHash = Animator.StringToHash("isRunning");
        }

        // Chamado a cada frame
        void Update()
        {
            // Se estiver olhando para um alvo, retorna sem processar o movimento
            if (isLookingTarget) return;
            // Se o l�der n�o estiver definido, retorna sem processar o movimento
            if (leader == null) return;

            // Calcula a dist�ncia at� o l�der
            currentDistanceToLeader = Vector3.Distance(transform.position, leader.position);

            // Decide se deve correr, andar ou parar com base na dist�ncia at� o l�der
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

        // Atualiza a anima��o de movimento
        private void updateMoveAnimation(bool _isWalking, bool _isRunning)
        {
            isWalking = _isWalking;
            isRunning = _isRunning;

            // Define a velocidade atual com base se est� correndo ou andando
            currentSpeed = isRunning ? runSpeed : walkSpeed;
            navMeshAgent.speed = currentSpeed;

            // Atualiza os par�metros do Animator
            animator.SetBool(walkingHash, isWalking);
            animator.SetBool(runningHash, isRunning);
        }

        // Faz o aliado olhar na dire��o de uma posi��o espec�fica
        public void lookAt(Vector3 position)
        {
            // Impede o movimento durante a rota��o
            isLookingTarget = true;
            navMeshAgent.ResetPath();
            updateMoveAnimation(false, false);
            // Faz o aliado olhar para a posi��o especificada
            transform.LookAt(position);
        }

        // Desativa o estado de olhar para um alvo
        public void desactiveAttack()
        {
            isLookingTarget = false;
        }
    }
}
