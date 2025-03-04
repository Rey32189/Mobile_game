using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartPoint;

namespace SmartPointExample
{
    public class CP_BulletExample : MonoBehaviour
    {
        [SerializeField]
        private float speed = 7f;
        [SerializeField]
        private float ttl = 5f;

        void Start()
        {
            StartCoroutine(DestroySelf());
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        private IEnumerator DestroySelf()
        {
            yield return new WaitForSeconds(ttl);
            Destroy(gameObject);
        }
        public void PassDirection(Vector3 dir)
        {
            transform.eulerAngles = dir;
        }
        public void SetParameters(float speed, float ttl)
        {
            this.speed = speed;
            this.ttl = ttl;
        }
    }
}
