using UnityEngine;

namespace GMTK
{
    public interface ICollision
    {
        void Collided(GameObject o, Collision collision, bool isOwnCore, int layer);
    }
}