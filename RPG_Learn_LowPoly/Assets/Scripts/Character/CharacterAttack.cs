using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Weapon;
using RPG.Projectile;
using UnityEngine.InputSystem.HID;

namespace RPG.Character.Attack
{
    public class CharacterAttack : MonoBehaviour
    {
        [Header("CharacterData")]
        [SerializeField] private float damage = 100f; 

        [Header("Other")]
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private GameObject bowPrefab;
        [SerializeField] private Transform leftHandTransform; // Transform do ponto onde a arma será anexada
        [SerializeField] private Transform rightHandTransform; // Transform do ponto onde a arma será anexada
        [SerializeField] private GameObject projectileprefab;
        [SerializeField] private bool isUsingSword = false;//´true-> weapon atual é a espada; false -> weapon atua´l é o arco

        private Transform ArrowParents;
        private GameObject projectileInstance;
        private ProjectileController projectileController = null;

        private Animator animator;
        private GameObject weapon; 
        private WeaponController weaponController; // Controlador da arma

        private bool isMeleeAttacking = false; // Flag para determinar se o jogador está usando o melee attack
        private int meleeAttackingHash; //Hash da String que se refere a animação de Melee Attacking

        private bool isRangedAttacking = false; // Flag para determinar se o jogador está usando o melee attack
        private int rangedAttackingHash; //Hash da String que se refere a animação de Melee Attacking

        private Transform target;

        public bool IsMeleeAttacking { get { return isMeleeAttacking; } }
        public bool IsRangedAttacking { get { return isRangedAttacking; } }

        public float Damage { set { damage = value; } }

        private void Start()
        {
            animator = GetComponent<Animator>();
            meleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack"); // Obtém o hash da string da animação de ataque corpo a corpo
            rangedAttackingHash = Animator.StringToHash("TriggerRangedAttack");

            ArrowParents = GameObject.Find("ArrowsParent").transform;

            if (isUsingSword) spawnWeapon(swordPrefab, rightHandTransform);
            else spawnWeapon(bowPrefab, leftHandTransform);
        }

        private void spawnWeapon(GameObject weaponPrefab, Transform hand)
        {
            weapon = Instantiate(weaponPrefab, hand); // Instancia a arma no ponto especificado
            weaponController = weapon.GetComponent<WeaponController>(); // Obtém o controlador da arma
            weaponController.EnemyTag = "Player"; // Define a tag do inimigo
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

            if (target != null)
            {
                projectileController?.SetTarget(tag, target.position + new Vector3(0,1f,0), "Player"); // Define o alvo do projétil como o jogador
            }

            Destroy(projectileInstance.gameObject, 10f); // Destruir o projétil após um tempo determinado
        }

        // Chamado pela animação de ataque
        public void activeAttack()
        {
            weaponController.IsAttacking = true; // Ativa o ataque da arma
        }

        // Chamado pela animação de ataque
        public void desactiveAttack()
        {
            isMeleeAttacking = false; // Desativa o ataque corpo a corpo
            isRangedAttacking = false;
            target = null;
            weaponController.IsAttacking = false; // Desativa o ataque da arma
        }
    }
}