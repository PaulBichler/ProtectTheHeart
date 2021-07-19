using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerControls controls;
    private PlayerConfiguration playerConfiguration;
    private PlayerController playerController;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    public void OnDestroy()
    {
        playerConfiguration.Input.onActionTriggered -= InputOnActionTriggered;
    }

    public void SetPlayerController(PlayerController controller)
    {
        playerController = controller;
    }

    public void InitializePlayer(PlayerConfiguration configuration)
    {
        playerConfiguration = configuration;
        playerController.Movement.isMouse = !playerConfiguration.Input.currentControlScheme.Equals("Gamepad");
        configuration.Input.onActionTriggered += InputOnActionTriggered;
    }

    private void InputOnActionTriggered(InputAction.CallbackContext context)
    {
        if (playerController.isDead) return;
        
        if (context.action.name == controls.Player.Move.name)
            OnMove(context);
        if (context.action.name == controls.Player.Aim.name)
            OnAim(context);
        if (context.action.name == controls.Player.Shoot.name)
            OnShoot();
        else if (context.action.name == controls.Player.Dash.name)
            OnDash();
        else if (context.action.name == controls.Player.Ready.name)
            OnReady();
    }

    public void OnControlsChanged(PlayerInput input)
    {
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        playerController.Movement.Aim = context.ReadValue<Vector2>();
    }

    private void OnShoot()
    {
        playerController.Shot.Shot();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        playerController.Movement.InputVector = context.ReadValue<Vector2>();
    }

    private void OnDash()
    {
        playerController.Movement.OnDash();
    }

    private void OnReady()
    {
        if (LobbyMenu.Instance)
            LobbyMenu.Instance.SetReady(playerConfiguration.PlayerIndex);
    }
}