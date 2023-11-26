using RPG.Player.Attack;
using RPG.Player.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RPG.Player.InputController
{
    public class PlayerInputController : MonoBehaviour
    {

        private PlayerInput playerInput; //Componente playerInput
        private PlayerMovement playerMovement;
        private PlayerAttack playerAttack;

        public GameObject inventory;
        public InventoryManager inventoryManager;

        private void Awake()
        {
            // Inicializa os componentes e vari�veis necess�rias quando o objeto � criado
            playerInput = new PlayerInput();
            playerMovement = GetComponent<PlayerMovement>();
            playerAttack = GetComponent<PlayerAttack>();


        }

        private void OnEnable()
        {
            // Ativa o registro de inputs do jogador
            enablePlayerInputs();
        }

        private void OnDisable()
        {
            // Desativa o registro de inputs do jogador
            disablePlayerInputs();
        }

        // Registra callbacks para as entradas
        private void enablePlayerInputs()
        {
            playerInput.Enable();
            playerInput.CharacterControls.KeyboardWalk.performed += playerMovement.KeyboardMove; // Callback de entrada de movimento via teclado
            playerInput.CharacterControls.KeyboardWalk.canceled += playerMovement.StopKeyboardMove; // Callback para parar o movimento via teclado
            playerInput.CharacterControls.MouseWalk.performed += playerMovement.MouseMove; // Callback de entrada de movimento via Mouse
            playerInput.CharacterControls.MouseWalk.canceled += playerMovement.StopMouseMove; // Callback para parar o movimento via Mouse
            playerInput.CharacterControls.Run.started += playerMovement.Run; // Callback para iniciar a corrida
            playerInput.CharacterControls.Run.canceled += playerMovement.StopRun; // Callback para parar a corrida
            playerInput.CharacterControls.Jump.started += playerMovement.Jump; // Callback para iniciar a Pulo
            playerInput.CharacterControls.Jump.canceled += playerMovement.StopJump; // Callback para indicar que soltamos bot�o de pulo
            playerInput.CharacterControls.Roll.started += playerMovement.Roll; // Callback para iniciar a troca de arma
            playerInput.CharacterControls.Roll.canceled += playerMovement.StopRoll; // Callback para indicar que soltamos bot�o de troca de arma

            playerInput.CharacterControls.Attack.started += playerAttack.Attack; // Callback para iniciar o Melee Attack
            playerInput.CharacterControls.Attack.canceled += playerAttack.StopAttack; // Callback para indicar que soltamos bot�o de melee attack
            playerInput.CharacterControls.ChangeWeapon.started += playerAttack.ChangeWeapon; // Callback para iniciar a troca de arma
            playerInput.CharacterControls.ChangeWeapon.canceled += playerAttack.StopChangeWeapon; // Callback para indicar que soltamos bot�o de troca de arma

            playerInput.CharacterControls.Inventory.started += openInventory;
        }

        // Cancela o registro de callbacks quando o script � desativado
        private void disablePlayerInputs()
        {
            playerInput.Disable();
            playerInput.CharacterControls.KeyboardWalk.performed -= playerMovement.KeyboardMove;
            playerInput.CharacterControls.KeyboardWalk.canceled -= playerMovement.StopKeyboardMove;
            playerInput.CharacterControls.MouseWalk.performed -= playerMovement.MouseMove;
            playerInput.CharacterControls.MouseWalk.canceled -= playerMovement.StopMouseMove;
            playerInput.CharacterControls.Run.started -= playerMovement.Run;
            playerInput.CharacterControls.Run.canceled -= playerMovement.StopRun;
            playerInput.CharacterControls.Jump.started -= playerMovement.Jump;
            playerInput.CharacterControls.Jump.canceled -= playerMovement.StopJump;
            playerInput.CharacterControls.Roll.started -= playerMovement.Roll; 
            playerInput.CharacterControls.Roll.canceled -= playerMovement.StopRoll; 

            playerInput.CharacterControls.Attack.started -= playerAttack.Attack;
            playerInput.CharacterControls.Attack.canceled -= playerAttack.StopAttack;
            playerInput.CharacterControls.ChangeWeapon.started -= playerAttack.ChangeWeapon; 
            playerInput.CharacterControls.ChangeWeapon.canceled -= playerAttack.StopChangeWeapon;

            playerInput.CharacterControls.Inventory.started -= openInventory;

        }


        public void openInventory(InputAction.CallbackContext context)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
            if (!inventory.active)
            {
                inventory.SetActive(true);
                inventoryManager.ListItems();


            }
            else
            {
                inventory.SetActive(false);
            }

        }
    }

    

}
