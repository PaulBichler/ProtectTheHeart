using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMTK
{
    public class HeatSeekShot : BasicShot
    {
        private GameObject enemy;

        public HeatSeekShot(WeaponData weaponData) : base(weaponData)
        {
        }


        public override void Shot(Transform transform, GameObject projectile, Vector3 _startPos)
        {
            Init(transform, projectile, _startPos, false);
            _playerShot.SetCollisionStrategy(new HeatSeekCollision(instantiate, _playerShot));

            GetEnemy();
            _playerShot.AddForce(enemy, 2000);
        }

        private void GetEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");
            List<GameObject> realEnemies = enemies.Where(gameObject => gameObject.transform.parent is null).ToList();

            GameObject closest = realEnemies[0];
            if (_playerShot.gameObject.transform.root.gameObject.Equals(closest))
                closest = realEnemies[1];


            foreach (GameObject o in from o in realEnemies where !o.Equals(_playerShot.gameObject.transform.root.gameObject) where !closest.Equals(o) where Vector3.Distance(closest.transform.position, instantiate.transform.position) >
                                                                                                                                                            Vector3.Distance(o.transform.position, instantiate.transform.position) select o)
            {
                closest = o;
            }

            enemy = closest;
        }
    }
}