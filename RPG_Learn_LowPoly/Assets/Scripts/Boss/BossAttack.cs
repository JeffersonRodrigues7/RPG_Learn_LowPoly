using UnityEngine;
using RPG.Weapon;
using RPG.Projectile;
using RPG.Boss.Movement;
using Cinemachine;
using Unity.VisualScripting;
using RPG.CinemachineCamera;
using UnityEngine.AI;

namespace RPG.Boss.Attack
{
    // Enumeração dos estágios de ataque do chefe
    public enum BossAttackStage
    {
        Stage01,
        Stage02,
        Stage03,
        Stage04,
        Stage05
    }

    // Classe responsável pelos ataques do chefe
    public class BossAttack : MonoBehaviour
    {
        // Dados de dano para diferentes tipos de ataque
        [Header("DATA")]
        [SerializeField] private float handDamage = 20f;
        [SerializeField] private float swordDamage = 35f;
        [SerializeField] private float jumpDamage = 50f;
        [SerializeField] private float meteorDamage = 50f;

        // Outras configurações e referências
        [Header("Other")]
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private GameObject handPrefab;
        [SerializeField] private Transform leftHandTransform; // Transform do ponto onde a arma será anexada
        [SerializeField] private Transform rightHandTransform; // Transform do ponto onde a arma será anexada
        [SerializeField] private bool isUsingSword = false; // true -> weapon atual é a espada; false -> weapon atual é o arco
        [SerializeField] public BossAttackStage bossStage = BossAttackStage.Stage01;
        [SerializeField] private SkinnedMeshRenderer Vampire;
        [SerializeField] private GameObject Aura;
        [SerializeField] private Material newFirstMaterial;
        [SerializeField] private Material newSecondMaterial;

        private BossMovement bossMovement;
        private Animator animator;
        private GameObject weapon;
        private WeaponController leftWeaponController; // Controlador da arma
        private WeaponController rightWeaponController; // Controlador da arma

        private NavMeshAgent navMeshAgent;

        private int meleeAttackingHash; // Hash da String que se refere a animação de Melee Attacking
        private int rangedAttackingHash; // Hash da String que se refere a animação de Melee Attacking

        private ShakeCamera shakeCamera;

        private bool isJumping = false;

        public float MeteorDamage { get { return meteorDamage; } }

        private AudioSource audioSource;
        // Sons
        [SerializeField] private AudioClip swordHitSound;
        [SerializeField] private AudioClip roarSound;
        [SerializeField] private AudioClip bossJump;
        private float roarInterval = 5f; // Intervalo de tempo entre os rugidos
        private bool canRoar = true;

        private void Awake()
        {
            // Inicializa os componentes
            navMeshAgent = GetComponent<NavMeshAgent>();
            bossMovement = GetComponent<BossMovement>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            // Configuração do áudio
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            Aura.SetActive(false);
            shakeCamera = GameObject.Find("ShakeCamera").GetComponent<ShakeCamera>();

            // Conversão das strings dos nomes das animações em hash para melhor performance
            meleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack");
            rangedAttackingHash = Animator.StringToHash("TriggerRangedAttack");

            // Configuração do controlador da arma para a mão esquerda
            leftWeaponController = handPrefab.GetComponent<WeaponController>();
            leftWeaponController.EnemyTag = "Player"; // Define a tag do inimigo
            leftWeaponController.EnemyTag2 = "Ally"; // Define a tag do inimigo
            leftWeaponController.Damage = handDamage;

            // Força o início da perseguição
            bossMovement.forceStartChase();
        }

        // Método para instanciar a arma
        private void spawnWeapon(GameObject weaponPrefab, Transform hand)
        {
            weapon = Instantiate(weaponPrefab, hand); // Instancia a arma no ponto especificado
            rightWeaponController = weapon.GetComponent<WeaponController>(); // Obtém o controlador da arma
            rightWeaponController.EnemyTag = "Player"; // Define a tag do inimigo
            rightWeaponController.EnemyTag2 = "Ally"; // Define a tag do inimigo
            rightWeaponController.Damage = swordDamage;
        }

