using UnityEngine;

namespace GMTK
{
    public class BasicShot : IShotStrategy
    {
        private readonly float _fireRate;
        private readonly float _force;
        protected ProjectileMove _playerShot;
        protected GameObject instantiate;

        public BasicShot(WeaponData weaponData)
        {
            _fireRate = weaponData.FireRate;
            _force = weaponData.Force;
        }

        public virtual void Shot(Transform transform, GameObject projectile, Vector3 _startPos)
        {
            Init(transform, projectile, _startPos);
            _playerShot.SetCollisionStrategy(new BasicCollision(instantiate, _playerShot));
        }

        public float GetFireRate()
        {
            return _fireRate;
        }

        protected void Init(Transform transform, GameObject projectile, Vector3 _startPos, bool force = true)
        {
            instantiate = InstantiateBullet(transform, projectile, _startPos);
            _playerShot = instantiate.GetComponent<ProjectileMove>();
            _playerShot.Force = _force;
            if (force)
                _playerShot.AddForce(instantiate.transform.forward);
        }


        private static GameObject InstantiateBullet(Transform transform, GameObject projectile, Vector3 _startPos)
        {
            SoundManager.Provider.PlayRandomSoundFromPack(SoundType.Shoot);
            Vector3 transformForward = transform.forward;
            GameObject o = Object.Instantiate(projectile, transform);
            o.layer = transform.gameObject.layer;
            o.transform.position = _startPos;
            o.transform.localRotation = Quaternion.Euler(transformForward);

            return o;
        }
    }
}