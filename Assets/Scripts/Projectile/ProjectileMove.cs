using System.Collections;
using UnityEngine;

namespace GMTK
{
    public class ProjectileMove : MonoBehaviour
    {
        private ICollision _collision;
        private Rigidbody _rb;
        private bool _stopFollowing;
        private GameObject _target;

        public float BounceCount { get; set; }
        public Vector3 PreviousPos { get; private set; }
        public float Force { get; set; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            StartCoroutine(Die(10));

            PreviousPos = transform.position;
            BounceCount = 0;
            Force = 2000;
            _stopFollowing = true;
        }

        private void FixedUpdate()
        {
            transform.LookAt(transform.position + _rb.velocity.normalized);
        }

        private void OnCollisionEnter(Collision other)
        {
            bool isOwnCore = other.gameObject.CompareTag("Core") &&
                             other.transform.parent.GetChild(0).gameObject.layer.Equals(gameObject.layer);
            _collision.Collided(gameObject, other, isOwnCore, gameObject.layer);
            CameraShake.Instance.Shake(0.2f, 0.2f);

            var splatterPrefab = Resources.Load<GameObject>(HelperClass.GetHitSplatterColors(gameObject.layer));
            Instantiate(splatterPrefab, other.GetContact(0).point, transform.rotation);
        }


        public void CommitSudoku(GameObject instantiate)
        {
            if (gameObject.Equals(instantiate)) Destroy(gameObject);
        }

        public void SetCollisionStrategy(ICollision collision)
        {
            _collision = collision;
        }

        public void AddForce(Vector3 direction)
        {
            _rb.AddForce(direction * Force);
        }

        private IEnumerator Die(float f)
        {
            yield return new WaitForSeconds(f);
            CommitSudoku(gameObject);
        }


        public void SetPosition()
        {
            PreviousPos = transform.position;
        }

        public void AddForce(GameObject o, int time)
        {
            _target = o;
            Force /= 1000;
            _rb.drag = 0.5f;
            _stopFollowing = false;
            StartCoroutine(MoveTowards());
            StartCoroutine(AddForceContinues(time));
        }

        private IEnumerator MoveTowards()
        {
            while (!_stopFollowing)
            {
                AddForce(_target.transform.GetChild(6).position - transform.position);
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator AddForceContinues(int time)
        {
            yield return new WaitForSeconds(time / 1000f);
            _rb.drag = 0;
            _stopFollowing = true;
            Force *= 1000;
        }

        public void Stop()
        {
            StopAllCoroutines();
            _rb.drag = 0;
            _stopFollowing = true;
            Force *= 1000;
        }
    }
}