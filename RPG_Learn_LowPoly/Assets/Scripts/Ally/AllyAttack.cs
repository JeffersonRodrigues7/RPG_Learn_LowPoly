using RPG.Projectile;
using RPG.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Ally.Attack
{

    public class AllyAttack : MonoBehaviour
    {
        [SerializeField] private GameObject bowPrefab;
        [SerializeField] private Transform leftHandTransform; // Transform do ponto onde a arma será anexada
        [SerializeField] private Transform rightHandTransform; // Transform do ponto onde a arma será anexada
        [SerializeField] private GameObject projectileprefab;
        
        private Transform ArrowParents;
        private GameObject projectileInstance;
        private ProjectileController projectileController = null;

        private Animator animator;
        private GameObject weapon;
        private WeaponController weaponController; // Controlador da arma

        private Transform target;

        private bool isRangedAttacking = false; // Flag para determinar se o jogador está usando o melee attack
        private int rangedAttackingHash; //Hash da String que se refere a animação de Melee Attacking

        public bool IsRangedAttacking { get { return isRangedAttacking; } set { isRangedAttacking = value; } }

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            rangedAttackingHash = Animator.StringToHash("TriggerRangedAttack");

            ArrowParents = GameObject.Find("ArrowsParent").transform;

            spawnWeapon(bowPrefab, leftHandTransform);
        }

        private void spawnWeapon(GameObject weaponPrefab, Transform hand)
        {
            weapon = Instantiate(weaponPrefab, hand); // Instancia a arma no ponto especificado
            weaponController = weapon.GetComponent<WeaponController>(); // Obtém o controlador da arma
            weaponController.EnemyTag = "Enemy"; // Define a tag do inimigo
        }

        public void startAttackAnimation(Transform _target)
        {
            animator.SetTrigger(rangedAttackingHash); // Inicia a animação de ataque à distância
            target = _target; // Define o alvo do ataque à distância
        }

        // Chamado pela animação de ataque
        public void shootArrow()
        {
            // Instancia um projétil para ataque à distância, direcionado ao alvo determinado
            //Debug.Log(target.name); // Nome do alvo (apenas para debug)
            projectileInstance = Instantiate(projectileprefab, rightHandTransform.position, Quaternion.identity, ArrowParents);
            projectileController = projectileInstance?.GetComponent<ProjectileController>();

            if(target != null)
            {
                projectileController?.SetTarget(tag, target.position, "Enemy"); // Define o alvo do projétil como o jogador
            }

            Destroy(projectileInstance?.gameObject, 10f); // Destruir o projétil após um tempo determinado
        }

        // Chamado pela animação de ataque
        public void desactiveAttack()
        {
            isRangedAttacking = false;
            target = null;
        }
    }

}
