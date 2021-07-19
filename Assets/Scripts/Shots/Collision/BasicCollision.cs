using UnityEngine;

namespace GMTK
{
    public class BasicCollision : ICollision
    {
        protected readonly GameObject instantiate;
        protected readonly ProjectileMove projectileMove;

        public BasicCollision(GameObject instantiate, ProjectileMove projectileMove)
        {
            this.instantiate = instantiate;
            this.projectileMove = projectileMove;
        }

        public virtual void Collided(GameObject o, Collision collision, bool isOwnCore, int layer)
        {
            SpawnHit(o, layer);
            
            if (!isOwnCore)
                DoDamage(collision.collider, GetPLayerIndex(layer));

            projectileMove.CommitSudoku(instantiate);
        }

        protected int GetPLayerIndex(int layer)
                {
                    if (layer < 10)
                    {
                        return layer - 6;
                    }
                    return layer - 10;
                }

        protected static bool DoDamage(Component collider, int index)
        {
            if (collider.GetComponent<IDamage>() is null) return false;
            collider.GetComponent<IDamage>()?.DoDamage(index);
            return true;
        }
        
        protected static void SpawnHit(GameObject o, int layer)
        {
            GameObject hit = HelperClass.GetBulletHit(layer);
            GameObject a = Object.Instantiate(hit, o.transform.position, Quaternion.identity);
            ParticleSystem[] componentsInChildren = a.GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem componentsInChild in componentsInChildren)
            {
                componentsInChild.Play();
            }
        }
    }
}