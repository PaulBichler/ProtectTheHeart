using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCamera(duration, magnitude));
    }
    
    private IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float z = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(x, originalPos.y, z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
