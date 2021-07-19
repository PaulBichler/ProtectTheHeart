using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GMTK
{
    public class BurstShot : BasicShot
    {
        private readonly int _time;

        public BurstShot(WeaponData weaponData) : base(weaponData)
        {
            _time = weaponData.TimeBurst;
        }


        public override void Shot(Transform transform, GameObject projectile, Vector3 _startPos)
        {
            Init(transform, projectile, _startPos);
            _playerShot.SetCollisionStrategy(new RicochetOneShotCollision(instantiate, _playerShot));

            DoTasks(() =>
            {
                Init(transform, projectile, _startPos);
                _playerShot.SetCollisionStrategy(new RicochetOneShotCollision(instantiate, _playerShot));
            }, _time).Await();

            DoTasks(() =>
            {
                Init(transform, projectile, _startPos);
                _playerShot.SetCollisionStrategy(new RicochetOneShotCollision(instantiate, _playerShot));
            }, _time * 2).Await();
        }

        private async Task DoTasks(Action action, int time)
        {
            await Task.Delay(time);
            action.Invoke();
        }
    }
}