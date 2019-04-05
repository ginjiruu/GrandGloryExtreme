using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(ParticleSystem))]
    public class FireballLogic : MonoBehaviour
    {
        public int damage = 25;
        public float radius = 10;
        public float speed = 10;
        public float explosionForce = 15f;

        public Transform explosionParticles;
        private ParticleSystem inAir;

        private SphereCollider sCollider;
        private Rigidbody rb;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            sCollider = GetComponent<SphereCollider>();
            inAir = GetComponent<ParticleSystem>();
            
            inAir.Play();

            rb.AddForce((this.transform.forward + this.transform.up) * speed, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Explode();
        }

        private void Explode()
        {
            Transform clone = Instantiate(explosionParticles, 
                this.transform.position, this.transform.rotation) as Transform;

            Destroy(clone.gameObject, 3f);

            var cols = Physics.OverlapSphere(transform.position, radius);
            var rbs = new List<Rigidbody>();
            var playerStats = new List<StatesManager>();

            foreach (var col in cols)
            {
                if (col.attachedRigidbody != null && !rbs.Contains(col.attachedRigidbody))
                {
                    rbs.Add(col.attachedRigidbody);
                }
                StatesManager hitPlayer = col.GetComponent<StatesManager>();
                if (hitPlayer != null)
                {
                    hitPlayer.GetHit(damage, PlayerStats.PlayerType.hero);
                }
            }

            foreach (var rb in rbs)
            {
                rb.AddExplosionForce(explosionForce, transform.position, radius, radius, ForceMode.Impulse);
            }


            Destroy(this.gameObject);
        }
    }
}