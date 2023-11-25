using RPG.Ally.Detection;
using RPG.Boss.Attack;
using RPG.Character.Detection;
using RPG.Character.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Health
{
    public class HealthController : MonoBehaviour
    {
        [Header("CharacterData")]
        [SerializeField] private float maxHealth = 100f;

        [Header("Other")]
        [SerializeField] private float currentHealth; 
        [SerializeField] private Slider healthSlider;// Slider para exibir a barra de vida

        [Header("effects")]
        [SerializeField] private GameObject healing;

        private Animator animator;
        private CharacterDetection characterDetection;
        private AllyDetection allyDetection;
        private Transform attacker;
        private BossAttack bossAttack;

        private int deathHash;

        public float MaxHealth { set { maxHealth = value; } }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            characterDetection = GetComponent<CharacterDetection>();
            allyDetection = GetComponent<AllyDetection>();
            bossAttack = GetComponent<BossAttack>();
        }

        private void Start()
        {
            currentHealth = maxHealth;
            deathHash = Animator.StringToHash("TriggerDeath");
            updateHealthUI(); 
        }

        public void takeDamage(float damage)
        {
            currentHealth -= damage; // Reduz a vida do personagem com base no dano recebido
            updateHealthUI();

            if (bossAttack != null)
            {
                changeBossStage();
            }

            if(currentHealth <= 0)
            {
                animator.SetTrigger(deathHash);
            }
        }

        public void takeDamage(string ownerTag, float damage)
        {
            currentHealth -= damage; // Reduz a vida do personagem com base no dano recebido
            updateHealthUI();

            if (characterDetection)
            {
                attacker = GameObject.FindGameObjectWithTag(ownerTag).transform;

                if(attacker != null)//faz inimigo rreagir a um dano de projétil
                    characterDetection.reactToProjectile(attacker);
            }
            
            else if (allyDetection)
            {
                allyDetection.reactToProjectile();
            }

            if (bossAttack != null)
            {
                changeBossStage();
            }

            if (currentHealth <= 0)
            {
                animator.SetTrigger(deathHash);
            }
        }

        public void usePotion(float value){
            currentHealth += value * maxHealth;
            updateHealthUI();
            carregarEfeito();
        }


        public void carregarEfeito()
        {

            // Instanciar o prefab acoplado ao jogador
            GameObject novoObjeto = Instantiate(healing, transform.position, Quaternion.identity);
            novoObjeto.transform.parent = transform;

            StartCoroutine(destroyEffect(novoObjeto));
        }



        IEnumerator destroyEffect(GameObject effect)
        {
            yield return new WaitForSeconds(2.0f);
            Destroy(effect);
        }

        private void updateHealthUI()
        {
            if (healthSlider != null)
            {
                // Atualiza o valor do Slider para refletir a porcentagem de vida restante
                healthSlider.value = currentHealth / maxHealth;
            }
        }

        private void changeBossStage()
        {
            float perHealth = (currentHealth / maxHealth);
            Debug.Log(perHealth);

            if(perHealth > 0.8 && bossAttack.bossStage != BossAttackStage.Stage01)
            {
                bossAttack.setStage(BossAttackStage.Stage01);
            }else if(perHealth <= 0.8 && perHealth > 0.6 && bossAttack.bossStage != BossAttackStage.Stage02)
            {
                bossAttack.setStage(BossAttackStage.Stage02);
            }
            else if (perHealth <= 0.6 && perHealth > 0.4 && bossAttack.bossStage != BossAttackStage.Stage03)
            {
                bossAttack.setStage(BossAttackStage.Stage03);
            }
            else if (perHealth <= 0.4 && perHealth > 0.2 && bossAttack.bossStage != BossAttackStage.Stage04)
            {
                bossAttack.setStage(BossAttackStage.Stage04);
            }
            else if(perHealth <= 0.2 && bossAttack.bossStage != BossAttackStage.Stage05)
            {
                bossAttack.setStage(BossAttackStage.Stage05);
            }
        }

        //Chamado atrav�s de animacao

        private void destroyObject()
        {
            Destroy(gameObject);
        }
    }
}