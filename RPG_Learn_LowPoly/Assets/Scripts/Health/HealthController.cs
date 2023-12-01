using RPG.Ally.Detection;
using RPG.Boss.Attack;
using RPG.Character.Detection;
using RPG.Character.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace RPG.Health
{
    public class HealthController : MonoBehaviour
    {
        public event Action OnDeath; // Adiciona um evento que será acionado quando o personagem morrer
        public GameObject deathPanel; // Painel que é ativado quando o personagem morre
        public GameObject artefact; // Artefato que é instanciado quando o personagem morre

        [Header("CharacterData")]
        [SerializeField] private float maxHealth = 100f; // Vida máxima do personagem

        [Header("Other")]
        [SerializeField] private float currentHealth; // Vida atual do personagem
        [SerializeField] private Slider healthSlider; // Slider para exibir a barra de vida

        [Header("effects")]
        [SerializeField] private GameObject healing; // Efeito de cura

        private Animator animator;
        private CharacterDetection characterDetection;
        private AllyDetection allyDetection;
        private Transform attacker;
        private BossAttack bossAttack;

        private int deathHash; // Hash usado para acionar a animação de morte

        public float MaxHealth { set { maxHealth = value; } }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            characterDetection = GetComponent<CharacterDetection>();
            allyDetection = GetComponent<AllyDetection>();
            bossAttack = GetComponent<BossAttack>();
        }

        private void Update()
        {
            // Verifica se a tecla "1" é pressionada e se o objeto é marcado como "Player" para usar uma poção
            if (Input.GetKeyDown(KeyCode.Alpha1) && tag.Equals("Player"))
            {
                usePotion(0.2f);
            }

            // Verifica se a vida está abaixo de 30% da vida máxima e se o objeto é marcado como "Ally" para usar uma poção
            if (currentHealth <= 0.3 * maxHealth && tag.Equals("Ally"))
            {
                usePotion(0.2f);
            }
        }

        private void Start()
        {
            currentHealth = maxHealth; // Inicializa a vida atual com a vida máxima
            deathHash = Animator.StringToHash("TriggerDeath"); // Converte o nome da trigger de morte em um hash
            updateHealthUI(); // Atualiza a barra de vida no início
        }

        public void takeDamage(float damage)
        {
            currentHealth -= damage; // Reduz a vida do personagem com base no dano recebido
            updateHealthUI();

            if (bossAttack != null)
            {
                changeBossStage();
            }

            if (currentHealth <= 0)
            {
                animator.SetTrigger(deathHash);
                // Dispara o evento OnDeath quando o personagem morre
                OnDeath?.Invoke();
            }
        }

        // Sobrecarga da função takeDamage para permitir especificar a tag do atacante
        public void takeDamage(string ownerTag, float damage)
        {
            currentHealth -= damage; // Reduz a vida do personagem com base no dano recebido
            updateHealthUI();

            if (characterDetection)
            {
                attacker = GameObject.FindGameObjectWithTag(ownerTag).transform;

                if (attacker != null) // Faz inimigo reagir a um dano de projétil
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

        // Função para usar uma poção e curar o personagem
        public void usePotion(float value)
        {
            currentHealth += value * maxHealth;
            updateHealthUI();
            carregarEfeito();
        }

        // Função para carregar o efeito de cura
        public void carregarEfeito()
        {
            Instantiate(healing, transform.position, Quaternion.identity, transform);
        }

        // Função para atualizar a barra de vida no UI
        private void updateHealthUI()
        {
            if (healthSlider != null)
            {
                // Atualiza o valor do Slider para refletir a porcentagem de vida restante
                healthSlider.value = currentHealth / maxHealth;
            }
        }

        // Função para mudar a fase do chefe com base na porcentagem de vida
        private void changeBossStage()
        {
            float perHealth = (currentHealth / maxHealth);

            if (perHealth > 0.8 && bossAttack.bossStage != BossAttackStage.Stage01)
            {
                bossAttack.setStage(BossAttackStage.Stage01);
            }
            else if (perHealth <= 0.8 && perHealth > 0.6 && bossAttack.bossStage != BossAttackStage.Stage02)
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
            else if (perHealth <= 0.2 && bossAttack.bossStage != BossAttackStage.Stage05)
            {
                bossAttack.setStage(BossAttackStage.Stage05);
            }
        }

        // Chamado através de animação para destruir o objeto (por exemplo, após a morte do personagem)
        private void destroyObject()
        {
            if (tag.Equals("Player") && deathPanel)
            {
                deathPanel.SetActive(true);
            }
            if (artefact)
            {
                Instantiate(artefact, transform.position, Quaternion.identity, transform.parent);
            }

            Destroy(gameObject);
        }
    }
}
