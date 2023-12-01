using RPG.Boss.Attack;
using RPG.Boss.Detection;
using RPG.Projectile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Boss.Movement
{

    public enum BossState // Estados possíveis
    {
        Chasing,
        Teleporting,
        SummoningMeteors
    }

    public class BossMovement : MonoBehaviour
    {
        [Header("BossData")]
        [SerializeField] private float chaseSpeed = 10; // Velocidade de perseguição do personagem
        [SerializeField] private float minDistanceToTarget = 2f; // Vai parar de seguir quando chegar a essa distância

        [Header("Effects")]
        private float arrivalDistance = 0.1f; // Distância para considerar que o personagem chegou à posição final
        [SerializeField] private Transform[] patrolPoints; // Pontos de patrulha do personagem
        [SerializeField] private GameObject TeleportEffect; // Efeito de teleporte
        [SerializeField] private GameObject Thrall; // Objeto Thrall
        [SerializeField] private GameObject[] Meteor; // Objetos Meteor
        [SerializeField] private GameObject ThrallConjureEffect; // Efeito de conjuração do Thrall

        private BossAttack bossAttack;

        public BossState currentBossState;

        private Animator animator;
        private NavMeshAgent navMeshAgent;

        private int isWalkingHash; // Hash da String que se refere à animação de Walk

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

            isWalkingHash = Animator.StringToHash("isWalking"); // Obtém o hash da string da animação de caminhada
            currentBossState = BossState.Chasing; // Define o estado inicial como "Chasing"

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
                    { // Se estiver muito próximo do alvo, vai parar de perseguir e começar a olhar na direção dele
                        lookToPlayer();
                    }
                    break;

                case BossState.Teleporting: // Personagem está parado atento ao inimigo que saiu de seu alcance de perseguição
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

                case BossState.SummoningMeteors: // Personagem está parado atento ao inimigo que saiu de seu alcance de perseguição
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
                    else if (Vector3.Distance(transform.position, player.transform.position) <= 15f && !isTeleporting)
                    {
                        animator.SetBool("ConjuringMeteor", false);
                        forceStartChase();
                    }
                    break;

                default:
                    break;
            }
        }

        // Função chamada através da animação de teleport
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

        // Conjuração de Thrall, chamada através de animação
        public void conjureThrall()
        {
            // Gera posição e rotação aleatória
            float angle = Random.Range(0f, 2f * Mathf.PI);
            float distance = Random.Range(10f, 20f);

            // Calcula a distância
            float offsetX = distance * Mathf.Cos(angle);
            float offsetZ = distance * Mathf.Sin(angle);

            // Nova posição
            Vector3 pos = player.transform.position + new Vector3(offsetX, 0f, offsetZ);

            //Conjura os servos e os portais
            GameObject portal = Instantiate(ThrallConjureEffect, pos + new Vector3(0, 1.5f, 0), Quaternion.identity, portalsParent);
            lookToPlayer(portal.transform, player.transform);
            GameObject Tharall = Instantiate(Thrall, pos, Quaternion.identity, thrallsParent);
            lookToPlayer(Tharall.transform, player.transform);
            Destroy(Tharall, 15);
        }

        public void conjureMeteor()
        {
            // Escolhe aleatoriamente um índice para selecionar o tipo de meteor a ser conjurado
            int value = Random.Range(0, 5);

            // Instancia o meteor na posição atual com uma elevação para evitar colisões imediatas
            GameObject meteor = Instantiate(Meteor[value], transform.position + new Vector3(0, 10f, 0), Quaternion.identity, meteorsParent);

            // Obtém o controlador do projectile associado ao meteor
            ProjectileController projectile = meteor.GetComponent<ProjectileController>();

            // Define o alvo, a posição do jogador, e o tipo de alvo para o projectile
            projectile.SetTarget(tag, player.transform.position, "Player");

            // Define o dano do meteor com base no valor atribuído na classe BossAttack
            projectile.Damage = bossAttack.MeteorDamage;

            // Destroi o meteor após um tempo (10 segundos neste caso)
            Destroy(meteor, 10);
        }

        // Encontra o ponto de patrulha mais distante em relação a um ponto de referência
        Transform FindFarthestPatrolPoint(Transform referencePoint)
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                Debug.LogError("Patrol points array is empty or not assigned.");
                return null;
            }

            Transform farthestPoint = null;
            float maxDistance = 0f;

            // Itera sobre os pontos de patrulha para encontrar o mais distante
            foreach (Transform point in patrolPoints)
            {
                float currentDistance = Vector3.Distance(referencePoint.position, point.position);

                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    farthestPoint = point;
                }
            }
            return farthestPoint;
        }

        // Orienta o NPC para olhar na direção do jogador
        private void lookToPlayer()
        {
            Vector3 targetPosition = player.transform.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
        }

        // Orienta um objeto específico para olhar na direção de um alvo
        private void lookToPlayer(Transform _transform, Transform target)
        {
            if (target != null)
            {
                Vector3 targetPosition = target.position;
                targetPosition.y = _transform.position.y;
                _transform.LookAt(targetPosition);
            }
        }

        // Encontra um ponto de patrulha aleatório
        public Vector3 FindOtherPoint()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                // Lida com o caso em que o array de Transform está vazio
                return Vector3.zero;
            }

            // Seleciona um índice aleatório
            int randomIndex = Random.Range(0, patrolPoints.Length);

            // Retorna a posição do ponto de patrulha selecionado
            return patrolPoints[randomIndex].position;
        }

        // Define o parâmetro da animação de caminhada
        private void setWalkingAnimation(bool _isWalking)
        {
            animator.SetBool(isWalkingHash, _isWalking);
        }

        // Força o início da perseguição pelo chefe
        public void forceStartChase()
        {
            currentBossState = BossState.Chasing;
            navMeshAgent.speed = chaseSpeed;

            // Ativa a animação de caminhada
            setWalkingAnimation(true);
        }
    }
}
