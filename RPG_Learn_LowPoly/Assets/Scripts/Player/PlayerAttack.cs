using RPG.Health;
using RPG.Projectile;
using RPG.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

namespace RPG.Player.Attack
{
    public class PlayerAttack : MonoBehaviour
    {
        #region VARIABLES DECLARATION
        [Header("DATA")]
        [SerializeField] private float swordDamage = 25f;
        [SerializeField] private float projectileDamage = 15f;

        [Header("Objects")]
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private GameObject bowPrefab;
        [SerializeField] private Transform leftHandTransform;
        [SerializeField] private Transform rightHandTransform;
        [SerializeField] private GameObject projectileprefab;
        [SerializeField] private List<string> projectileTagsToExclude = new List<string> { "Weapon", "Detection" };

        [Header("Debug")]
        [SerializeField] private int actualAttackAnimation = 0;

        private Animator animator; //Componente animator
        private GameObject weapon;
        private WeaponController weaponController;

        private Transform ArrowParents;
        private GameObject projectileInstance;
        private ProjectileController projectileController = null;

        private bool isUsingSword = false;//´true-> weapon atual é a espada; false -> weapon atua´l é o arco

        private bool isMeleeAttacking = false; // Flag para determinar se o jogador está usando o melee attack
        private int meleeAttackingHash; //Hash da String que se refere a animação de Melee Attacking

        private bool isRangedAttacking = false; // Flag para determinar se o jogador está usando o melee attack
        private int rangedAttackingHash; //Hash da String que se refere a animação de Melee Attacking

        public bool IsMeleeAttacking { get { return isMeleeAttacking; } }
        public bool IsRangedAttacking { get { return isRangedAttacking; } }

        #endregion

        #region  BEGIN/END SCRIPT

        private void Awake()
        {
            // Inicializa os componentes e variáveis necessárias quando o objeto é criado
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            // Inicializa hashes das strings usadas para controlar animações
            projectileTagsToExclude.Add(gameObject.tag);
            meleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack");
            rangedAttackingHash = Animator.StringToHash("TriggerRangedAttack");

            ArrowParents = GameObject.Find("ArrowsParent").transform;

            spawnWeapon(swordPrefab, rightHandTransform);
        }

        private void spawnWeapon(GameObject weaponPrefab, Transform hand)
        {
            // Alternar entre o uso de espada e arco
            isUsingSword = !isUsingSword;

            // Destruir arma atual se existir
            if (weapon != null)
            {
                Destroy(weapon);
            }

            // Instanciar e configurar a nova arma
            if (weaponPrefab != null)
            {
                weapon = Instantiate(weaponPrefab, hand);
                weaponController = weapon.GetComponent<WeaponController>();
                weaponController.EnemyTag = "Enemy";

                if (isUsingSword) weaponController.Damage = swordDamage;
            }
        }
        #endregion

        #region FUNÇÕES DE ANIMAÇÃO

        public void triggerAttack(int value)
        {
            actualAttackAnimation = value;
        }

        // Ativar ataque - Chamado pela animação de ataque
        public void activeAttack()
        {
            // Configurar a variável de controle de ataque da arma
            weaponController.IsAttacking = true;
        }

        // Atirar flecha - Chamado pela animação de ataque à distância
        public void shootArrow()
        {
            // Realiza um raycast para identificar um alvo
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

            foreach (RaycastHit hit in hits)
            {
                // Se o alvo não estiver em uma tag excluída, atira na direção identificada
                if (!projectileTagsToExclude.Contains(hit.collider.tag))
                {
                    // Instancia um projétil e define seu alvo, destruindo-o após um tempo
                    projectileInstance = Instantiate(projectileprefab, rightHandTransform.position, Quaternion.identity, ArrowParents);
                    projectileController = projectileInstance?.GetComponent<ProjectileController>();
                    projectileController?.SetTarget(tag, hit.point, "Enemy"); // Define o alvo do projétil como o jogador
                    projectileController.Damage = projectileDamage;
                    Destroy(projectileInstance.gameObject, 10f);
                    return;
                }
            }


            
            // Se nenhum alvo foi identificado pelo raycast, atira na direção do mouse
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 100;
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            //Debug.Log($"Atirando aqui {mousePosition}");
            // Instancia um projétil e define seu alvo como a posição do mouse, destruindo-o após um tempo
            projectileInstance = Instantiate(projectileprefab, rightHandTransform.position, Quaternion.identity, ArrowParents);
            projectileController = projectileInstance?.GetComponent<ProjectileController>();
            projectileController?.SetTarget(tag, worldMousePosition, "Enemy"); // Define o alvo do projétil como o jogador
            Destroy(projectileInstance.gameObject, 10f);
        }

        // Desativar ataque - Chamado pela animação de ataque
        public void desactiveAttack()
        {
            // Desativa flags de ataque
            actualAttackAnimation = 0;
            isMeleeAttacking = false;
            isRangedAttacking = false;
            weaponController.IsAttacking = false;
        }

        #endregion

        #region CALLBACKS DE INPUT

        // Inicia animação de ataque
        public void Attack(InputAction.CallbackContext context)
        {
            // Inicia o ataque, com base no tipo de arma sendo usada
            if (isUsingSword)
            {
                //animator.SetTrigger(meleeAttackingHash);
                //isMeleeAttacking = true;

                if(actualAttackAnimation == 0)
                {
                    animator.SetTrigger("TriggerAttack01");
                }else if(actualAttackAnimation == 1)
                {
                    animator.SetTrigger("TriggerAttack02");
                }
                else if (actualAttackAnimation == 2)
                {
                    animator.SetTrigger("TriggerAttack03");
                }
            }
            else
            {
                animator.SetTrigger(rangedAttackingHash);
                isRangedAttacking = true;
            }
        }

        // Chamado quando soltamos o botão de ataque
        public void StopAttack(InputAction.CallbackContext context)
        {
            // Lógica para encerrar o ataque (se necessário)
        }

        // Inicia a troca de arma
        public void ChangeWeapon(InputAction.CallbackContext context)
        {
            // Troca entre espada e arco
            if (isUsingSword)
            {
                spawnWeapon(bowPrefab, leftHandTransform);
            }
            else
            {
                spawnWeapon(swordPrefab, rightHandTransform);
            }
        }

        // Chamado quando soltamos o botão de troca de arma
        public void StopChangeWeapon(InputAction.CallbackContext context)
        {
            // Lógica para encerrar a troca de arma (se necessário)
        }

        #endregion

    }

}
