using RPG.Boss.Attack;
using RPG.Boss.Detection;
using RPG.Projectile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

namespace RPG.Boss.Movement
{

    public enum BossState // Estados possiveis
    {
        Chasing,
        Teleporting,
        SummoningMeteors

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
        [SerializeField] private GameObject[] Meteor; // Pontos de patrulha do personagem
        [SerializeField] private GameObject ThrallConjureEffect; // Pontos de patrulha do personagem

        private BossAttack bossAttack;

        public BossState currentBossState;

        private Animator animator; 
        private NavMeshAgent navMeshAgent;

        private int isWalkingHash; // Hash da String que se refere � anima��o de Walk

        private GameObject player;
        
        private bool isTeleporting = false;
        private Vector3 positionBeforeTeleport;

        private bool initMeteor = false;

        public float ChaseSpeed { get { return chaseSpeed; } set { chaseSpeed = value; } }
        public float ArrivalDistance { set { arrivalDistance = value; } }
        public Transform[] PatrolPoints { set { patrolPoints = value; } }

        // SONS
        [SerializeField] private AudioClip teleportSound;
        private AudioSource audioSource;

        private GameObject teleportPoints;
        private Transform thrallsParent;
        private Transform portalsParent;
        private Transform meteorsParent;
        private Transform effectsParent;


        private void Awake()
        {
            bossAttack = GetComponent<BossAttack>();
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
            thrallsParent = GameObject.Find("Thralls").transform;
            portalsParent = GameObject.Find("Portals").transform;
            meteorsParent = GameObject.Find("Meteors").transform;
            effectsParent = GameObject.Find("Effects").transform;

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

                case BossState.SummoningMeteors: // Personagem est� parado atento ao inimigo que saiu de seu alcance de perseguicao
                    if (!initMeteor)
                    {
                        initMeteor = true;
                        positionBeforeTeleport = transform.position;
                        isTeleporting = true;
                        animator.SetTrigger("TriggerTeleport");
                    }

                    navMeshAgent.ResetPath();
                    lookToPlayer();
                    setWalkingAnimation(false);

                    if (Vector3.Distance(transform.position, player.transform.position) > 15f && !isTeleporting)
                    {
                        animator.SetBool("ConjuringMeteor", true);
                    }
                    else if(Vector3.Distance(transform.position, player.transform.position) <= 15f && !isTeleporting)
                    {
                        animator.SetBool("ConjuringMeteor", false);
                        forceStartChase();
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

            Instantiate(TeleportEffect, positionBeforeTeleport, Quaternion.identity, effectsParent);
            Instantiate(TeleportEffect, positionBeforeTeleport, Quaternion.identity, effectsParent);
            Instantiate(TeleportEffect, positionBeforeTeleport, Quaternion.identity, effectsParent);


            Vector3 nextPosition;

            if (currentBossState == BossState.SummoningMeteors) nextPosition = FindFarthestPatrolPoint(player.transform).position;
            else nextPosition = FindOtherPoint();

            transform.position = nextPosition;
            isTeleporting = false;
        }

        //ConjuraMinion, chamada através de animação
        public void conjureThrall()
        {
            // Gera pos e rotacao aleatoria
            float angle = Random.Range(0f, 2f * Mathf.PI); 
            float distance = Random.Range(10f, 20f); 

            // Calcula a distancia
            float offsetX = distance * Mathf.Cos(angle);
            float offsetZ = distance * Mathf.Sin(angle);

            // nova pos
            Vector3 pos = player.transform.position + new Vector3(offsetX, 0f, offsetZ);

            GameObject portal = Instantiate(ThrallConjureEffect, pos + new Vector3(0,1.5f,0), Quaternion.identity, portalsParent);
            lookToPlayer(portal.transform, player.transform);
            GameObject Tharall = Instantiate(Thrall, pos, Quaternion.identity, thrallsParent);
            lookToPlayer(Tharall.transform, player.transform);
            Destroy(Tharall, 15);
        }


        public void conjureMeteor()
        {
            int value = Random.Range(0, 5);
            GameObject meteor = Instantiate(Meteor[value], transform.position + new Vector3(0, 10f, 0), Quaternion.identity, meteorsParent);
            ProjectileController projectile = meteor.GetComponent<ProjectileController>();
            projectile.SetTarget(tag, player.transform.position, "Player");
            projectile.Damage = bossAttack.MeteorDamage;
            Destroy(meteor, 10);
        }

        Transform FindFarthestPatrolPoint(Transform referencePoint)
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                Debug.LogError("Patrol points array is empty or not assigned.");
                return null;
            }

            Transform farthestPoint = null;
            float maxDistance = 0f;

            foreach (Transform point in patrolPoints)
            {
                float currentDistance = Vector3.Distance(referencePoint.position, point.position);
                Debug.Log(currentDistance);
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    farthestPoint = point;
                }
            }
            //Debug.Log("Final: " + farthestPoint.position);
            return farthestPoint;
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