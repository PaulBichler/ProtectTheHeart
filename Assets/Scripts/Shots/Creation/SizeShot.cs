using UnityEngine;

namespace GMTK
{
    public class SizeShot : BasicShot
    {
        private readonly float _scale;

        public SizeShot(WeaponData weaponData) : base(weaponData)
        {
            _scale = weaponData.Scale;
        }

        public override void Shot(Transform transform, GameObject projectile, Vector3 _startPos)
        {
            Init(transform, projectile, _startPos, false);
            _playerShot.SetCollisionStrategy(new RicochetOneShotCollision(instantiate, _playerShot));
            instantiate.transform.localScale = Vector3.one * _scale;
            _playerShot.AddForce(instantiate.transform.forward);
        }
    }
}