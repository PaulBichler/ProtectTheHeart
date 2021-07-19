// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Object = UnityEngine.Object;

public class PlayerControls : IInputActionCollection, IDisposable
{
    // Player
    private readonly InputActionMap m_Player;
    private readonly InputAction m_Player_Aim;
    private readonly InputAction m_Player_Dash;
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Ready;
    private readonly InputAction m_Player_Shoot;
    private int m_GamepadSchemeIndex = -1;
    private int m_keyboardmouseSchemeIndex = -1;
    private IPlayerActions m_PlayerActionsCallbackInterface;

    public PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""eedba91d-d733-4ce9-a5dd-a52c0e87b4be"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""dd7198d9-d74b-42e3-8b7c-42aa30f327c1"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""0120bb68-8a41-4e6e-b6e7-f9866491ca66"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""1986d8fd-3563-474f-8578-5b181d763fb4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Value"",
                    ""id"": ""e5b53925-494a-4750-86b5-bc72f8e33248"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ready"",
                    ""type"": ""Button"",
                    ""id"": ""23dcaedd-662f-40a1-8a8f-4ffdd48c053f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3857f5be-1bdf-4423-8611-a50957080cbf"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""abce7bb3-ce1a-4933-8728-f60c9d9eb1af"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f4c598ec-c3a4-4914-998e-e98eae4d3b2a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyboard+mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""42af06b8-059a-4dd2-832b-7db70e5b0b60"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyboard+mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a765a1d2-9cdd-434c-96e0-7ccc44c79fe3"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyboard+mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""40387d0c-fa28-46c0-8e1e-75d5c30857bd"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyboard+mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""732d0ffb-a057-4c78-93bc-bea501b9feef"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5e4242f6-f543-47ff-9fc3-8864769a8b74"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyboard+mouse"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""43fcd12f-8f61-44ce-ae65-39c89ba704f9"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f447d7dd-bb92-4b56-aad2-eb67842d235b"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyboard+mouse"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""802547cd-5c9d-438f-ae93-92778c9d02b3"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1ab127f0-8f30-408a-9f5b-85a1da294bfa"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyboard+mouse"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e16faff2-2194-49a2-860a-e7341df81e78"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Ready"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""db10928d-9346-4078-b8d2-343d7f5405ca"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyboard+mouse"",
                    ""action"": ""Ready"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""keyboard+mouse"",
            ""bindingGroup"": ""keyboard+mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", true);
        m_Player_Move = m_Player.FindAction("Move", true);
        m_Player_Dash = m_Player.FindAction("Dash", true);
        m_Player_Shoot = m_Player.FindAction("Shoot", true);
        m_Player_Aim = m_Player.FindAction("Aim", true);
        m_Player_Ready = m_Player.FindAction("Ready", true);
    }

    public InputActionAsset asset { get; }
    public PlayerActions Player => new PlayerActions(this);

    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }

    public InputControlScheme keyboardmouseScheme
    {
        get
        {
            if (m_keyboardmouseSchemeIndex == -1)
                m_keyboardmouseSchemeIndex = asset.FindControlSchemeIndex("keyboard+mouse");
            return asset.controlSchemes[m_keyboardmouseSchemeIndex];
        }
    }

    public void Dispose()
    {
        Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public struct PlayerActions
    {
        private readonly PlayerControls m_Wrapper;

        public PlayerActions(PlayerControls wrapper)
        {
            m_Wrapper = wrapper;
        }

        public InputAction Move => m_Wrapper.m_Player_Move;
        public InputAction Dash => m_Wrapper.m_Player_Dash;
        public InputAction Shoot => m_Wrapper.m_Player_Shoot;
        public InputAction Aim => m_Wrapper.m_Player_Aim;
        public InputAction Ready => m_Wrapper.m_Player_Ready;

        public InputActionMap Get()
        {
            return m_Wrapper.m_Player;
        }

        public void Enable()
        {
            Get().Enable();
        }

        public void Disable()
        {
            Get().Disable();
        }

        public bool enabled => Get().enabled;

        public static implicit operator InputActionMap(PlayerActions set)
        {
            return set.Get();
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                Dash.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                Dash.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                Dash.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                Shoot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                Shoot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                Shoot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                Aim.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAim;
                Aim.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAim;
                Aim.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAim;
                Ready.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnReady;
                Ready.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnReady;
                Ready.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnReady;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                Move.started += instance.OnMove;
                Move.performed += instance.OnMove;
                Move.canceled += instance.OnMove;
                Dash.started += instance.OnDash;
                Dash.performed += instance.OnDash;
                Dash.canceled += instance.OnDash;
                Shoot.started += instance.OnShoot;
                Shoot.performed += instance.OnShoot;
                Shoot.canceled += instance.OnShoot;
                Aim.started += instance.OnAim;
                Aim.performed += instance.OnAim;
                Aim.canceled += instance.OnAim;
                Ready.started += instance.OnReady;
                Ready.performed += instance.OnReady;
                Ready.canceled += instance.OnReady;
            }
        }
    }

    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);

        void OnDash(InputAction.CallbackContext context);

        void OnShoot(InputAction.CallbackContext context);

        void OnAim(InputAction.CallbackContext context);

        void OnReady(InputAction.CallbackContext context);
    }
}