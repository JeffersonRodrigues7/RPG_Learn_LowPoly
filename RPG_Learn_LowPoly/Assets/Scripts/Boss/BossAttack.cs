using UnityEngine;
using RPG.Weapon;
using RPG.Projectile;

namespace RPG.Boss.Attack
{
    public class BossAttack : MonoBehaviour
    {
        [Header("DATA")]
        [SerializeField] private float handDamage = 30f;
        [SerializeField] private float swordDamage = 50f;
        [SerializeField] private float projectileDamage = 15f;

        [Header("Other")]
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private GameObject handPrefab;
        [SerializeField] private Transform leftHandTransform; // Transform do ponto onde a arma será anexada
        [SerializeField] private Transform rightHandTransform; // Transform do ponto onde a arma será anexada
        [SerializeField] private GameObject projectileprefab;
        [SerializeField] private bool isUsingSword = false;//´true-> weapon atual é a espada; false -> weapon atua´l é o arco

        private Transform ArrowParents;
        private GameObject projectileInstance;
        private ProjectileController projectileController = null;

        private Animator animator;
        private GameObject weapon;
        private WeaponController leftWeaponController; // Controlador da arma
        private WeaponController rightWeaponController; // Controlador da arma

        private bool isMeleeAttacking = false; // Flag para determinar se o jogador está usando o melee attack
        private int meleeAttackingHash; //Hash da String que se refere a animação de Melee Attacking

        private bool isRangedAttacking = false; // Flag para determinar se o jogador está usando o melee attack
        private int rangedAttackingHash; //Hash da String que se refere a animação de Melee Attacking

        private Transform target;

        public bool IsMeleeAttacking { get { return isMeleeAttacking; } }
        public bool IsRangedAttacking { get { return isRangedAttacking; } }

        public float Damage { set { swordDamage = value; } }

        private void Start()
        {
            animator = GetComponent<Animator>();
            meleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack"); // Obtém o hash da string da animação de ataque corpo a corpo
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
            rightWeaponController = weapon.GetComponent<WeaponController>(); // Obtém o controlador da arma
            rightWeaponController.EnemyTag = "Player"; // Define a tag do inimigo
            rightWeaponController.EnemyTag2 = "Ally"; // Define a tag do inimigo
            rightWeaponController.Damage = swordDamage;
        }

        public void startAttackAnimation(Transform _target)
        {
            // Inicia a animação de ataque, com base no tipo de arma e estado de ataque do jogador
            if (isUsingSword && !isMeleeAttacking) // Se estiver usando espada e não estiver atacando
            {
                animator.SetTrigger(meleeAttackingHash); // Inicia a animação de ataque corpo a corpo
                isMeleeAttacking = true; // Define o estado de ataque corpo a corpo como verdadeiro
            }

            else if (!isUsingSword && !isRangedAttacking) // Se não estiver usando espada e não estiver atacando à distância
            {
                animator.SetTrigger(rangedAttackingHash); // Inicia a animação de ataque à distância
                isRangedAttacking = true; // Define o estado de ataque à distância como verdadeiro
                target = _target; // Define o alvo do ataque à distância
            }
        }

        public void shootArrow()
        {
            // Instancia um projétil para ataque à distância, direcionado ao alvo determinado
            //Debug.Log(target.name); // Nome do alvo (apenas para debug)
            projectileInstance = Instantiate(projectileprefab, rightHandTransform.position, Quaternion.identity, ArrowParents);
            projectileController = projectileInstance?.GetComponent<ProjectileController>();
            projectileController.Damage = projectileDamage;

            if (target != null)
            {
                projectileController?.SetTarget(tag, target.position + new Vector3(0,1f,0), target.tag); // Define o alvo do projétil como o jogador
            }

            Destroy(projectileInstance.gameObject, 10f); // Destruir o projétil após um tempo determinado
        }

        // Chamado pela animação de ataque
        public void activeLeftAttack()
        {
            leftWeaponController.IsAttacking = true; // Ativa o ataque da arma
        }

        // Chamado pela animação de ataque
        public void desactiveAttack()
        {
            isMeleeAttacking = false; // Desativa o ataque corpo a corpo
            isRangedAttacking = false;
            target = null;
            leftWeaponController.IsAttacking = false; // Desativa o ataque da arma
        }
    }
}