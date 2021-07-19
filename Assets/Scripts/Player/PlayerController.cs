using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerShot Shot { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public int PlayerIndex { get; private set; }
    public PlayerHeart Heart { get; private set; }

    private void Awake()
    {
        Movement = GetComponentInChildren<PlayerMovement>();
        Shot = GetComponentInChildren<PlayerShot>();
        InputHandler = GetComponentInChildren<PlayerInputHandler>();
        Heart = GetComponentInChildren<PlayerHeart>();
        
        InputHandler.SetPlayerController(this);
    }

    public void SetPlayerIndex(int index)
    {
        PlayerIndex = index;
        Heart.SetPlayerController(this);
    }

    public void SetMaterial(Material playerMaterial)
    {
        Renderer[] children = transform.GetComponentsInChildren<Renderer>();
        for (int j = 0; j < children.Length - 1; j++)
            children[j].material = playerMaterial;
    }

    public bool isDead = false;
    
    public void Die()
    {
        isDead = true;
        
        transform.DOScale(Vector3.zero, 2f);
        transform.GetChild(0).DOShakeRotation(2f).OnComplete(() => Destroy(gameObject));
    }
}