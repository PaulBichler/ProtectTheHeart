using UnityEngine;

namespace GMTK
{
    public class MultiShot : BasicShot
    {
        public MultiShot(WeaponData weaponData) : base(weaponData)
        {
        }

        public override void Shot(Transform transform, GameObject projectile, Vector3 _startPos)
        {
            Vector3 transformForward = transform.forward;

            Init(transform, projectile, _startPos);
            _playerShot.SetCollisionStrategy(new RicochetOneShotCollision(instantiate, _playerShot));

            Init(transform, projectile, _startPos, false);
            _playerShot.SetCollisionStrategy(new RicochetOneShotCollision(instantiate, _playerShot));
            instantiate.transform.localRotation = Quaternion.Euler(transformForward + new Vector3(0, -20, 0));
            _playerShot.AddForce(instantiate.transform.forward);

            Init(transform, projectile, _startPos, false);
            _playerShot.SetCollisionStrategy(new RicochetOneShotCollision(instantiate, _playerShot));
            instantiate.transform.localRotation = Quaternion.Euler(transformForward + new Vector3(0, 20, 0));
            _playerShot.AddForce(instantiate.transform.forward);
        }
    }
}