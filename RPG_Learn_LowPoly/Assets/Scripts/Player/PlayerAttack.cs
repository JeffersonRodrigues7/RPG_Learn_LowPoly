using RPG.Health;
using RPG.Player.Movement;
using RPG.Projectile;
using RPG.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

namespace RPG.Player.Attack
{
    public class PlayerAttack : MonoBehaviour
    {
        #region VARIABLES DECLARATION
        [Header("DATA")]
        [SerializeField] private float swordFirstAttackDamage = 25f;
        [SerializeField] private float swordSecondAttackDamage = 30f;
        [SerializeField] private float swordThirdAttackDamage = 35f;
        [SerializeField] private float swordJumpAttackDamage = 50f;
        [SerializeField] private float projectileDamage = 15f;

        [Header("Objects")]
        [SerializeField] private AnimatorOverrideController swordAnimator;
        [SerializeField] private AnimatorOverrideController bowAnimator;
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private GameObject bowPrefab;
        [SerializeField] private Transform leftHandTransform;
        [SerializeField] private Transform rightHandTransform;
        [SerializeField] private GameObject projectileprefab;
        [SerializeField] private List<string> projectileTagsToExclude = new List<string> { "Weapon", "Detection" };
        
        [Header("Effects")]
        [SerializeField] private GameObject buff;
        [SerializeField] private GameObject starEffect;

        [Header("Debug")]
        [SerializeField] private int actualAttackAnimation = 0;

        private PlayerMovement playerMovement;

        private Animator animator; //Componente animator
        private GameObject weapon;
        private WeaponController weaponController;

        private Transform ArrowParents;
        private GameObject projectileInstance;
        private ProjectileController projectileController = null;

        private bool isUsingSword = false;//�true-> weapon atual � a espada; false -> weapon atua�l � o arco

        [SerializeField] private bool isMeleeAttacking = false; // Flag para determinar se o jogador est� usando o melee attack
        private int meleeAttackingHash; //Hash da String que se refere a anima��o de Melee Attacking

        private bool isRangedAttacking = false; // Flag para determinar se o jogador est� usando o melee attack
        private int rangedAttackingHash; //Hash da String que se refere a anima��o de Melee Attacking

        public bool IsMeleeAttacking { get { return isMeleeAttacking; } }
        public bool IsRangedAttacking { get { return isRangedAttacking; } }

        #endregion
        [SerializeField] private AudioClip swordHitSound;
        [SerializeField] private AudioClip swordDrawSound;
        [SerializeField] private AudioClip bowShotSound;

        private AudioSource audioSource;
        #region  BEGIN/END SCRIPT

        

        private void Awake()
        {
            // Inicializa os componentes e vari�veis necess�rias quando o objeto � criado
            animator = GetComponent<Animator>();
            playerMovement = GetComponent<PlayerMovement>();
            animator.runtimeAnimatorController = swordAnimator;
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Inicializa hashes das strings usadas para controlar anima��es
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

                if (isUsingSword) 
                {
                    weaponController.Damage = swordFirstAttackDamage; 
                    animator.runtimeAnimatorController = swordAnimator;
                    if (swordDrawSound != null)
                    {
                        audioSource.PlayOneShot(swordDrawSound);
                    }
                }
                else
                {
                    animator.runtimeAnimatorController = bowAnimator;
                }
                
            }
        }

        public void carregarEfeito(string efeito)
        {
            GameObject efeitoParaInstanciar;

            if (efeito == "Buff") efeitoParaInstanciar = buff;
            else efeitoParaInstanciar = starEffect;

            // Instanciar o prefab acoplado ao jogador
            GameObject novoObjeto = Instantiate(efeitoParaInstanciar, transform.position, Quaternion.identity);
            novoObjeto.transform.parent = transform;

            StartCoroutine(destroyEffect(novoObjeto));
        }



        IEnumerator destroyEffect(GameObject effect)
        {
            yield return new WaitForSeconds(2.0f);
            Destroy(effect);
        }



        public void increaseSwordAttack(float value){
            swordFirstAttackDamage += value;
            swordSecondAttackDamage += value;
            swordThirdAttackDamage += value;
            swordJumpAttackDamage += value;

            carregarEfeito("Buff");
            


        }

        public void increaseBowAttack(float value){
            projectileDamage += value;

            carregarEfeito("Bow");

        }



        #endregion

        #region FUN��ES DE ANIMA��O

        //Preciso avisar que o player entrou na anima��o de idle para que ele possa ataca novamente
        public void idleBegin()
        {
            isMeleeAttacking = false;
        }

        public void triggerAttack(int value)
        {
            isMeleeAttacking = true;
            actualAttackAnimation = value;
            if(value == 0 || value == 1) weaponController.Damage = swordFirstAttackDamage;
            else if (value == 2) weaponController.Damage = swordSecondAttackDamage;
            else if (value == 3) weaponController.Damage = swordThirdAttackDamage;
            else if (value == 4) weaponController.Damage = swordJumpAttackDamage;
        }

        public void desactiveTriggerAttack(int value)
        {
            actualAttackAnimation = value;
        }

        // Ativar ataque - Chamado pela anima��o de ataque
        public void activeAttack()
        {   
            // Configurar a vari�vel de controle de ataque da arma
            weaponController.IsAttacking = true;

            if (isUsingSword && swordHitSound != null)
            {
                audioSource.PlayOneShot(swordHitSound);
            } 

            
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
                    projectileInstance = Instantiate(projectileprefab, rightHandTransform.position, Quaternion.identity, ArrowParents);
                    projectileController = projectileInstance?.GetComponent<ProjectileController>();
                    projectileController?.SetTarget(tag, hit.point, "Enemy"); // Define o alvo do proj�til como o jogador
                    projectileController.Damage = projectileDamage;
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
            projectileInstance = Instantiate(projectileprefab, rightHandTransform.position, Quaternion.identity, ArrowParents);
            projectileController = projectileInstance?.GetComponent<ProjectileController>();
            projectileController?.SetTarget(tag, worldMousePosition, "Enemy"); // Define o alvo do proj�til como o jogador
            Destroy(projectileInstance.gameObject, 10f);
        }

        // Desativar ataque - Chamado pela anima��o de ataque
        public void desactiveAttack()
        {
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

                if (playerMovement.IsJumping)
                {
                    animator.SetTrigger("TriggerJumpAttack");
                }

                else
                {
                    if ((actualAttackAnimation == 0 || actualAttackAnimation == 1) && !isMeleeAttacking)
                    {
                        //Debug.Log($"Trigando primeiro ataque: {actualAttackAnimation} - {isMeleeAttacking}");
                        animator.SetTrigger("TriggerAttack01");
                    }
                    else if (actualAttackAnimation == 1)
                    {
                        animator.SetTrigger("TriggerAttack02");
                    }
                    else if (actualAttackAnimation == 2)
                    {
                        animator.SetTrigger("TriggerAttack03");
                    }
                }
         

            }
            else
            {
                animator.SetTrigger(rangedAttackingHash);
                isRangedAttacking = true;
                if (!audioSource.isPlaying && bowShotSound != null)
                {
                    audioSource.PlayOneShot(bowShotSound);
                }
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
