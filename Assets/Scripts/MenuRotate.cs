using Lean.Transition;
using Lean.Transition.Method;
using UnityEngine;

public class MenuRotate : MonoBehaviour
{
    public int RotateSpeed;
    private LeanState _leanState;

    private bool _rotating = true;
    private float startRotX;

    private void Start()
    {
        startRotX = transform.rotation.eulerAngles.x > 180
            ? 360 - transform.rotation.eulerAngles.x
            : transform.rotation.eulerAngles.x;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        if (_rotating)
            transform.Rotate(Vector3.right, RotateSpeed * Time.deltaTime);
    }

    public void OnSelect()
    {
        if (SoundManager.Provider is { })
            SoundManager.Provider.PlayRandomSoundFromPack(SoundType.Select, 0.5f);

        _rotating = false;

        float currRotX = transform.rotation.eulerAngles.x;
        //xRot needs to be at 0,90,180 or 270
        //Modulo 90 tells us how far away we are
        //if that's above 45, we know we transform positively, else negatively
        float xRot = currRotX % 90;

        if (xRot < 45)
            xRot = xRot - 90;
            //xRot -= startRotX;
        //else xRot += startRotX;

        _leanState = LeanTransformRotate.Register(transform, new Vector3(xRot + startRotX, 0, 0), Space.Self, 0.5f,
            LeanEase.Bounce);
    }

    public void OnDeselect()
    {
        _leanState.Skip();
        _rotating = true;
    }
}