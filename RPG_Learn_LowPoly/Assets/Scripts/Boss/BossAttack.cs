using UnityEngine;
using RPG.Weapon;
using RPG.Projectile;
using RPG.Boss.Movement;

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
        [SerializeField] private float handDamage = 30f;
        [SerializeField] private float swordDamage = 50f;
        [SerializeField] private float projectileDamage = 15f;

        [Header("Other")]
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private GameObject handPrefab;
        [SerializeField] private Transform leftHandTransform; // Transform do ponto onde a arma ser� anexada
        [SerializeField] private Transform rightHandTransform; // Transform do ponto onde a arma ser� anexada
        [SerializeField] private GameObject projectileprefab;
        [SerializeField] private bool isUsingSword = false;//�true-> weapon atual � a espada; false -> weapon atua�l � o arco
        [SerializeField] public BossAttackStage bossStage = BossAttackStage.Stage01;

        private BossMovement bossMovement;

        private Transform ArrowParents;
        private GameObject projectileInstance;
        private ProjectileController projectileController = null;

        private Animator animator;
        private GameObject weapon;
        private WeaponController leftWeaponController; // Controlador da arma
        private WeaponController rightWeaponController; // Controlador da arma

        private bool isMeleeAttacking = false; // Flag para determinar se o jogador est� usando o melee attack
        private int meleeAttackingHash; //Hash da String que se refere a anima��o de Melee Attacking

        private int rangedAttackingHash; //Hash da String que se refere a anima��o de Melee Attacking

        private Transform target;

        public bool IsMeleeAttacking { get { return isMeleeAttacking; } }

        public float Damage { set { swordDamage = value; } }

        private void Awake()
        {
            bossMovement = GetComponent<BossMovement>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            meleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack"); // Obt�m o hash da string da anima��o de ataque corpo a corpo
            rangedAttackingHash = Animator.StringToHash("TriggerRangedAttack");

            ArrowParents = GameObject.Find("ArrowsParent").transform;
            leftWeaponController = handPrefab.GetComponent<WeaponController>();
            leftWeaponController.EnemyTag = "Player"; // Define a tag do inimigo
            leftWeaponController.EnemyTag2 = "Ally"; // Define a tag do inimigo
            leftWeaponController.Damage = handDamage;

            //if (isUsingSword) spawnWeapon(swordPrefab, rightHandTransform);
            //else spawnWeapon(bowPrefab, leftHandTransform);
        }

        private void spawnWeapon(GameObject weaponPrefab, Transform hand)
        {
            weapon = Instantiate(weaponPrefab, hand); // Instancia a arma no ponto especificado
            rightWeaponController = weapon.GetComponent<WeaponController>(); // Obt�m o controlador da arma
            rightWeaponController.EnemyTag = "Player"; // Define a tag do inimigo
            rightWeaponController.EnemyTag2 = "Ally"; // Define a tag do inimigo
            rightWeaponController.Damage = swordDamage;
        }

        public void setStage(BossAttackStage stage)
        {
            Debug.Log("Iniciando estado: " + stage);
            bossStage = stage;

            if (bossStage == BossAttackStage.Stage02) spawnWeapon(swordPrefab, rightHandTransform);
        }

        public void startAttackAnimation(Transform _target)
        {
            switch (bossStage)
            {
                case BossAttackStage.Stage01:
                    animator.SetTrigger(rangedAttackingHash); 
                    isMeleeAttacking = true; 
                    break;
                case BossAttackStage.Stage02:
                    //bossMovement.currentBossState = BossState.Teleporting;
                    animator.SetTrigger(meleeAttackingHash); 
                    isMeleeAttacking = true; 
                    break;
                case BossAttackStage.Stage03:
                    break;
                case BossAttackStage.Stage04:
                    break;
                case BossAttackStage.Stage05:
                    break;
            }
        }

        public void shootArrow()
        {
            // Instancia um proj�til para ataque � dist�ncia, direcionado ao alvo determinado
            //Debug.Log(target.name); // Nome do alvo (apenas para debug)
            projectileInstance = Instantiate(projectileprefab, rightHandTransform.position, Quaternion.identity, ArrowParents);
            projectileController = projectileInstance?.GetComponent<ProjectileController>();
            projectileController.Damage = projectileDamage;

            if (target != null)
            {
                projectileController?.SetTarget(tag, target.position + new Vector3(0,1f,0), target.tag); // Define o alvo do proj�til como o jogador
            }

            Destroy(projectileInstance.gameObject, 10f); // Destruir o proj�til ap�s um tempo determinado
        }

        // Chamado pela anima��o de ataque
        public void activeLeftAttack()
        {
            leftWeaponController.IsAttacking = true; // Ativa o ataque da arma
        }

        public void activeRightAttack()
        {
            rightWeaponController.IsAttacking = true; // Ativa o ataque da arma
        }

        // Chamado pela anima��o de ataque
        public void desactiveAttack()
        {
            isMeleeAttacking = false; // Desativa o ataque corpo a corpo
            target = null;
            leftWeaponController.IsAttacking = false; // Desativa o ataque da arma
            if(rightWeaponController != null) rightWeaponController.IsAttacking = false; // Desativa o ataque da arma
        }
    }
}