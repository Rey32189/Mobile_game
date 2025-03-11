using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartPointExample
{
    public class CP_EnemyExample : MonoBehaviour
    {
        private float _speed = 1f;

        void Update()
        {
            transform.Translate(Vector3.forward * Time.deltaTime * _speed);
        }

        //Temporarily increase speed when colliding with checkpoint
        public IEnumerator IncreaseSpeed()
        {
            _speed *= 1.5f;
            yield return new WaitForSeconds(1.5f);
            _speed /= 1.5f;
        }

        //Colliding with bullet, destroy self and bullet
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name.Contains("Bullet"))
            {
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
        }
    }
}