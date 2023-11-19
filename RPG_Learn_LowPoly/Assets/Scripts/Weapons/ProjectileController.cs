using RPG.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Projectile
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private string ownerTag = "";
        [SerializeField] private float speed = 1;
        [SerializeField] private string enemyTag = "";
        [SerializeField] private float damage = 15f;

        private Vector3 target = Vector3.zero;

        HealthController healthController;

        public string EnemyTag { set { enemyTag = value; } }

        void Update()
        {
            if(target != null)
            { 
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        }

        public void SetTarget(string _ownerTag, Vector3 _target, string _enemyTag)
        {
            ownerTag = _ownerTag;
            target = _target;
            enemyTag = _enemyTag;
            transform.LookAt(target);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals(enemyTag))
            {
                healthController = other.gameObject?.GetComponent<HealthController>();

                if (healthController != null)
                {
                    healthController.takeDamage(ownerTag, damage);
                }
                Destroy(gameObject);
            }
            
        }

    }

}

