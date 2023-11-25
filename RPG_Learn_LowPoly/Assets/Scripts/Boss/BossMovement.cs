using RPG.Boss.Detection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Boss.Movement
{

    public enum BossState // Estados possiveis
    {
        Chasing,
        Teleporting
    }

    public class BossMovement : MonoBehaviour
    {
        [Header("BossData")]
        [SerializeField] private float chaseSpeed = 10; // Velocidade de persegui��o do personagem
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

        private int isWalkingHash; // Hash da String que se refere � anima��o de Walk
        private bool isWalking = false;


        private GameObject player;
        private GameObject teleportPoints;
        private bool isTeleporting = false;
        private Vector3 positionBeforeTeleport;

        private BossDetection bossDetection;

        public float ChaseSpeed { set { chaseSpeed = value; } }
        public float ArrivalDistance { set { arrivalDistance = value; } }
        public Transform[] PatrolPoints { set { patrolPoints = value; } }

        // SONS
        [SerializeField] private AudioClip teleportSound;
        private AudioSource audioSource;

        private void Awake()
        {
            bossDetection = GetComponent<BossDetection>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            isWalkingHash = Animator.StringToHash("isWalking"); // Obt�m o hash da string da anima��o de caminhada
            currentBossState = BossState.Chasing; // Define o estado inicial como "Patrolling"

            player = GameObject.FindGameObjectWithTag("Player");
            teleportPoints = GameObject.Find("BossTeleportPoints");

            patrolPoints = teleportPoints.GetComponentsInChildren<Transform>();

        }

        private void Update()
        {
            switch (currentBossState)
            {

                case BossState.Chasing: // Perseguindo algum alvo
                    animator.SetBool("Conjuring", false);

                    float distanceToTarget = Vector3.Distance(transform.position, player.transform.position);

                    if (distanceToTarget > minDistanceToTarget)
                    {
                        navMeshAgent.SetDestination(player.transform.position);
                    }

                    else
                    { //Se estiver muito proximo do alvo vai parar de perseguir e começar a olhar na direção dele
                        lookToPlayer();
                    }
                    
                    break;

                case BossState.Teleporting: // Personagem est� parado atento ao inimigo que saiu de seu alcance de persegui��o
                    navMeshAgent.ResetPath();
                    lookToPlayer();
                    setWalkingAnimation(false);
                    if (Vector3.Distance(transform.position, player.transform.position) < 20f && !isTeleporting)
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
            if (teleportSound != null)
            {
                audioSource.PlayOneShot(teleportSound);
            }

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
            float distance = Random.Range(10f, 20f); 

            // Calcula a distancia
            float offsetX = distance * Mathf.Cos(angle);
            float offsetZ = distance * Mathf.Sin(angle);

            // nova pos
            Vector3 pos = player.transform.position + new Vector3(offsetX, 0f, offsetZ);

            GameObject portal = Instantiate(ThrallConjureEffect, pos + new Vector3(0,1.5f,0), Quaternion.identity);
            lookToPlayer(portal.transform, player.transform);
            GameObject Tharall = Instantiate(Thrall, pos, Quaternion.identity);
            lookToPlayer(Tharall.transform, player.transform);
            Destroy(Tharall, 20);
        }

        private void lookToPlayer()
        {

                Vector3 targetPosition = player.transform.position;
                targetPosition.y = transform.position.y;
                transform.LookAt(targetPosition);
            
        }

        private void lookToPlayer(Transform _transform, Transform target)
        {
            if (target != null)
            {
                Vector3 targetPosition = target.position;
                targetPosition.y = _transform.position.y;
                _transform.LookAt(targetPosition);
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

        private void setWalkingAnimation(bool _isWalking)
        {
            animator.SetBool(isWalkingHash, _isWalking); // Define o par�metro da anima��o de caminhada
        }

        public void forceStartChase()
        {
            currentBossState = BossState.Chasing;
            navMeshAgent.speed = chaseSpeed;

            setWalkingAnimation(true);
        }
    }
}