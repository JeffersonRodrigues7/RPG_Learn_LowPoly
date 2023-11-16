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
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private GameObject bowPrefab;
        [SerializeField] private Transform leftHandTransform;
        [SerializeField] private Transform rightHandTransform;
        [SerializeField] private ProjectileController projectileController = null;
        [SerializeField] private List<string> projectileTagsToExclude = new List<string> { "Weapon", "Detection" };
        [SerializeField] private Transform ArrowParents;

        private Animator animator; //Componente animator
        private GameObject weapon;
        private WeaponController weaponController;

        ProjectileController projectileInstance;

        private bool isUsingSword = false;//�true-> weapon atual � a espada; false -> weapon atua�l � o arco

        private bool isMeleeAttacking = false; // Flag para determinar se o jogador est� usando o melee attack
        private int meleeAttackingHash; //Hash da String que se refere a anima��o de Melee Attacking

        private bool isRangedAttacking = false; // Flag para determinar se o jogador est� usando o melee attack
        private int rangedAttackingHash; //Hash da String que se refere a anima��o de Melee Attacking

        public bool IsMeleeAttacking { get { return isMeleeAttacking; } }
        public bool IsRangedAttacking { get { return isRangedAttacking; } }

        #endregion

        #region  BEGIN/END SCRIPT

        private void Awake()
        {
            // Inicializa os componentes e vari�veis necess�rias quando o objeto � criado
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            // Inicializa hashes das strings usadas para controlar anima��es
            projectileTagsToExclude.Add(gameObject.tag);
            meleeAttackingHash = Animator.StringToHash("TriggerMeleeAttack");
            rangedAttackingHash = Animator.StringToHash("TriggerRangedAttack");

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
            }
        }
        #endregion

        #region FUN��ES DE ANIMA��O

        // Ativar ataque - Chamado pela anima��o de ataque
        public void activeAttack()
        {
            // Configurar a vari�vel de controle de ataque da arma
            weaponController.IsAttacking = true;
        }

        // Atirar flecha - Chamado pela anima��o de ataque � dist�ncia
        public void shootArrow()
        {
            // Realiza um raycast para identificar um alvo
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

            foreach (RaycastHit hit in hits)
            {
                // Se o alvo n�o estiver em uma tag exclu�da, atira na dire��o identificada
                if (!projectileTagsToExclude.Contains(hit.collider.tag))
                {
                    // Instancia um proj�til e define seu alvo, destruindo-o ap�s um tempo
                    projectileInstance = Instantiate(projectileController, rightHandTransform.position, Quaternion.identity, ArrowParents);
                    projectileInstance.SetTarget(hit.point, "Enemy");
                    Destroy(projectileInstance.gameObject, 10f);
                    return;
                }
            }


            
            // Se nenhum alvo foi identificado pelo raycast, atira na dire��o do mouse
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 100;
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            //Debug.Log($"Atirando aqui {mousePosition}");
            // Instancia um proj�til e define seu alvo como a posi��o do mouse, destruindo-o ap�s um tempo
            projectileInstance = Instantiate(projectileController, rightHandTransform.position, Quaternion.identity, ArrowParents);
            projectileInstance.SetTarget(worldMousePosition, "Enemy");
            Destroy(projectileInstance.gameObject, 10f);
        }

        // Desativar ataque - Chamado pela anima��o de ataque
        public void desactiveAttack()
        {
            // Desativa flags de ataque
            isMeleeAttacking = false;
            isRangedAttacking = false;
            weaponController.IsAttacking = false;
        }

        #endregion

        #region CALLBACKS DE INPUT

        // Inicia anima��o de ataque
        public void Attack(InputAction.CallbackContext context)
        {
            // Inicia o ataque, com base no tipo de arma sendo usada
            if (isUsingSword)
            {
                animator.SetTrigger(meleeAttackingHash);
                isMeleeAttacking = true;
            }
            else
            {
                animator.SetTrigger(rangedAttackingHash);
                isRangedAttacking = true;
            }
        }

        // Chamado quando soltamos o bot�o de ataque
        public void StopAttack(InputAction.CallbackContext context)
        {
            // L�gica para encerrar o ataque (se necess�rio)
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

        // Chamado quando soltamos o bot�o de troca de arma
        public void StopChangeWeapon(InputAction.CallbackContext context)
        {
            // L�gica para encerrar a troca de arma (se necess�rio)
        }

        #endregion

    }

}
