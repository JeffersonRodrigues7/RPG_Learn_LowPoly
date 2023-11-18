using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using RPG.Player.Attack;
using System;



namespace RPG.Ally.Movement
{
    public class AllyMovement : MonoBehaviour
    {
        [Header("Basic")]
        [SerializeField] private Transform leader;
        [SerializeField] private float minDistanceToWalk  = 5.0f; //Distância pro leader
        [SerializeField] private float minDistanceToRun = 10.0f; //Distância pro leader
        [Space]

        [Header("Movimentacao")]
        [SerializeField] private float walkSpeed = 5.0f; //Velocidade do jogador ao andar
        [SerializeField] private float runSpeed = 10.0f; //Velocidade do jogador ao correr
        [Space]

        [Header("Debug")]
        [SerializeField] private bool isWalking = false;
        [SerializeField] private bool isRunning = false;

        private Animator animator; //Componente animator
        private NavMeshAgent navMeshAgent; //Componente navMeshAgent

        private float currentDistanceToLeader;

        private int walkingHash;
        private int runningHash;

        private float currentSpeed;

        private bool isLookingTarget = false;

        private void Awake()
        {
            // Inicializa os componentes e vari?veis necess?rias quando o objeto ? criado
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            walkingHash = Animator.StringToHash("isWalking");
            runningHash = Animator.StringToHash("isRunning");
        }

        // Update is called once per frame
        void Update()
        {
            if (isLookingTarget) return;

            currentDistanceToLeader = Vector3.Distance(transform.position, leader.position); 

            if (currentDistanceToLeader >= minDistanceToRun) //Correndo
            {
                navMeshAgent.SetDestination(leader.position);
                updateMoveAnimation(true, true);
            }
            else if (currentDistanceToLeader >= minDistanceToWalk) //Andando
            {
                navMeshAgent.SetDestination(leader.position);
                updateMoveAnimation(true, false);
            }
            else if(navMeshAgent.hasPath) //Parado
            {
                navMeshAgent.ResetPath();
                updateMoveAnimation(false, false);
            }
        }

        //Atualiza??o anima??o de mvimento
        private void updateMoveAnimation(bool _isWalking, bool _isRunning)
        {
            isWalking = _isWalking;
            isRunning = _isRunning;

            currentSpeed = isRunning ? runSpeed : walkSpeed;
            navMeshAgent.speed = currentSpeed;

            animator.SetBool(walkingHash, isWalking);//Para que ele chegue na anima??o de correr, ? necess?rio que esteja andando
            animator.SetBool(runningHash, isRunning);
        }

        public void lookAt(Vector3 position)
        {
            isLookingTarget = true;
            navMeshAgent.ResetPath();
            updateMoveAnimation(false, false);
            transform.LookAt(position);
        }

        public void desactiveAttack()
        {
            isLookingTarget = false;
        }
    }

}
