using RPG.Boss.Detection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Boss.Movement
{

    public enum BossState // Estados possiveis
    {
        Patrolling,
        Chasing,
        MovingToLastKnownEnemyPosition,
        Searching,
        ReturningToOriginalPosition,
        Teleporting
    }

    public class BossMovement : MonoBehaviour
    {
        [Header("BossData")]
        [SerializeField] private float walkSpeed = 5; // Velocidade de caminhada do personagem
        [SerializeField] private float chaseSpeed = 10; // Velocidade de persegui��o do personagem
        [SerializeField] private float cooldownTimeAfterChase = 2f; // Tempo de espera ap�s a persegui��o
        [SerializeField] private float minDistanceToTarget = 2f;//Vai parar de seguir qnd chegar a essa distancia

        [Header("Effects")]
        private float arrivalDistance = 0.1f; // Dist�ncia para considerar que o personagem chegou � posi��o final
        [SerializeField] private Transform[] patrolPoints; // Pontos de patrulha do personagem
        [SerializeField] private GameObject TeleportEffect; // Pontos de patrulha do personagem
        [SerializeField] private GameObject Thrall; // Pontos de patrulha do personagem
        [SerializeField] private GameObject ThrallConjureEffect; // Pontos de patrulha do personagem


        public BossState currentBossState;

        private Animator animator; 
        private NavMeshAgent navMeshAgent;
        private Transform target; // Alvo atual do personagem
        private Coroutine goBackToOriginalPositionVariable; // Corrotina para retornar � posi��o original
        private Vector3 originalPosition; // Posi��o original do personagem

        private int isWalkingHash; // Hash da String que se refere � anima��o de Walk
        private bool isWalking = false;

        private int initialPatrolPoint = 0; // Ponto inicial de patrulha
        private int currentPatrolPoint = 0;
        private int patrolPointsLength = 0;

        private GameObject player;
        private GameObject teleportPoints;
        private bool isTeleporting = false;
        private Vector3 positionBeforeTeleport;

        private BossDetection bossDetection;

        public float WalkSpeed { set { walkSpeed = value; } }
        public float ChaseSpeed { set { chaseSpeed = value; } }
        public float CooldownTimeAfterChase { set { cooldownTimeAfterChase = value; } }
        public float ArrivalDistance { set { arrivalDistance = value; } }
        public Transform[] PatrolPoints { set { patrolPoints = value; } }

        private void Awake()
        {
            bossDetection = GetComponent<BossDetection>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {

            isWalkingHash = Animator.StringToHash("isWalking"); // Obt�m o hash da string da anima��o de caminhada
            originalPosition = transform.position; // Registra a posi��o original do personagem
            currentBossState = BossState.Patrolling; // Define o estado inicial como "Patrolling"
            navMeshAgent.speed = walkSpeed;

            player = GameObject.FindGameObjectWithTag("Player");
            teleportPoints = GameObject.Find("BossTeleportPoints");

            patrolPoints = teleportPoints.GetComponentsInChildren<Transform>();

            if (patrolPoints != null) patrolPointsLength = patrolPoints.Length; // Registra o n�mero de pontos de patrulha se existirem.
        }

        private void Update()
        {
            switch (currentBossState)
            {
                case BossState.Patrolling: // Personagem parado na posi��o original.
                    if (target != null) return;

                    if (patrolPointsLength > 1 && !isWalking) // Se existir mais de um ponto de patrulha e o personagem n�o estiver caminhando
                    {
                        setWalkingAnimation(true);
                    }
                    doPatrolling(); // Executa a patrulha
                    break;

                case BossState.Chasing: // Perseguindo algum alvo
                    if (target == null) stopChase();
                    else
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, target.position);
                        if (distanceToTarget > minDistanceToTarget)
                            navMeshAgent.SetDestination(target.position);
                        else
                        { //Se estiver muito proximo do alvo vai parar de perseguir e começar a olhar na direção dele
                            lookToPlayer();
                        }
                    }

                    break;

                case BossState.MovingToLastKnownEnemyPosition: // Movendo-se para a �ltima posi��o conhecida do inimigo.
                    if (navMeshAgent.remainingDistance <= arrivalDistance)
                    {
                        goBackToOriginalPositionVariable = StartCoroutine(goBackToOriginalPosition()); // Inicia a corrotina para retornar � posi��o original
                        currentBossState = BossState.Searching; // Muda para o estado de busca
                    }
                    break;

                case BossState.Searching: // Personagem est� parado atento ao inimigo que saiu de seu alcance de persegui��o
                    
                    break;

                case BossState.ReturningToOriginalPosition: // Retornando � posi��o original.
                    if (navMeshAgent.remainingDistance <= arrivalDistance)
                    {
                        navMeshAgent.ResetPath(); // Limpa o caminho
                        setWalkingAnimation(false); 
                        currentBossState = BossState.Patrolling; // Muda para o estado de patrulha

                        currentPatrolPoint = initialPatrolPoint;
                        navMeshAgent.SetDestination(patrolPoints[currentPatrolPoint].position); // Enviando o personagem para o primeiro ponto de patrulha
                    }
                    break;

                case BossState.Teleporting: // Personagem est� parado atento ao inimigo que saiu de seu alcance de persegui��o
                    navMeshAgent.ResetPath();
                    lookToPlayer();
                    setWalkingAnimation(false);
                    if (Vector3.Distance(transform.position, player.transform.position) < bossDetection.detectionRadius && !isTeleporting)
                    {
                        animator.SetBool("Conjuring", false);
                        positionBeforeTeleport = transform.position;           
                        isTeleporting = true;
                        animator.SetTrigger("TriggerTeleport");
                    }
                    else
                    {
                        animator.SetBool("Conjuring", true);
                    }
                    break;

                default:
                    break;
            }
        }

        //função chamada através da animação de teleport
        public void doTeleport() 
        {
            Instantiate(TeleportEffect, positionBeforeTeleport, Quaternion.identity);
            Instantiate(TeleportEffect, positionBeforeTeleport, Quaternion.identity);
            Instantiate(TeleportEffect, positionBeforeTeleport, Quaternion.identity);
            Vector3 nextPosition = FindOtherPoint();
            transform.position = nextPosition;
            isTeleporting = false;
        }

        //ConjuraMinion, chamada através de animação
        void conjureThrall()
        {
            // Gera pos e rotacao aleatoria
            float angle = Random.Range(0f, 2f * Mathf.PI); 
            float distance = Random.Range(0f, 10f); 

            // Calcula a distancia
            float offsetX = distance * Mathf.Cos(angle);
            float offsetZ = distance * Mathf.Sin(angle);

            // nova pos
            Vector3 pos = player.transform.position + new Vector3(offsetX, 0f, offsetZ);

            Instantiate(ThrallConjureEffect, pos + new Vector3(0,1.5f,0), Quaternion.identity);
            GameObject Tharall = Instantiate(Thrall, pos, Quaternion.identity);
            Destroy(Tharall, 20);
        }

        private void lookToPlayer()
        {
            if(target != null)
            {
                Vector3 targetPosition = target.position;
                targetPosition.y = transform.position.y;
                transform.LookAt(targetPosition);
            }
        }

        //Encontra ponto mais distante do player
        public Vector3 FindOtherPoint()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                // Handle the case where the Transform array is empty
                return Vector3.zero;
            }

            int randomIndex = Random.Range(0, patrolPoints.Length);
            return patrolPoints[randomIndex].position;
        }

        // Fun��o que realizar a patrulha do personagem
        private void doPatrolling()
        {
            if (patrolPointsLength > 0) //Se houve algum ponto de patrulha
            {
                if (navMeshAgent.remainingDistance <= arrivalDistance) //Se personagem chegou a algum ponto de patrulha
                {
                    currentPatrolPoint++;
                    if (currentPatrolPoint >= patrolPointsLength) //Se estamos no ultimo ponto, voltamos pro inicio
                    {
                        currentPatrolPoint = 0;
                    }
                    navMeshAgent.SetDestination(patrolPoints[currentPatrolPoint].position);
                }
            }
            else
            {
                setWalkingAnimation(false); // Se n�o houver pontos de patrulha, desativa a anima��o de caminhada
            }
        }

        private void setWalkingAnimation(bool _isWalking)
        {
            if (isWalking != _isWalking)
            {
                isWalking = _isWalking;
                animator.SetBool(isWalkingHash, isWalking); // Define o par�metro da anima��o de caminhada
            }
        }

        // Inicia a persegui��o.
        public void startChase(Transform transform)
        {
            if (target) return;

            //Debug.Log("Starting Chase");
            currentBossState = BossState.Chasing;
            navMeshAgent.speed = chaseSpeed;

            // Se estiver no meio de uma corrotina de retorno � posi��o original, interrompa-a.
            if (goBackToOriginalPositionVariable != null)
            {
                StopCoroutine(goBackToOriginalPositionVariable);
                goBackToOriginalPositionVariable = null;
            }

            target = transform; // Define o alvo da persegui��o
            setWalkingAnimation(true); 
        }

        // Interrompe a persegui��o e muda para o estado de movimento para a �ltima posi��o conhecida do inimigo.
        public void stopChase()
        {

            target = null;
            navMeshAgent.speed = walkSpeed;
            currentBossState = BossState.MovingToLastKnownEnemyPosition;
        }

        // Corrotina para retornar � posi��o original ap�s a persegui��o.
        private IEnumerator goBackToOriginalPosition()
        {

            navMeshAgent.ResetPath(); // Limpa o caminho
            setWalkingAnimation(false); 

            yield return new WaitForSeconds(cooldownTimeAfterChase);

            // Ap�s o tempo de espera, entra no estado de retornar � posi��o original
            currentBossState = BossState.ReturningToOriginalPosition;
            navMeshAgent.SetDestination(originalPosition);
            setWalkingAnimation(true);
        }

        public void lookAt(Transform _target)
        {
            target = _target;

            if(target != null)
            {
                lookToPlayer();
            }

        }
    }
}