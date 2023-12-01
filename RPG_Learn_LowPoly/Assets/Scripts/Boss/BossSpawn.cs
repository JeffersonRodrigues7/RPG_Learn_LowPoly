using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Boss
{
    public class BossSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject bossPrefab; // Prefab do chefe
        [SerializeField] private GameObject rain; // Efeito de chuva
        [SerializeField] private GameObject gameLight; // Luz do jogo
        [SerializeField] private GameObject Camera; // Câmera do jogo
        [SerializeField] private Material newSky; // Novo material para o céu
        [SerializeField] private Transform portal; // Ponto de origem do portal
        [SerializeField] private Transform spawnLocation; // Local de spawn do chefe
        [SerializeField] float riseSpeed = 0.5f; // Velocidade de subida do chefe
        [SerializeField] float initialPosition = -3.5f; // Posição inicial do chefe antes de subir
        [SerializeField] float maxYPosition = 0f; // Altura máxima que o chefe atingirá
        // Músicas
        [SerializeField] private AudioClip bossMusic; // Música do chefe

        private Transform player; // Referência ao jogador
        private GameObject boss; // Instância do chefe
        private bool isRising = false; // Indica se o chefe está subindo
        private bool alreadySpawned = false; // Indica se o chefe já foi spawnado

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform; // Encontra o jogador
            rain.SetActive(false); // Desativa o efeito de chuva inicialmente
            gameLight.SetActive(true); // Ativa a luz do jogo inicialmente
        }

        void Update()
        {
            if (isRising)
            {
                rain.SetActive(true); // Ativa o efeito de chuva durante a subida
                gameLight.SetActive(false); // Desativa a luz do jogo durante a subida
                Camera.GetComponent<Skybox>().material = newSky; // Altera o material do céu

                // Move o chefe para cima
                boss.transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);

                if (player != null)
                {
                    // Mantém o chefe olhando na direção do jogador
                    Vector3 targetPosition = player.position;
                    targetPosition.y = boss.transform.position.y;
                    boss.transform.LookAt(targetPosition);
                }

                // Verifica se atingiu a altura máxima
                if (boss.transform.position.y >= maxYPosition)
                {
                    isRising = false;

                    // Habilita os scripts do chefe quando atinge a altura máxima
                    EnableBossScripts();
                }
            }
        }

        void EnableBossScripts()
        {
            // Habilita os componentes específicos do chefe
            boss.GetComponent<NavMeshAgent>().enabled = true;
            boss.GetComponent<BoxCollider>().enabled = true;
            boss.GetComponent<Rigidbody>().useGravity = true;

            // Habilita todos os scripts do chefe
            MonoBehaviour[] scripts = boss.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = true;
            }
        }

        void PlayBossMusic()
        {
            // Encontra o gerenciador de música
            GameObject songManager = GameObject.Find("SongManager");

            // Verifica se o gerenciador foi encontrado
            if (songManager != null)
            {
                // Obtém o componente AudioSource do gerenciador
                AudioSource audioSource = songManager.GetComponent<AudioSource>();

                // Verifica se o componente AudioSource foi encontrado
                if (audioSource != null)
                {
                    // Para a música atual
                    audioSource.Stop();

                    // Atribui a música do chefe ao AudioSource
                    audioSource.clip = bossMusic;
                    audioSource.volume = 0.6f;

                    // Inicia a música do chefe
                    audioSource.Play();
                }
                else
                {
                    Debug.LogError("AudioSource component not found on SongManager.");
                }
            }
            else
            {
                Debug.LogError("SongManager GameObject not found.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Verifica a colisão com o jogador e se o chefe não foi spawnado ainda
            if (other.CompareTag("Player") && boss == null && !alreadySpawned)
            {
                alreadySpawned = true;

                // Instancia o chefe no portal de spawn
                boss = Instantiate(bossPrefab, portal.position, Quaternion.identity, spawnLocation);
                boss.transform.position = new Vector3(boss.transform.position.x, initialPosition, boss.transform.position.z);

                // Inicia a subida do chefe
                isRising = true;

                // Inicia a música do chefe
                PlayBossMusic();
            }
        }
    }
}
