using System.Collections;
using System.Collections.Generic;
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

        private int deathHash;

        public float MaxHealth { set { maxHealth = value; } }

        private void Awake()
        {
            animator = GetComponent<Animator>();
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

            Debug.Log($"{gameObject.name}, DANO LEVADO = {currentHealth}");

            if(currentHealth <= 0)
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