        // Método para reproduzir o som de ataque da espada
        private void PlaySwordAttackSound()
        {
            if (swordHitSound != null)
            {
                audioSource.PlayOneShot(swordHitSound);
            }
        }

        // Define o estágio atual do ataque do chefe
        public void setStage(BossAttackStage stage)
        {
            Debug.Log("Iniciando estado: " + stage);
            bossStage = stage;

            if (bossStage == BossAttackStage.Stage03)
            {
                bossMovement.forceStartChase();
                spawnWeapon(swordPrefab, rightHandTransform);
                isJumping = true;
                animator.SetTrigger("TriggerJump");
            }

            if (bossStage == BossAttackStage.Stage04)
            {
                bossMovement.currentBossState = BossState.SummoningMeteors;
            }

            if (bossStage == BossAttackStage.Stage05)
            {
                transform.localScale = new Vector3(3, 3, 3);
                Aura.SetActive(true);
                Material[] materialsCopy = Vampire.materials;

                // Modifica os materiais na cópia
                materialsCopy[0] = newFirstMaterial;
                materialsCopy[1] = newSecondMaterial;

                // Atribui a cópia modificada de volta ao SkinnedMeshRenderer
                Vampire.materials = materialsCopy;

                if (rightWeaponController != null) rightWeaponController.changeMaterial();
            }
        }

        // Inicia a animação de ataque com base no estágio atual
        public void startAttackAnimation(Transform _target)
        {
            switch (bossStage)
            {
                case BossAttackStage.Stage01:
                    animator.SetTrigger(rangedAttackingHash);
                    break;
                case BossAttackStage.Stage02:
                    bossMovement.currentBossState = BossState.Teleporting;
                    break;
                case BossAttackStage.Stage03:
                    if (!isJumping) animator.SetTrigger(meleeAttackingHash);
                    break;
                case BossAttackStage.Stage04:
                    animator.SetTrigger(meleeAttackingHash);
                    break;
                case BossAttackStage.Stage05:
                    animator.SetTrigger("TriggerJump");
                    break;
            }
        }

        // Chamado pela animação de ataque para ativar o ataque da mão esquerda
        public void activeLeftAttack()
        {
            leftWeaponController.IsAttacking = true; // Ativa o ataque da arma

            // Adiciona isso para reproduzir o rugido a cada 5 segundos
            if (canRoar && roarSound != null)
            {
                canRoar = false;
                InvokeRepeating(nameof(PlayRoarSound), 0f, roarInterval);
            }
        }

        // Reproduz o som de rugido
        private void PlayRoarSound()
        {
            if (roarSound != null)
            {
                audioSource.PlayOneShot(roarSound);
            }
        }

        // Ativa o ataque da mão direita
        public void activeRightAttack()
        {
            rightWeaponController.Damage = swordDamage;
            rightWeaponController.IsAttacking = true; // Ativa o ataque da arma
            if (swordHitSound != null)
            {
                audioSource.PlayOneShot(swordHitSound);
            }
        }

        // Inicia o pulo do chefe
        public void startJump()
        {
            navMeshAgent.speed = 30f;
        }

        // Ativa o ataque durante o pulo
        public void activeJumpAttack()
        {
            rightWeaponController.Damage = jumpDamage;
            rightWeaponController.IsAttacking = true; // Ativa o ataque da arma
            if (bossJump != null)
            {
                audioSource.volume = 0.6f;
                audioSource.PlayOneShot(bossJump);
            }
            audioSource.volume = 0.47f;
        }

        // Ativa o efeito de tremor na câmera
        public void shakeCameraEffect()
        {
            shakeCamera.startShaking(10f);
        }

        // Chamado pela animação de ataque para desativar o ataque
        public void desactiveAttack()
        {
            navMeshAgent.speed = bossMovement.ChaseSpeed;
            leftWeaponController.IsAttacking = false; // Desativa o ataque da arma
            if (rightWeaponController != null) rightWeaponController.IsAttacking = false; // Desativa o ataque da arma
        }

        // Chamado pela animação de ataque para parar o efeito de pulo
        public void stopJumpEffect()
        {
            isJumping = false;
        }
    }
}
