using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Boss
{
    public class BossSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject bossPrefab;
        [SerializeField] private GameObject rain;
        [SerializeField] private GameObject gameLight;
        [SerializeField] private GameObject Camera;
        [SerializeField] private Material newSky;
        [SerializeField] private Transform portal;
        [SerializeField] private Transform spawnLocation;
        [SerializeField] float riseSpeed = 0.5f;  // Velocidade de subida do boss
        [SerializeField] float initialPosition = -3.5f;  // Velocidade de subida do boss
        [SerializeField] float maxYPosition = 0f;  // Altura m�xima que o boss atingir�
        // Musicas
        [SerializeField] private AudioClip bossMusic; // Adicione isso para a música do boss


        private Transform player;
        private GameObject boss;
        private bool isRising = false;  // Indica se o boss est� subindo
        private bool alreadySpawned = false;

        private void Start()
        {
        
            player = GameObject.FindGameObjectWithTag("Player").transform;
            rain.SetActive(false);
            gameLight.SetActive(true);
        }

        void Update()
        {
            if (isRising)
            {
                rain.SetActive(true);
                gameLight.SetActive(false);
                Camera.GetComponent<Skybox>().material = newSky;

                // Move o boss para cima
                boss.transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);
                if(player != null)
                {
                    Vector3 targetPosition = player.position;
                    targetPosition.y = boss.transform.position.y;
                    boss.transform.LookAt(targetPosition);
                }

                // Verifica se atingiu a altura m�xima
                if (boss.transform.position.y >= maxYPosition)
                {
                    isRising = false;

                    // Habilita os scripts do boss quando atinge a altura m�xima
                    EnableBossScripts();
                }
            }
        }

        void EnableBossScripts()
        {
            boss.GetComponent<NavMeshAgent>().enabled = true;
            boss.GetComponent<BoxCollider>().enabled = true;
            boss.GetComponent<Rigidbody>().useGravity = true;

            // Habilita todos os scripts do boss
            MonoBehaviour[] scripts = boss.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = true;
            }
        }

        void PlayBossMusic()
        {
            // Encontre o GameObject chamado "SongManager"
            GameObject songManager = GameObject.Find("SongManager");

            // Verifique se o GameObject foi encontrado
            if (songManager != null)
            {
                // Acesse o componente AudioSource do SongManager
                AudioSource audioSource = songManager.GetComponent<AudioSource>();

                // Verifique se o componente AudioSource foi encontrado
                if (audioSource != null)
                {
                    // Pare a música atual
                    audioSource.Stop();

                    // Atribua a música do boss ao AudioSource
                    audioSource.clip = bossMusic;

                    // Inicie a música do boss
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
            if (other.CompareTag("Player") && boss == null && !alreadySpawned)
            {
                alreadySpawned = true;
                boss = Instantiate(bossPrefab, portal.position, Quaternion.identity, spawnLocation);
                boss.transform.position = new Vector3(boss.transform.position.x, initialPosition, boss.transform.position.z);

                isRising = true;
                PlayBossMusic();
            }
        }

    }

}
