using UnityEngine;

namespace GMTK
{
    public class RicochetOneShotCollision : BasicCollision
    {
        private readonly int _maxBounceCount;

        public RicochetOneShotCollision(GameObject instantiate, ProjectileMove projectileMove)
            : base(instantiate, projectileMove)
        {
            _maxBounceCount = 1;
        }

        public override void Collided(GameObject o, Collision collision, bool isOwnCore, int layer)
        {
            if (!isOwnCore)
                if (DoDamage(collision.collider, GetPLayerIndex(layer)))
                {
                    SpawnHit(o, layer);
                    projectileMove.CommitSudoku(o);
                    return;
                }
            if (projectileMove.BounceCount >= _maxBounceCount)
            {
                SpawnHit(o, layer);
                projectileMove.CommitSudoku(o);
                return;
            }

            SoundManager.Provider.PlayRandomSoundFromPack(SoundType.Bounce);
            
            projectileMove.BounceCount++;
            ContactPoint contact = collision.contacts[0];
            Vector3 _direction = Vector3.Reflect((contact.point - projectileMove.PreviousPos).normalized,
                contact.normal);
            projectileMove.SetPosition();

            projectileMove.AddForce(_direction);
        }
    }
}