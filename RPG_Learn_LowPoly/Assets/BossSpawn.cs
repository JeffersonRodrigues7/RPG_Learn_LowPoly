using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Boss
{
    public class BossSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject bossPrefab;
        [SerializeField] private Transform spawnLocation;
        [SerializeField] float riseSpeed = 0.5f;  // Velocidade de subida do boss
        [SerializeField] float initialPosition = -3.5f;  // Velocidade de subida do boss
        [SerializeField] float maxYPosition = 0f;  // Altura m�xima que o boss atingir�

        private Transform player;
        private GameObject boss;
        private bool isRising = false;  // Indica se o boss est� subindo

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        void Update()
        {
            if (isRising)
            {
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && boss == null)
            {
                boss = Instantiate(bossPrefab, spawnLocation.position, Quaternion.identity, spawnLocation);
                boss.transform.position = new Vector3(boss.transform.position.x, initialPosition, boss.transform.position.z);

                isRising = true;
            }
        }

    }

}
