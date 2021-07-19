using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UpgradePopup : MonoBehaviour
{
    private GameObject player;
    private Vector3 OffsetToPlayer = new Vector3(0, 3f, 0);

    public void Setup(GameObject p, Texture2D tex, string text, Color color)
    {
        player = p;
        GetComponentInChildren<TextMeshPro>().text = text;
        GetComponentInChildren<Renderer>().material.SetTexture("Texture2D_d6c8ef4ed772428397c28e34fb1ee6c1", tex);
        GetComponentInChildren<Renderer>().material.SetColor("Color_f5f60154132a4cffba29c65eb104b6c4", color);
    }
    
    private void Awake()
    {
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(1.5f);

        transform.DOScale(Vector3.zero, 2f).OnComplete(() => Destroy(gameObject));
    }
    
    void Update()
    {
        transform.position = player.transform.position + OffsetToPlayer;
        transform.LookAt(Camera.main.transform.position);
    }
}
