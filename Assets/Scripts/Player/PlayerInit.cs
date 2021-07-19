using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInit : MonoBehaviour
{
    private void Awake()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        PlayerConfiguration playerConfig = GameManager.Instance.HandlePlayerJoin(playerInput);

        if (LobbyMenu.Instance)
            LobbyMenu.Instance.HandlePlayerJoin(playerInput, playerConfig);
    }
}