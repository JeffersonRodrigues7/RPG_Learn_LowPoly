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
        [SerializeField] private Transform leftHandTransform; // Transform do ponto onde a arma ser� anexada
        [SerializeField] private Transform rightHandTransform; // Transform do ponto onde a arma ser� anexada
        [SerializeField] private GameObject projectileprefab;
        
        private Transform ArrowParents;
        private GameObject projectileInstance;
        private ProjectileController projectileController = null;

        private Animator animator;
        private GameObject weapon;
        private WeaponController weaponController; // Controlador da arma

        private Transform target;

        private bool isRangedAttacking = false; // Flag para determinar se o jogador est� usando o melee attack
        private int rangedAttackingHash; //Hash da String que se refere a anima��o de Melee Attacking

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
            weaponController = weapon.GetComponent<WeaponController>(); // Obt�m o controlador da arma
            weaponController.EnemyTag = "Enemy"; // Define a tag do inimigo
        }

        public void startAttackAnimation(Transform _target)
        {
            animator.SetTrigger(rangedAttackingHash); // Inicia a anima��o de ataque � dist�ncia
            target = _target; // Define o alvo do ataque � dist�ncia
        }

        // Chamado pela anima��o de ataque
        public void shootArrow()
        {
            // Instancia um proj�til para ataque � dist�ncia, direcionado ao alvo determinado
            //Debug.Log(target.name); // Nome do alvo (apenas para debug)
            projectileInstance = Instantiate(projectileprefab, rightHandTransform.position, Quaternion.identity, ArrowParents);
            projectileController = projectileInstance?.GetComponent<ProjectileController>();

            if(target != null)
            {
                projectileController?.SetTarget(tag, target.position, "Enemy"); // Define o alvo do proj�til como o jogador
            }

            Destroy(projectileInstance?.gameObject, 10f); // Destruir o proj�til ap�s um tempo determinado
        }

        // Chamado pela anima��o de ataque
        public void desactiveAttack()
        {
            isRangedAttacking = false;
            target = null;
        }
    }

}
