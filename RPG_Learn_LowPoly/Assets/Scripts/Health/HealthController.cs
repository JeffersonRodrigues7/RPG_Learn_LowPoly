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
        [SerializeField] private Slider healthSlider; // Slider para exibir a barra de vida

        private Animator animator;
        private CharacterDetection characterDetection;
        private Transform attacker;

        private int deathHash;

        public float MaxHealth { set { maxHealth = value; } }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            characterDetection = GetComponentInChildren<CharacterDetection>();
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

                if(attacker != null)
                    characterDetection.reactToProjectile(attacker);
            }

            if (currentHealth <= 0)
            {
                animator.SetTrigger(deathHash);
            }
        }

        private void updateHealthUI()
        {
            if (healthSlider != null)
            {
                // Atualiza o valor do Slider para refletir a porcentagem de vida restante
                healthSlider.value = currentHealth / maxHealth;
            }
        }

        //Chamado através de animacao

        private void destroyObject()
        {
            Destroy(gameObject);
        }
    }
}