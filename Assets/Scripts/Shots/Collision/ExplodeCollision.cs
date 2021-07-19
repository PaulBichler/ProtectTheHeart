using UnityEngine;

namespace GMTK
{
    public class ExplodeCollision : BasicCollision
    {
        private readonly float _radius;

        public ExplodeCollision(GameObject instantiate, ProjectileMove projectileMove, float radius) : base(
            instantiate, projectileMove)
        {
            _radius = radius;
        }

        public override void Collided(GameObject o, Collision collision, bool isOwnCore, int layer)
        {
            SpawnHit(o, layer);
            SpawnExplosion(o,layer);

            Collider[] colliders = Physics.OverlapSphere(instantiate.transform.position, _radius);

            if (colliders.Length > 0)
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.CompareTag("Core") &&
                        collider.transform.root.GetChild(0).gameObject.layer.Equals(layer))
                        continue;
                    DoDamage(collider, GetPLayerIndex(layer));
                }
            
            CameraShake.Instance.Shake(0.2f, 0.6f);
            projectileMove.CommitSudoku(o);
        }

        private void SpawnExplosion(GameObject gameObject, int layer)
        {
            GameObject hit = HelperClass.GetBulletExplosion(layer);
            GameObject a = Object.Instantiate(hit, gameObject.transform.position, Quaternion.identity);
            ParticleSystem[] componentsInChildren = a.GetComponentsInChildren<ParticleSystem>();
            a.transform.localScale = Vector3.one * _radius;

            foreach (ParticleSystem componentsInChild in componentsInChildren)
            {
                componentsInChild.Play();
            }
        }
    }
}