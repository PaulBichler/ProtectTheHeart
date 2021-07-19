﻿//
//
//NOTES:
//
//This script is used for DEMONSTRATION porpuses of the Projectiles. I recommend everyone to create their own code for their own projects.
//THIS IS JUST A BASIC EXAMPLE PUT TOGETHER TO DEMONSTRATE VFX ASSETS.
//
//


#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMoveScript : MonoBehaviour
{
    public bool rotate;
    public float rotateAmount = 45;
    public bool bounce;
    public float bounceForce = 10;
    public float speed;

    [Tooltip("From 0% to 100%")] public float accuracy;

    public float fireRate;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public List<GameObject> trails;
    private bool collided;
    private Vector3 offset;
    private Rigidbody rb;
    private RotateToMouseScript rotateToMouse;
    private float speedRandomness;

    private Vector3 startPos;
    private GameObject target;

    private void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();

        //used to create a radius for the accuracy and have a very unique randomness
        if (accuracy != 100)
        {
            accuracy = 1 - accuracy / 100;

            for (int i = 0; i < 2; i++)
            {
                float val = 1 * Random.Range(-accuracy, accuracy);
                int index = Random.Range(0, 2);
                if (i == 0)
                {
                    if (index == 0)
                        offset = new Vector3(0, -val, 0);
                    else
                        offset = new Vector3(0, val, 0);
                }
                else
                {
                    if (index == 0)
                        offset = new Vector3(0, offset.y, -val);
                    else
                        offset = new Vector3(0, offset.y, val);
                }
            }
        }

        if (muzzlePrefab != null)
        {
            GameObject muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward + offset;
            ParticleSystem ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(muzzleVFX, ps.main.duration);
            }
            else
            {
                ParticleSystem psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
    }

    private void FixedUpdate()
    {
        if (target != null)
            rotateToMouse.RotateToMouse(gameObject, target.transform.position);
        if (rotate)
            transform.Rotate(0, 0, rotateAmount, Space.Self);
        if (speed != 0 && rb != null)
            rb.position += (transform.forward + offset) * (speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision co)
    {
        if (!bounce)
        {
            if (co.gameObject.tag != "Bullet" && !collided)
            {
                collided = true;

                if (trails.Count > 0)
                    for (int i = 0; i < trails.Count; i++)
                    {
                        trails[i].transform.parent = null;
                        ParticleSystem ps = trails[i].GetComponent<ParticleSystem>();
                        if (ps != null)
                        {
                            ps.Stop();
                            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                        }
                    }

                speed = 0;
                GetComponent<Rigidbody>().isKinematic = true;

                ContactPoint contact = co.contacts[0];
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
                Vector3 pos = contact.point;

                if (hitPrefab != null)
                {
                    GameObject hitVFX = Instantiate(hitPrefab, pos, rot);

                    ParticleSystem ps = hitVFX.GetComponent<ParticleSystem>();
                    if (ps == null)
                    {
                        ParticleSystem psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                        Destroy(hitVFX, psChild.main.duration);
                    }
                    else
                    {
                        Destroy(hitVFX, ps.main.duration);
                    }
                }

                StartCoroutine(DestroyParticle(0f));
            }
        }
        else
        {
            ContactPoint contact = co.contacts[0];
            rb.AddForce(Vector3.Reflect((contact.point - startPos).normalized, contact.normal) * bounceForce,
                ForceMode.Impulse);
            //Destroy ( this );
        }
    }

    public IEnumerator DestroyParticle(float waitTime)
    {
        if (transform.childCount > 0 && waitTime != 0)
        {
            List<Transform> tList = new List<Transform>();

            foreach (Transform t in transform.GetChild(0).transform) tList.Add(t);

            while (transform.GetChild(0).localScale.x > 0)
            {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < tList.Count; i++) tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            }
        }

        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }

    public void SetTarget(GameObject trg, RotateToMouseScript rotateTo)
    {
        target = trg;
        rotateToMouse = rotateTo;
    }
}