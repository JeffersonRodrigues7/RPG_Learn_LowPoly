using RPG.Projectile;
using RPG.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Ally.Attack
{
    // Classe responsável pelos ataques do aliado
    public class AllyAttack : MonoBehaviour
    {
        // Variáveis de dano para espada e projétil
        [Header("DATA")]
        [SerializeField] private float swordDamage = 25f;
        [SerializeField] private float projectileDamage = 15f;

        // Prefabs e posições para arma e projétil
        [SerializeField] private GameObject bowPrefab;
        [SerializeField] private Transform leftHandTransform; // Transform do ponto onde a arma será anexada
        [SerializeField] private Transform rightHandTransform; // Transform do ponto onde a arma será anexada
        [SerializeField] private GameObject projectileprefab;
        [SerializeField] private AudioClip hitComArma;

        // Componentes de áudio
        private AudioSource audioSource;

        // Transform para agrupar as setas
        private Transform ArrowParents;

        // Instâncias de arma e controladores
        private GameObject projectileInstance;
        private ProjectileController projectileController = null;

        private Animator animator;
        private GameObject weapon;
        private WeaponController weaponController; // Controlador da arma

        private Transform target;

        // Flags e hash para controle de ataques
        private bool isRangedAttacking = false; // Flag para determinar se o jogador está usando o melee attack
        private int rangedAttackingHash; //Hash da String que se refere à animação de Melee Attacking

        // Propriedade para acessar isRangedAttacking de fora da classe
        public bool IsRangedAttacking { get { return isRangedAttacking; } set { isRangedAttacking = value; } }

        // Awake é chamado quando o script é iniciado
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        // Start é chamado no primeiro frame
        private void Start()
        {
            // Configuração do áudio
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Configuração do animator e hash da animação de ataque à distância
            animator = GetComponent<Animator>();
            rangedAttackingHash = Animator.StringToHash("TriggerRangedAttack");

            // Encontrar o transform para agrupar as setas
            ArrowParents = GameObject.Find("ArrowsParent").transform;

            // Instanciar a arma
            spawnWeapon(bowPrefab, leftHandTransform);
        }

        // Método para instanciar a arma
        private void spawnWeapon(GameObject weaponPrefab, Transform hand)
        {
            // Instancia a arma no ponto especificado
            weapon = Instantiate(weaponPrefab, hand);
            // Obtém o controlador da arma
            weaponController = weapon.GetComponent<WeaponController>();
            // Define a tag do inimigo
            weaponController.EnemyTag = "Enemy";
        }

        // Método para iniciar a animação de ataque
        public void startAttackAnimation(Transform _target)
        {
            // Reproduzir som de ataque se estiver disponível
            if (hitComArma != null)
            {
                audioSource.PlayOneShot(hitComArma);
            }
            // Inicia a animação de ataque à distância
            animator.SetTrigger(rangedAttackingHash);
            // Define o alvo do ataque à distância
            target = _target;
        }

        // Chamado pela animação de ataque para disparar a seta
        public void shootArrow()
        {
            // Instancia o projétil na mão direita
            projectileInstance = Instantiate(projectileprefab, rightHandTransform.position, Quaternion.identity, ArrowParents);
            // Obtém o controlador do projétil
            projectileController = projectileInstance?.GetComponent<ProjectileController>();

            // Se houver um alvo, define o alvo do projétil como o jogador
            if (target != null)
            {
                projectileController?.SetTarget(tag, target.position + new Vector3(0, 1, 0), "Enemy");
                // Define o dano do projétil
                projectileController.Damage = projectileDamage;
            }

            // Destruir o projétil após um tempo determinado
            Destroy(projectileInstance?.gameObject, 10f);
        }

        // Chamado pela animação de ataque para desativar o ataque
        public void desactiveAttack()
        {
            isRangedAttacking = false;
            target = null;
        }
    }
}
