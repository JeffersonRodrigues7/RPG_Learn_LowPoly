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
        [SerializeField] private Transform leftHandTransform; // Transform do ponto onde a arma será anexada
        [SerializeField] private Transform rightHandTransform; // Transform do ponto onde a arma será anexada
        [SerializeField] private bool isUsingSword = false;//´true-> weapon atual é a espada; false -> weapon atua´l é o arco
        [SerializeField] public BossAttackStage bossStage = BossAttackStage.Stage01;

        private BossMovement bossMovement;
        private Animator animator;
        private GameObject weapon;
        private WeaponController leftWeaponController; // Controlador da arma
        private WeaponController rightWeaponController; // Controlador da arma

        private NavMeshAgent navMeshAgent;

        private int meleeAttackingHash; //Hash da String que se refere a animação de Melee Attacking
        private int rangedAttackingHash; //Hash da String que se refere a animação de Melee Attacking

        private ShakeCamera shakeCamera;

        private bool isJumping = false;

        public float MeteorDamage { get { return meteorDamage; } }


        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            bossMovement = GetComponent<BossMovement>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            shakeCamera = GameObject.Find("ShakeCamera").GetComponent<ShakeCamera>();

            meleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack"); // Obtém o hash da string da animação de ataque corpo a corpo
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
            rightWeaponController = weapon.GetComponent<WeaponController>(); // Obtém o controlador da arma
            rightWeaponController.EnemyTag = "Player"; // Define a tag do inimigo
            rightWeaponController.EnemyTag2 = "Ally"; // Define a tag do inimigo
            rightWeaponController.Damage = swordDamage;
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
                    if (bossMovement.currentBossState != BossState.SummoningMeteors) bossMovement.currentBossState = BossState.SummoningMeteors;
                    else animator.SetTrigger(meleeAttackingHash);
                    break;
                case BossAttackStage.Stage05:
                    animator.SetTrigger(meleeAttackingHash);
                    break;
            }
        }

        // Chamado pela animação de ataque
        public void activeLeftAttack()
        {
            leftWeaponController.IsAttacking = true; // Ativa o ataque da arma
        }

        public void activeRightAttack()
        {
            rightWeaponController.Damage = swordDamage;
            rightWeaponController.IsAttacking = true; // Ativa o ataque da arma
        }

        public void startJump()
        {
            navMeshAgent.speed = 30f;
        }

        public void activeJumpAttack()
        {
            rightWeaponController.Damage = jumpDamage;
            rightWeaponController.IsAttacking = true; // Ativa o ataque da arma
        }

        public void shakeCameraEffect()
        {
            shakeCamera.startShaking(10f);
        }

        // Chamado pela animação de ataque
        public void desactiveAttack()
        {
            navMeshAgent.speed = bossMovement.ChaseSpeed;
            leftWeaponController.IsAttacking = false; // Desativa o ataque da arma
            if(rightWeaponController != null) rightWeaponController.IsAttacking = false; // Desativa o ataque da arma
        }

        // Chamado pela animação de ataque
        public void stopJumpEffect()
        {
            isJumping = false;
        }
    }
}