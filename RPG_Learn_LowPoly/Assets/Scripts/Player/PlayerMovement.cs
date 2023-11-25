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

        [Header("Movimenta��o")]
        [SerializeField] private float attackingSpeed = 2.0f; //Velocidade do jogador ao andar
        [SerializeField] private float walkSpeed = 5.0f; //Velocidade do jogador ao andar
        [SerializeField] private float runSpeed = 10.0f; //Velocidade do jogador ao correr
        [SerializeField] private float rollingSpeed = 15.0f; //Velocidade do jogador ao correr
        [SerializeField] private float rotationSpeed = 10.0f; //Velocidade de rota��o do jogador

        [SerializeField] private float jumpForce = 2.0f; //For�a com a qual o objeto ir� pular
        [SerializeField] private float jumpSlowdown = 1.0f; //Valor com o qual o player vai perdendo a velocidade no pulo

        [SerializeField] private float mouseInputDistance = 50.0f; //Dist�ncia com a qual o jogador consegue interagir com o mapa atrav�s do mouse

        private Animator animator; //Componente animator
        private Rigidbody rb; //Componente rigidibody
        private NavMeshAgent navMeshAgent; //Componente navMeshAgent
        private PlayerAttack playerAttack;

        private Camera cam; //Camera principal do jogo

        private Ray ray;
        private Vector3 movementPosition;  //Posi��o para qual o player ir� se mover

        private Vector3 mousePosition;
        private Vector3 wantedMousePosition;

        private bool isWalking = false; //Flag que indica que o objetando est� se movendo
        [SerializeField]  private float currentSpeed = 3.0f; //Velocidade atual do jogador
        private float currentjumpVelocity = 0; //Velocidade do objeto atual nop ar
        private bool navMeshRemainingPath = false; // Flag que indica se o objeto deve continuar a se movimentar com o navmesh

        private bool isKeyboardMoving = false;  // Flag para determinar se o jogador est� andando via teclado
        private bool isMouseMoving = false;  // Flag para determinar se o jogador est� andando via mouse
        private int isWalkingHash; //Hash da String que se refere a anima��o de Walk

        private bool isRunning = false; // Flag para determinar se o jogador est� correndo
        private int isRunningHash; //Hash da String que se refere a anima��o de Running

        [SerializeField]  private bool isJumping = false; // Flag para determinar se o jogador est� pulando
        [SerializeField]  private bool isFalling = false; // Flag para determinar se o jogador est� caindo
        private int isJumpingHash; //Hash da String que se refere a anima��o de Jumping

        private bool isRolling = false;
        private int isRollingHash;

        public bool IsJumping { get { return isJumping; } }



        #endregion

        #region  BEGIN/END SCRIPT

        private void Awake()
        {
            // Inicializa os componentes e vari�veis necess�rias quando o objeto � criado
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            playerAttack = GetComponent<PlayerAttack>();
        }

        private void Start()
        {
            // Inicializa hashes das strings usadas para controlar anima��es e obt�m a c�mera principal do jogo
            isWalkingHash = Animator.StringToHash("isWalking");
            isRunningHash = Animator.StringToHash("isRunning");
            isJumpingHash = Animator.StringToHash("TriggerJump");
            isRollingHash = Animator.StringToHash("TriggerRoll");
            cam = Camera.main;
        }

        #endregion

        #region  UPDATES 

        private void Update()
        {
            if (playerAttack.IsRangedAttacking) // Se o jogador est� atacando � dist�ncia
            {
                mousePosition = Input.mousePosition;
                wantedMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 100f));
                // Determina a dire��o para onde o jogador deve olhar
                Vector3 direction = wantedMousePosition - transform.position;

                direction.y = 0; // Define o componente y como zero para restringir ao plano XZ

                // Calcula a rota��o do jogador para olhar na dire��o determinada
                Quaternion novaRotacao = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Euler(0, novaRotacao.eulerAngles.y, 0); // Ajusta a rota��o do jogador apenas nos eixos X e Z
            }
            else
            {
                updateMoveParameters();
            }

        }

        private void FixedUpdate()
        {
            if (playerAttack.IsRangedAttacking) //Se o player est� atirando, ent�o ele n�o pode se mover
            {
                return;
            }

            if (playerAttack.IsMeleeAttacking) currentSpeed = attackingSpeed;
            else if (isRolling) currentSpeed = rollingSpeed;
            else if (isRunning) currentSpeed = runSpeed;
            else currentSpeed = walkSpeed;


            if (isJumping) //Player est� pulando
            {
                currentjumpVelocity -= jumpForce * jumpSlowdown * Time.fixedDeltaTime; //Diminuindo a velocidade do player no ar
                if (currentjumpVelocity < 0) isFalling = true; //Player come�a a cair
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

        #region  MOVIMENTA��O 

        //Faz movimenta��o via Mouse
        private void doMouseMovimentation()
        {

            // Cria um raio a partir da posi��o do mouse convertida para o espa�o da tela.
            ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            // Realiza um raio para detectar colis�es no mundo e armazena as informa��es na vari�vel 'hit'.
            if (Physics.Raycast(ray, out RaycastHit hit, mouseInputDistance))
            {
                movementPosition = hit.point; // Define a posi��o de movimento com base no ponto onde o raio colidiu com o objeto.
            }

            MoveToPosition(movementPosition); // Chama a fun��o 'MoveToPosition' para mover o jogador at� a nova posi��o definida.

            navMeshRemainingPath = true; //Indica que temos um caminho a percorrer 

            if (navMeshAgent.enabled)
            {
                navMeshAgent.speed = currentSpeed; // Atualiza a velocidade do NavMeshAgent para a velocidade atual (corrida ou caminhada).
            }
        }

        //Faz movimenta��o via teclado
        private void doKeyboardMovimentation()
        {
            // Cria um vetor de dire��o a partir da posi��o de movimento do jogador, com a componente y zerada, e normaliza-o.
            Vector3 movementInputDirection = new Vector3(movementPosition.x, 0.0f, movementPosition.y).normalized;

            // Transforma a dire��o do movimento no espa�o da c�mera
            Vector3 cameraForward = cam.transform.forward;
            Vector3 cameraRight = cam.transform.right;

            // Define as componentes y de cameraForward e cameraRight como zero para manter o movimento no plano horizontal.
            cameraForward.y = 0;
            cameraRight.y = 0;

            // Normaliza os vetores da c�mera para garantir que tenham comprimento igual a 1.
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calcula a dire��o de movimento desejada no espa�o da c�mera.
            Vector3 desiredMoveDirection = cameraForward * movementInputDirection.z + cameraRight * movementInputDirection.x;

            // Define a velocidade do Rigidbody com base na dire��o de movimento desejada e na velocidade atual.
            rb.velocity = new Vector3(desiredMoveDirection.x * currentSpeed, currentjumpVelocity, desiredMoveDirection.z * currentSpeed) ;

            // Se houver uma dire��o de movimento (diferente de zero), realiza a rota��o do jogador.
            if (movementInputDirection != Vector3.zero)
            {
                // Calcula uma nova rota��o com base na dire��o de movimento desejada.
                Quaternion newRotation = Quaternion.LookRotation(desiredMoveDirection);

                // Aplica uma rota��o suave (Slerp) do jogador em dire��o � nova rota��o.
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.fixedDeltaTime);

            }
        }

        //Fun��o respons�vel por cuidar da anima��o do jogador
        private void updateMoveParameters()
        {
            //Verifica se o player chegou na posi��o via navmesh
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

        //Atualiza��o anima��o de mvimento
        private void updateMoveAnimation(bool isWalking, bool isRunning)
        {
            animator.SetBool(isWalkingHash, isWalking);//Para que ele chegue na anima��o de correr, � necess�rio que esteja andando
            animator.SetBool(isRunningHash, isRunning);
        }

        // Move o jogador para a posi��o de destino usando o NavMeshAgent
        private void MoveToPosition(Vector3 destination)
        {
            if (navMeshAgent.enabled)
            {
                navMeshAgent.SetDestination(destination);
            }
        }

        #endregion

        #region  FUN��ES DE ANIMA��O

        //Fun��o chamada a partir de um determinado frame da anima��o de Jump
        public void startJump()
        {
            //Setando valores iniciais do pulo
            currentjumpVelocity = jumpForce;
            navMeshAgent.enabled = false;
            isFalling = false;
            isJumping = true;
        }

        //Chamado da animação de pulo e ataque com pulo
        public void stopJump()
        {
            //Reseta valores
            currentjumpVelocity = 0;
            isJumping = false;
            isFalling = false;
            navMeshAgent.enabled = true;
        }

        public void stopRolling()
        {
            isRolling = false;
            currentSpeed = walkSpeed;
        }

        #endregion

        #region  CALLBACKS DE INPUT 

        /**Callback que reseta o caminho do NavMeshAgent, ativa a flag de movimento via teclado e atualiza a posi��o de movimento, 
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

        // Desativa a flag de movimento via teclado, reseta a posi��o de movimento e para a anima��o de corrida
        public void StopKeyboardMove(InputAction.CallbackContext context)
        {
            isKeyboardMoving = false;
            movementPosition = Vector2.zero;
            navMeshAgent.enabled = true;//Habilita navmesh novamente
        }

        // Ativa a flag de movimento via mouse e a anima��o de caminhar
        public void MouseMove(InputAction.CallbackContext context)
        {
            if (!isKeyboardMoving)
            {
                isMouseMoving = true;
            }
        }

        // Desativa a flag de movimento via mouse, reseta a posi��o de movimento e para a anima��o de corrida
        public void StopMouseMove(InputAction.CallbackContext context)
        {
            if (isMouseMoving)
            {
                isMouseMoving = false;
                movementPosition = Vector2.zero;
            }
        }

        // Define a flag de corrida como verdadeira e inicia a anima��o de corrida
        public void Run(InputAction.CallbackContext context)
        {
            isRunning = true;
        }

        // Para a anima��o de corrida, chamado quando usu�rio solta o shift ou quando para de andar
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

        // Chamado quando soltamos o bot�o de pulo
        public void StopJump(InputAction.CallbackContext context)
        {

        }

                //Inicia pulo
        public void Roll(InputAction.CallbackContext context)
        {
            //Debug.Log(isRolling);
            if (!isRolling)
            {
                currentSpeed = rollingSpeed;
                isRolling = true;
                animator.SetTrigger(isRollingHash);
            }
        }

        // Chamado quando soltamos o bot�o de pulo
        public void StopRoll(InputAction.CallbackContext context)
        {

        }

        #endregion
    }
}

