//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Actions/BallAction.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @BallAction: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @BallAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""BallAction"",
    ""maps"": [
        {
            ""name"": ""BallControl"",
            ""id"": ""41387f94-218f-400d-9386-5c8bce81fea2"",
            ""actions"": [
                {
                    ""name"": ""LR"",
                    ""type"": ""PassThrough"",
                    ""id"": ""df9a841f-3793-4a6b-9fee-1555731c75e3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Launch"",
                    ""type"": ""Button"",
                    ""id"": ""8b45c745-8c62-4472-95bf-dc8bce45767b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""81679fcd-153d-4831-8054-126f2357dc05"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LR"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""left"",
                    ""id"": ""7dcbc3c4-2107-435f-b437-bf961f2bdbfc"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""de6dd01b-9b97-45ef-9829-1de8fae98a15"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""24af431b-0098-4263-9155-035d1f0630ae"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Hold(duration=0.1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Launch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Player1"",
            ""id"": ""d79eb84e-16ac-4d88-a284-a24f5418e346"",
            ""actions"": [
                {
                    ""name"": ""Launch"",
                    ""type"": ""Button"",
                    ""id"": ""92e8ada2-2459-4104-9fc4-0cb1bae9203d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LR"",
                    ""type"": ""PassThrough"",
                    ""id"": ""739647f3-92a1-4682-adb1-ccc7f6e4c069"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""affda1e2-a0a0-4c30-8e6e-bfeabc5f465a"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LR"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""left"",
                    ""id"": ""6d4c114f-bcc8-49b7-a67b-7409a2454ad2"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""1aec017b-010a-4b6c-b510-c759cb96e487"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""5de87b4e-65b4-4c1e-a62d-66ae3d66157c"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Hold(duration=0.1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Launch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Player2"",
            ""id"": ""daf80224-d6c1-4b34-88cb-b6ad69ccf695"",
            ""actions"": [
                {
                    ""name"": ""Launch"",
                    ""type"": ""Button"",
                    ""id"": ""739d0bf4-9cd2-4e23-a4da-c90c575c70ed"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LR"",
                    ""type"": ""PassThrough"",
                    ""id"": ""1852836e-fb7c-41d5-961c-f8417caf4e98"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""faa74646-f346-413b-970e-2f0a2eba87f1"",
                    ""path"": ""<Keyboard>/numpad0"",
                    ""interactions"": ""Hold(duration=0.1)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Launch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""5cc9a919-252f-4d8d-9691-22e836ef9998"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LR"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""left"",
                    ""id"": ""08077283-0930-49c0-a09c-fc36fff043f9"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""6424f909-d9ec-4e99-a6a0-c2729c9716c5"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LR"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // BallControl
        m_BallControl = asset.FindActionMap("BallControl", throwIfNotFound: true);
        m_BallControl_LR = m_BallControl.FindAction("LR", throwIfNotFound: true);
        m_BallControl_Launch = m_BallControl.FindAction("Launch", throwIfNotFound: true);
        // Player1
        m_Player1 = asset.FindActionMap("Player1", throwIfNotFound: true);
        m_Player1_Launch = m_Player1.FindAction("Launch", throwIfNotFound: true);
        m_Player1_LR = m_Player1.FindAction("LR", throwIfNotFound: true);
        // Player2
        m_Player2 = asset.FindActionMap("Player2", throwIfNotFound: true);
        m_Player2_Launch = m_Player2.FindAction("Launch", throwIfNotFound: true);
        m_Player2_LR = m_Player2.FindAction("LR", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // BallControl
    private readonly InputActionMap m_BallControl;
    private List<IBallControlActions> m_BallControlActionsCallbackInterfaces = new List<IBallControlActions>();
    private readonly InputAction m_BallControl_LR;
    private readonly InputAction m_BallControl_Launch;
    public struct BallControlActions
    {
        private @BallAction m_Wrapper;
        public BallControlActions(@BallAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @LR => m_Wrapper.m_BallControl_LR;
        public InputAction @Launch => m_Wrapper.m_BallControl_Launch;
        public InputActionMap Get() { return m_Wrapper.m_BallControl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BallControlActions set) { return set.Get(); }
        public void AddCallbacks(IBallControlActions instance)
        {
            if (instance == null || m_Wrapper.m_BallControlActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_BallControlActionsCallbackInterfaces.Add(instance);
            @LR.started += instance.OnLR;
            @LR.performed += instance.OnLR;
            @LR.canceled += instance.OnLR;
            @Launch.started += instance.OnLaunch;
            @Launch.performed += instance.OnLaunch;
            @Launch.canceled += instance.OnLaunch;
        }

        private void UnregisterCallbacks(IBallControlActions instance)
        {
            @LR.started -= instance.OnLR;
            @LR.performed -= instance.OnLR;
            @LR.canceled -= instance.OnLR;
            @Launch.started -= instance.OnLaunch;
            @Launch.performed -= instance.OnLaunch;
            @Launch.canceled -= instance.OnLaunch;
        }

        public void RemoveCallbacks(IBallControlActions instance)
        {
            if (m_Wrapper.m_BallControlActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IBallControlActions instance)
        {
            foreach (var item in m_Wrapper.m_BallControlActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_BallControlActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public BallControlActions @BallControl => new BallControlActions(this);

    // Player1
    private readonly InputActionMap m_Player1;
    private List<IPlayer1Actions> m_Player1ActionsCallbackInterfaces = new List<IPlayer1Actions>();
    private readonly InputAction m_Player1_Launch;
    private readonly InputAction m_Player1_LR;
    public struct Player1Actions
    {
        private @BallAction m_Wrapper;
        public Player1Actions(@BallAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Launch => m_Wrapper.m_Player1_Launch;
        public InputAction @LR => m_Wrapper.m_Player1_LR;
        public InputActionMap Get() { return m_Wrapper.m_Player1; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Player1Actions set) { return set.Get(); }
        public void AddCallbacks(IPlayer1Actions instance)
        {
            if (instance == null || m_Wrapper.m_Player1ActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_Player1ActionsCallbackInterfaces.Add(instance);
            @Launch.started += instance.OnLaunch;
            @Launch.performed += instance.OnLaunch;
            @Launch.canceled += instance.OnLaunch;
            @LR.started += instance.OnLR;
            @LR.performed += instance.OnLR;
            @LR.canceled += instance.OnLR;
        }

        private void UnregisterCallbacks(IPlayer1Actions instance)
        {
            @Launch.started -= instance.OnLaunch;
            @Launch.performed -= instance.OnLaunch;
            @Launch.canceled -= instance.OnLaunch;
            @LR.started -= instance.OnLR;
            @LR.performed -= instance.OnLR;
            @LR.canceled -= instance.OnLR;
        }

        public void RemoveCallbacks(IPlayer1Actions instance)
        {
            if (m_Wrapper.m_Player1ActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayer1Actions instance)
        {
            foreach (var item in m_Wrapper.m_Player1ActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_Player1ActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public Player1Actions @Player1 => new Player1Actions(this);

    // Player2
    private readonly InputActionMap m_Player2;
    private List<IPlayer2Actions> m_Player2ActionsCallbackInterfaces = new List<IPlayer2Actions>();
    private readonly InputAction m_Player2_Launch;
    private readonly InputAction m_Player2_LR;
    public struct Player2Actions
    {
        private @BallAction m_Wrapper;
        public Player2Actions(@BallAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Launch => m_Wrapper.m_Player2_Launch;
        public InputAction @LR => m_Wrapper.m_Player2_LR;
        public InputActionMap Get() { return m_Wrapper.m_Player2; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Player2Actions set) { return set.Get(); }
        public void AddCallbacks(IPlayer2Actions instance)
        {
            if (instance == null || m_Wrapper.m_Player2ActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_Player2ActionsCallbackInterfaces.Add(instance);
            @Launch.started += instance.OnLaunch;
            @Launch.performed += instance.OnLaunch;
            @Launch.canceled += instance.OnLaunch;
            @LR.started += instance.OnLR;
            @LR.performed += instance.OnLR;
            @LR.canceled += instance.OnLR;
        }

        private void UnregisterCallbacks(IPlayer2Actions instance)
        {
            @Launch.started -= instance.OnLaunch;
            @Launch.performed -= instance.OnLaunch;
            @Launch.canceled -= instance.OnLaunch;
            @LR.started -= instance.OnLR;
            @LR.performed -= instance.OnLR;
            @LR.canceled -= instance.OnLR;
        }

        public void RemoveCallbacks(IPlayer2Actions instance)
        {
            if (m_Wrapper.m_Player2ActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayer2Actions instance)
        {
            foreach (var item in m_Wrapper.m_Player2ActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_Player2ActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public Player2Actions @Player2 => new Player2Actions(this);
    public interface IBallControlActions
    {
        void OnLR(InputAction.CallbackContext context);
        void OnLaunch(InputAction.CallbackContext context);
    }
    public interface IPlayer1Actions
    {
        void OnLaunch(InputAction.CallbackContext context);
        void OnLR(InputAction.CallbackContext context);
    }
    public interface IPlayer2Actions
    {
        void OnLaunch(InputAction.CallbackContext context);
        void OnLR(InputAction.CallbackContext context);
    }
}