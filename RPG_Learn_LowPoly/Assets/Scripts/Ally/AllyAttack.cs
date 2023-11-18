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
        [SerializeField] private ProjectileController projectileController = null;
        
        private Transform ArrowParents;

        private Animator animator;
        private GameObject weapon;
        private WeaponController weaponController; // Controlador da arma

        private Transform target;

        private bool isRangedAttacking = false; // Flag para determinar se o jogador está usando o melee attack
        private int rangedAttackingHash; //Hash da String que se refere a animação de Melee Attacking

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
            // Inicia a animação de ataque, com base no tipo de arma e estado de ataque do jogador
            if (!isRangedAttacking) // Se não estiver usando espada e não estiver atacando à distância
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
            ProjectileController projectileInstance = Instantiate(projectileController, rightHandTransform.position, Quaternion.identity, ArrowParents);
            projectileInstance.SetTarget(target.position, "Enemy"); // Define o alvo do projétil como o jogador
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
            isRangedAttacking = false;
            target = null;
        }
    }

}
