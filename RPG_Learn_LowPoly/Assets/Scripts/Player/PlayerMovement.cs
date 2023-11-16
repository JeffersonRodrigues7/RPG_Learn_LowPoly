using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using RPG.Player.Attack;
using System;

namespace RPG.Player.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        #region VARIABLES DECLARATION

        [Header("Movimentação")]
        [SerializeField] private float walkSpeed = 5.0f; //Velocidade do jogador ao andar
        [SerializeField] private float runSpeed = 10.0f; //Velocidade do jogador ao correr
        [SerializeField] private float rotationSpeed = 10.0f; //Velocidade de rotação do jogador

        [SerializeField] private float jumpForce = 2.0f; //Força com a qual o objeto irá pular
        [SerializeField] private float jumpSlowdown = 1.0f; //Valor com o qual o player vai perdendo a velocidade no pulo

        [SerializeField] private float mouseInputDistance = 50.0f; //Distância com a qual o jogador consegue interagir com o mapa através do mouse

        private Animator animator; //Componente animator
        private Rigidbody rb; //Componente rigidibody
        private NavMeshAgent navMeshAgent; //Componente navMeshAgent
        private PlayerAttack playerAttack;

        private Camera cam; //Camera principal do jogo

        private Ray ray;
        private Vector3 movementPosition;  //Posição para qual o player irá se mover

        private Vector3 mousePosition;
        private Vector3 wantedMousePosition;

        private bool isWalking = false; //Flag que indica que o objetando está se movendo
        private float currentSpeed = 3.0f; //Velocidade atual do jogador
        private float currentjumpVelocity = 0; //Velocidade do objeto atual nop ar
        private bool navMeshRemainingPath = false; // Flag que indica se o objeto deve continuar a se movimentar com o navmesh

        private bool isKeyboardMoving = false;  // Flag para determinar se o jogador está andando via teclado
        private bool isMouseMoving = false;  // Flag para determinar se o jogador está andando via mouse
        private int isWalkingHash; //Hash da String que se refere a animação de Walk

        private bool isRunning = false; // Flag para determinar se o jogador está correndo
        private int isRunningHash; //Hash da String que se refere a animação de Running

        private bool isJumping = false; // Flag para determinar se o jogador está pulando
        private bool isFalling = false; // Flag para determinar se o jogador está caindo
        private int isJumpingHash; //Hash da String que se refere a animação de Jumping

        #endregion

        #region  BEGIN/END SCRIPT

        private void Awake()
        {
            // Inicializa os componentes e variáveis necessárias quando o objeto é criado
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            playerAttack = GetComponent<PlayerAttack>();
        }

        private void Start()
        {
            // Inicializa hashes das strings usadas para controlar animações e obtém a câmera principal do jogo
            isWalkingHash = Animator.StringToHash("isWalking");
            isRunningHash = Animator.StringToHash("isRunning");
            isJumpingHash = Animator.StringToHash("TriggerJump");
            cam = Camera.main;
        }

        #endregion

        #region  UPDATES 

        private void Update()
        {
            if (playerAttack.IsRangedAttacking) // Se o jogador está atacando à distância
            {
                mousePosition = Input.mousePosition;
                wantedMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 100f));
                // Determina a direção para onde o jogador deve olhar
                Vector3 direction = wantedMousePosition - transform.position;

                direction.y = 0; // Define o componente y como zero para restringir ao plano XZ

                // Calcula a rotação do jogador para olhar na direção determinada
                Quaternion novaRotacao = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Euler(0, novaRotacao.eulerAngles.y, 0); // Ajusta a rotação do jogador apenas nos eixos X e Z
            }
            else
            {
                updateMoveParameters();
            }

        }

        private void FixedUpdate()
        {
            if (playerAttack.IsRangedAttacking) //Se o player está atirando, então ele não pode se mover
            {
                return;
            }

            currentSpeed = isRunning ? runSpeed : walkSpeed;

            if (isJumping) //Player está pulando
            {
                currentjumpVelocity -= jumpForce * jumpSlowdown * Time.fixedDeltaTime; //Diminuindo a velocidade do player no ar
                if (currentjumpVelocity < 0) isFalling = true; //Player começa a cair
            }

            if (isMouseMoving) //Se estivermos nos movendo via mouse
            {
                doMouseMovimentation();
            }

            else if (isKeyboardMoving || isJumping) //Se estivermos no movendo via teclado
            {
                doKeyboardMovimentation();
            }
        }

        #endregion

        #region  MOVIMENTAÇÃO 

        //Faz movimentação via Mouse
        private void doMouseMovimentation()
        {

            // Cria um raio a partir da posição do mouse convertida para o espaço da tela.
            ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            // Realiza um raio para detectar colisões no mundo e armazena as informações na variável 'hit'.
            if (Physics.Raycast(ray, out RaycastHit hit, mouseInputDistance))
            {
                movementPosition = hit.point; // Define a posição de movimento com base no ponto onde o raio colidiu com o objeto.
            }

            MoveToPosition(movementPosition); // Chama a função 'MoveToPosition' para mover o jogador até a nova posição definida.

            navMeshRemainingPath = true; //Indica que temos um caminho a percorrer 

            if (navMeshAgent.enabled)
            {
                navMeshAgent.speed = currentSpeed; // Atualiza a velocidade do NavMeshAgent para a velocidade atual (corrida ou caminhada).
            }
        }

        //Faz movimentação via teclado
        private void doKeyboardMovimentation()
        {
            // Cria um vetor de direção a partir da posição de movimento do jogador, com a componente y zerada, e normaliza-o.
            Vector3 movementInputDirection = new Vector3(movementPosition.x, 0.0f, movementPosition.y).normalized;

            // Transforma a direção do movimento no espaço da câmera
            Vector3 cameraForward = cam.transform.forward;
            Vector3 cameraRight = cam.transform.right;

            // Define as componentes y de cameraForward e cameraRight como zero para manter o movimento no plano horizontal.
            cameraForward.y = 0;
            cameraRight.y = 0;

            // Normaliza os vetores da câmera para garantir que tenham comprimento igual a 1.
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calcula a direção de movimento desejada no espaço da câmera.
            Vector3 desiredMoveDirection = cameraForward * movementInputDirection.z + cameraRight * movementInputDirection.x;

            // Define a velocidade do Rigidbody com base na direção de movimento desejada e na velocidade atual.
            rb.velocity = new Vector3(desiredMoveDirection.x * currentSpeed, currentjumpVelocity, desiredMoveDirection.z * currentSpeed) ;

            // Se houver uma direção de movimento (diferente de zero), realiza a rotação do jogador.
            if (movementInputDirection != Vector3.zero)
            {
                // Calcula uma nova rotação com base na direção de movimento desejada.
                Quaternion newRotation = Quaternion.LookRotation(desiredMoveDirection);

                // Aplica uma rotação suave (Slerp) do jogador em direção à nova rotação.
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.fixedDeltaTime);

            }
        }

        //Função responsável por cuidar da animação do jogador
        private void updateMoveParameters()
        {
            //Verifica se o player chegou na posição via navmesh
            if (navMeshAgent.enabled && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                navMeshAgent.ResetPath();
                navMeshRemainingPath = false;
            }

            isWalking = isKeyboardMoving || isMouseMoving || navMeshRemainingPath;

            if (isWalking && isRunning) //Correndo
            {
                updateMoveAnimation(true, true);
            }
            else if (isWalking) //Andando
            {
                updateMoveAnimation(true, false);
            }
            else //Parado
            {
                updateMoveAnimation(false, false);
            }
        }

        //Atualização animação de mvimento
        private void updateMoveAnimation(bool isWalking, bool isRunning)
        {
            animator.SetBool(isWalkingHash, isWalking);//Para que ele chegue na animação de correr, é necessário que esteja andando
            animator.SetBool(isRunningHash, isRunning);
        }

        // Move o jogador para a posição de destino usando o NavMeshAgent
        private void MoveToPosition(Vector3 destination)
        {
            if (navMeshAgent.enabled)
            {
                navMeshAgent.SetDestination(destination);
            }
        }

        #endregion

        #region  FUNÇÕES DE ANIMAÇÃO

        //Função chamada a partir de um determinado frame da animação de Jump
        public void startJump()
        {
            //Setando valores iniciais do pulo
            currentjumpVelocity = jumpForce;
            navMeshAgent.enabled = false;
            isFalling = false;
            isJumping = true;
        }

        #endregion

        #region  COLISÕES

        //Detecta quando o personagem toca no chão
        private void OnCollisionEnter(Collision collision)
        {
            if (isFalling && collision.gameObject.CompareTag("Ground"))  // Para que haja a detecção o player precisa estar caindo, caso contrário ele pode detectar quando o player está tentando pular
            {
                //Reseta valores
                currentjumpVelocity = 0;
                isJumping = false;
                isFalling = false;
                navMeshAgent.enabled = true;
            }
        }

        #endregion

        #region  CALLBACKS DE INPUT 

        /**Callback que reseta o caminho do NavMeshAgent, ativa a flag de movimento via teclado e atualiza a posição de movimento, 
         * Keyboard movemente tem prioridade sobre o MouseMovement*/
        public void KeyboardMove(InputAction.CallbackContext context)
        {
            //Debug.Log("TESTE");
            if (navMeshAgent.enabled)
            {
                navMeshAgent.ResetPath();
                navMeshAgent.enabled = false; //Quando o navmesh fica ligado enquanto nos movemos com o mouse, aparece o efeito de jitter
            }

            isMouseMoving = false;
            isKeyboardMoving = true;
            movementPosition = context.ReadValue<Vector2>();
        }

        // Desativa a flag de movimento via teclado, reseta a posição de movimento e para a animação de corrida
        public void StopKeyboardMove(InputAction.CallbackContext context)
        {
            isKeyboardMoving = false;
            movementPosition = Vector2.zero;
            navMeshAgent.enabled = true;//Habilita navmesh novamente
        }

        // Ativa a flag de movimento via mouse e a animação de caminhar
        public void MouseMove(InputAction.CallbackContext context)
        {
            if (!isKeyboardMoving)
            {
                isMouseMoving = true;
            }
        }

        // Desativa a flag de movimento via mouse, reseta a posição de movimento e para a animação de corrida
        public void StopMouseMove(InputAction.CallbackContext context)
        {
            if (isMouseMoving)
            {
                isMouseMoving = false;
                movementPosition = Vector2.zero;
            }
        }

        // Define a flag de corrida como verdadeira e inicia a animação de corrida
        public void Run(InputAction.CallbackContext context)
        {
            isRunning = true;
        }

        // Para a animação de corrida, chamado quando usuário solta o shift ou quando para de andar
        public void StopRun(InputAction.CallbackContext context)
        {
            isRunning = false;
        }

        //Inicia pulo
        public void Jump(InputAction.CallbackContext context)
        {
            if (!isJumping)
            {
                animator.SetTrigger(isJumpingHash);
            }
        }

        // Chamado quando soltamos o botão de pulo
        public void StopJump(InputAction.CallbackContext context)
        {

        }
        #endregion
    }
}

