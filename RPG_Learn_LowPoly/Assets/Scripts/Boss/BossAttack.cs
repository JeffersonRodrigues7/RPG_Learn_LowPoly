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
    public enum BossAttackStage // Estados possiveis
    {
        Stage01,
        Stage02,
        Stage03,
        Stage04,
        Stage05
    }
    public class BossAttack : MonoBehaviour
    {

        [Header("DATA")]
        [SerializeField] private float handDamage = 20f;
        [SerializeField] private float swordDamage = 35f;
        [SerializeField] private float jumpDamage = 50f;
        [SerializeField] private float meteorDamage = 50f;

        [Header("Other")]
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private GameObject handPrefab;
        [SerializeField] private Transform leftHandTransform; // Transform do ponto onde a arma ser� anexada
        [SerializeField] private Transform rightHandTransform; // Transform do ponto onde a arma ser� anexada
        [SerializeField] private bool isUsingSword = false;//�true-> weapon atual � a espada; false -> weapon atua�l � o arco
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

        private int meleeAttackingHash; //Hash da String que se refere a anima��o de Melee Attacking
        private int rangedAttackingHash; //Hash da String que se refere a anima��o de Melee Attacking

        private ShakeCamera shakeCamera;

        private bool isJumping = false;

        public float MeteorDamage { get { return meteorDamage; } }

        private AudioSource audioSource;
        // SONS
        [SerializeField] private AudioClip swordHitSound;
        [SerializeField] private AudioClip roarSound;
        [SerializeField] private AudioClip bossJump;
        private float roarInterval = 5f; // Intervalo de tempo entre os rugidos
        private bool canRoar = true;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            bossMovement = GetComponent<BossMovement>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            Aura.SetActive(false);
            shakeCamera = GameObject.Find("ShakeCamera").GetComponent<ShakeCamera>();

            meleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack"); // Obt�m o hash da string da anima��o de ataque corpo a corpo
            rangedAttackingHash = Animator.StringToHash("TriggerRangedAttack");

            leftWeaponController = handPrefab.GetComponent<WeaponController>();
            leftWeaponController.EnemyTag = "Player"; // Define a tag do inimigo
            leftWeaponController.EnemyTag2 = "Ally"; // Define a tag do inimigo
            leftWeaponController.Damage = handDamage;

            bossMovement.forceStartChase();
        }

        private void spawnWeapon(GameObject weaponPrefab, Transform hand)
        {
            weapon = Instantiate(weaponPrefab, hand); // Instancia a arma no ponto especificado
            rightWeaponController = weapon.GetComponent<WeaponController>(); // Obt�m o controlador da arma
            rightWeaponController.EnemyTag = "Player"; // Define a tag do inimigo
            rightWeaponController.EnemyTag2 = "Ally"; // Define a tag do inimigo
            rightWeaponController.Damage = swordDamage;

        }
        
        private void PlaySwordAttackSound()
        {
            if (swordHitSound != null)
            {
                audioSource.PlayOneShot(swordHitSound);
            }
        }
        
        public void setStage(BossAttackStage stage)
        {
            Debug.Log("Iniciando estado: " + stage);
            bossStage = stage;

            if (bossStage == BossAttackStage.Stage03) {
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

                // Modifique os materiais na c�pia
                materialsCopy[0] = newFirstMaterial;
                materialsCopy[1] = newSecondMaterial;

                // Atribua a c�pia modificada de volta ao SkinnedMeshRenderer
                Vampire.materials = materialsCopy;

                if (rightWeaponController != null) rightWeaponController.changeMaterial();
            }
        }

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
                    if(!isJumping) animator.SetTrigger(meleeAttackingHash);
                    break;
                case BossAttackStage.Stage04:
                    animator.SetTrigger(meleeAttackingHash);
                    break;
                case BossAttackStage.Stage05:
                    animator.SetTrigger("TriggerJump");
                    break;
            }
        }

        // Chamado pela anima��o de ataque
        public void activeLeftAttack()
        {
            leftWeaponController.IsAttacking = true; // Ativa o ataque da arma

            // Adicione isso para reproduzir o rugido a cada 5 segundos
            if (canRoar && roarSound != null)
            {
                canRoar = false;
                InvokeRepeating(nameof(PlayRoarSound), 0f, roarInterval);
            }
        }

        private void PlayRoarSound()
        {
            if (roarSound != null)
            {
                audioSource.PlayOneShot(roarSound);
            }
        }

        public void activeRightAttack()
        {
            rightWeaponController.Damage = swordDamage;
            rightWeaponController.IsAttacking = true; // Ativa o ataque da arma
            if (swordHitSound != null)
            {
                audioSource.PlayOneShot(swordHitSound);
            }
        }

        public void startJump()
        {
            navMeshAgent.speed = 30f;
        }

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

        public void shakeCameraEffect()
        {
            shakeCamera.startShaking(10f);
        }

        // Chamado pela anima��o de ataque
        public void desactiveAttack()
        {
            navMeshAgent.speed = bossMovement.ChaseSpeed;
            leftWeaponController.IsAttacking = false; // Desativa o ataque da arma
            if(rightWeaponController != null) rightWeaponController.IsAttacking = false; // Desativa o ataque da arma
        }

        // Chamado pela anima��o de ataque
        public void stopJumpEffect()
        {
            isJumping = false;
        }
    }
}