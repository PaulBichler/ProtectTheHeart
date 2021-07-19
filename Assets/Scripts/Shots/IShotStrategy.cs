using UnityEngine;

namespace GMTK
{
    public interface IShotStrategy
    {
        void Shot(Transform transform, GameObject projectile, Vector3 _startPos);

        float GetFireRate();
    }
}