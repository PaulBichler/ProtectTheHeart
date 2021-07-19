using UnityEngine;

namespace GMTK
{
    public class ExplodeBasicShot : BasicShot
    {
        private readonly float _radius;

        public ExplodeBasicShot(WeaponData weaponData) : base(weaponData)
        {
            _radius = weaponData.Radius;
        }

        public override void Shot(Transform transform, GameObject projectile, Vector3 _startPos)
        {
            Init(transform, projectile, _startPos);
            _playerShot.SetCollisionStrategy(new ExplodeCollision(instantiate, _playerShot, _radius));
        }
    }
}