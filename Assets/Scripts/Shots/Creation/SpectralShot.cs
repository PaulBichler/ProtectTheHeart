using UnityEngine;

namespace GMTK
{
    public class SpectralShot : RicochetOneBasicShot
    {
        public SpectralShot(WeaponData weaponData) : base(weaponData)
        {
        }

        public override void Shot(Transform transform, GameObject projectile, Vector3 _startPos)
        {
            Init(transform, projectile, _startPos);
            _playerShot.SetCollisionStrategy(new RicochetOneShotCollision(instantiate, _playerShot));
            instantiate.layer += 4;
        }
    }
}