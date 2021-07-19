using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }

    public PlayerInput Input { get; }
    public int PlayerIndex { get; }
    public bool isReady { get; set; }
    public Material playerMaterial { get; set; }
}