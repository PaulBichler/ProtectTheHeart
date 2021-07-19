using UnityEngine;

namespace GMTK
{
    public class HeatSeekCollision : RicochetOneShotCollision
    {
        public HeatSeekCollision(GameObject instantiate, ProjectileMove projectileMove)
            : base(instantiate, projectileMove)
        {
        }

        public override void Collided(GameObject o, Collision collision, bool isOwnCore, int layer)
        {
            projectileMove.Stop();
            base.Collided(o, collision, isOwnCore, layer);
        }
    }
}