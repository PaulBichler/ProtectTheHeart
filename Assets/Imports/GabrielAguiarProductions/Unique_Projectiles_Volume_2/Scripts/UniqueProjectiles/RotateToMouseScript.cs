//
//NOTES:
//This script is used for DEMONSTRATION porpuses of the Projectiles. I recommend everyone to create their own code for their own projects.
//This is just a basic example.
//

using System.Collections;
using UnityEngine;

public class RotateToMouseScript : MonoBehaviour
{
    public float maximumLenght;
    private Camera cam;
    private Vector3 direction;
    private Vector3 pos;
    private Ray rayMouse;
    private Quaternion rotation;
    private readonly WaitForSeconds updateTime = new WaitForSeconds(0.01f);

    private bool use2D;


    public void StartUpdateRay()
    {
        StartCoroutine(UpdateRay());
    }

    private IEnumerator UpdateRay()
    {
        if (cam != null)
        {
            if (use2D)
            {
                Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (angle > 180) angle -= 360;
                rotation.eulerAngles = new Vector3(-angle, 90, 0); // use different values to lock on different axis
                transform.rotation = rotation;
            }
            else
            {
                RaycastHit hit;
                Vector3 mousePos = Input.mousePosition;
                rayMouse = cam.ScreenPointToRay(mousePos);
                if (Physics.Raycast(rayMouse.origin, rayMouse.direction, out hit, maximumLenght))
                {
                    RotateToMouse(gameObject, hit.point);
                }
                else
                {
                    Vector3 pos = rayMouse.GetPoint(maximumLenght);
                    RotateToMouse(gameObject, pos);
                }
            }
            yield return updateTime;
            StartCoroutine(UpdateRay());
        }
        else
        {
            Debug.Log("Camera not set");
        }
    }

    public void RotateToMouse(GameObject obj, Vector3 destination)
    {
        direction = destination - obj.transform.position;
        rotation = Quaternion.LookRotation(direction);
        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
    }

    public void Set2D(bool state)
    {
        use2D = state;
    }

    public void SetCamera(Camera camera)
    {
        cam = camera;
    }

    public Vector3 GetDirection()
    {
        return direction;
    }

    public Quaternion GetRotation()
    {
        return rotation;
    }
}