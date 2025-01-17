//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Configurations/Input/InputActions.inputactions
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

public partial class @InputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""Kart"",
            ""id"": ""4db9e03e-6202-4df4-ab33-ff9f236b79a3"",
            ""actions"": [
                {
                    ""name"": ""Forward"",
                    ""type"": ""Button"",
                    ""id"": ""8914ea57-f83c-471e-8659-fde3ea8ca087"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Steer"",
                    ""type"": ""Value"",
                    ""id"": ""f60d06ab-df81-4a19-a6ea-5b13729cdfd6"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Drift"",
                    ""type"": ""Button"",
                    ""id"": ""256ac391-2332-43cd-9c6b-4f929bf5a26f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Look Back"",
                    ""type"": ""Button"",
                    ""id"": ""1dffb5d3-322b-44a0-bd6a-02ed1e8092a7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Use Item"",
                    ""type"": ""Button"",
                    ""id"": ""afc12db4-7212-47b2-ad3d-457cabeed447"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""fa361a73-1e41-49b5-82b6-ad4ccbd3b4fc"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""fd7e9865-7a6f-4ee9-80a2-21441d31c755"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""7f00c553-fd79-4b60-9091-2d0bfec2dd6d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""5a0002f0-e6c1-4c37-9f80-a6ce1940de04"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""e8d2b640-22cf-40a0-b804-573e9d279a6a"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""e2ca1559-376b-4b78-8e7c-f3f0364b51da"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis Keyboard"",
                    ""id"": ""44558629-ea9a-48dc-b13c-5cc446c0ebcf"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""ede0e4e0-5e96-4451-a545-7d821e1d2238"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""9d68022c-564a-4582-971c-aa2ff6a114e0"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis Gamepad"",
                    ""id"": ""c961f1cf-bcca-4090-93ac-02b6a11a6e02"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""774da092-a5a0-424b-802f-a3fbeb5787a1"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""2e55c0d1-0f42-47a6-8854-2f03d372cc78"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""15ac9a13-576f-469a-a892-bcce3934f497"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b9e5548-de74-47af-a5aa-de621e99f089"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f02bc37c-54b7-4787-b860-9e0d64afd63d"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""71d4e12b-fb0a-456d-adb3-e4d612269eb6"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drift"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fc0651d5-0225-4119-8a31-cd6a8e50b488"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ef662ca8-1b02-4fc8-b336-47cc4f44d9b3"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""53b7bb7d-fbb1-4831-8fdf-b07ac84e1b9d"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""726dd808-9288-4f07-addc-a34dc34dadfb"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Use Item"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""127af21e-c5e5-423c-9227-044daf954238"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Use Item"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""42a7c752-bf7d-40b5-a7d0-3154d58e17c7"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Use Item"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""System"",
            ""id"": ""10d64cd3-4109-4cd5-bbda-b65737c03d1f"",
            ""actions"": [
                {
                    ""name"": ""Change Player"",
                    ""type"": ""Button"",
                    ""id"": ""db3b7487-4c30-43d8-a1ff-3f7611ac79ca"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""095ea113-3ec8-4a6c-b96d-2e6c22fbe698"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e6c85b34-128f-40e0-b905-8f9485b60b55"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Change Player"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dd3ab157-401b-450f-925f-827ab06f8c3d"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Change Player"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fcdc97ac-a3e9-4bdc-bfce-a8cbae643045"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Change Player"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cff10557-d0f7-40ea-9257-9aa2942057a1"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""06c5fc70-f920-4f03-bc50-5f1a2785fa0c"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Kart
        m_Kart = asset.FindActionMap("Kart", throwIfNotFound: true);
        m_Kart_Forward = m_Kart.FindAction("Forward", throwIfNotFound: true);
        m_Kart_Steer = m_Kart.FindAction("Steer", throwIfNotFound: true);
        m_Kart_Drift = m_Kart.FindAction("Drift", throwIfNotFound: true);
        m_Kart_LookBack = m_Kart.FindAction("Look Back", throwIfNotFound: true);
        m_Kart_UseItem = m_Kart.FindAction("Use Item", throwIfNotFound: true);
        // System
        m_System = asset.FindActionMap("System", throwIfNotFound: true);
        m_System_ChangePlayer = m_System.FindAction("Change Player", throwIfNotFound: true);
        m_System_Back = m_System.FindAction("Back", throwIfNotFound: true);
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

    // Kart
    private readonly InputActionMap m_Kart;
    private List<IKartActions> m_KartActionsCallbackInterfaces = new List<IKartActions>();
    private readonly InputAction m_Kart_Forward;
    private readonly InputAction m_Kart_Steer;
    private readonly InputAction m_Kart_Drift;
    private readonly InputAction m_Kart_LookBack;
    private readonly InputAction m_Kart_UseItem;
    public struct KartActions
    {
        private @InputActions m_Wrapper;
        public KartActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Forward => m_Wrapper.m_Kart_Forward;
        public InputAction @Steer => m_Wrapper.m_Kart_Steer;
        public InputAction @Drift => m_Wrapper.m_Kart_Drift;
        public InputAction @LookBack => m_Wrapper.m_Kart_LookBack;
        public InputAction @UseItem => m_Wrapper.m_Kart_UseItem;
        public InputActionMap Get() { return m_Wrapper.m_Kart; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KartActions set) { return set.Get(); }
        public void AddCallbacks(IKartActions instance)
        {
            if (instance == null || m_Wrapper.m_KartActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_KartActionsCallbackInterfaces.Add(instance);
            @Forward.started += instance.OnForward;
            @Forward.performed += instance.OnForward;
            @Forward.canceled += instance.OnForward;
            @Steer.started += instance.OnSteer;
            @Steer.performed += instance.OnSteer;
            @Steer.canceled += instance.OnSteer;
            @Drift.started += instance.OnDrift;
            @Drift.performed += instance.OnDrift;
            @Drift.canceled += instance.OnDrift;
            @LookBack.started += instance.OnLookBack;
            @LookBack.performed += instance.OnLookBack;
            @LookBack.canceled += instance.OnLookBack;
            @UseItem.started += instance.OnUseItem;
            @UseItem.performed += instance.OnUseItem;
            @UseItem.canceled += instance.OnUseItem;
        }

        private void UnregisterCallbacks(IKartActions instance)
        {
            @Forward.started -= instance.OnForward;
            @Forward.performed -= instance.OnForward;
            @Forward.canceled -= instance.OnForward;
            @Steer.started -= instance.OnSteer;
            @Steer.performed -= instance.OnSteer;
            @Steer.canceled -= instance.OnSteer;
            @Drift.started -= instance.OnDrift;
            @Drift.performed -= instance.OnDrift;
            @Drift.canceled -= instance.OnDrift;
            @LookBack.started -= instance.OnLookBack;
            @LookBack.performed -= instance.OnLookBack;
            @LookBack.canceled -= instance.OnLookBack;
            @UseItem.started -= instance.OnUseItem;
            @UseItem.performed -= instance.OnUseItem;
            @UseItem.canceled -= instance.OnUseItem;
        }

        public void RemoveCallbacks(IKartActions instance)
        {
            if (m_Wrapper.m_KartActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IKartActions instance)
        {
            foreach (var item in m_Wrapper.m_KartActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_KartActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public KartActions @Kart => new KartActions(this);

    // System
    private readonly InputActionMap m_System;
    private List<ISystemActions> m_SystemActionsCallbackInterfaces = new List<ISystemActions>();
    private readonly InputAction m_System_ChangePlayer;
    private readonly InputAction m_System_Back;
    public struct SystemActions
    {
        private @InputActions m_Wrapper;
        public SystemActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @ChangePlayer => m_Wrapper.m_System_ChangePlayer;
        public InputAction @Back => m_Wrapper.m_System_Back;
        public InputActionMap Get() { return m_Wrapper.m_System; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SystemActions set) { return set.Get(); }
        public void AddCallbacks(ISystemActions instance)
        {
            if (instance == null || m_Wrapper.m_SystemActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_SystemActionsCallbackInterfaces.Add(instance);
            @ChangePlayer.started += instance.OnChangePlayer;
            @ChangePlayer.performed += instance.OnChangePlayer;
            @ChangePlayer.canceled += instance.OnChangePlayer;
            @Back.started += instance.OnBack;
            @Back.performed += instance.OnBack;
            @Back.canceled += instance.OnBack;
        }

        private void UnregisterCallbacks(ISystemActions instance)
        {
            @ChangePlayer.started -= instance.OnChangePlayer;
            @ChangePlayer.performed -= instance.OnChangePlayer;
            @ChangePlayer.canceled -= instance.OnChangePlayer;
            @Back.started -= instance.OnBack;
            @Back.performed -= instance.OnBack;
            @Back.canceled -= instance.OnBack;
        }

        public void RemoveCallbacks(ISystemActions instance)
        {
            if (m_Wrapper.m_SystemActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ISystemActions instance)
        {
            foreach (var item in m_Wrapper.m_SystemActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_SystemActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public SystemActions @System => new SystemActions(this);
    public interface IKartActions
    {
        void OnForward(InputAction.CallbackContext context);
        void OnSteer(InputAction.CallbackContext context);
        void OnDrift(InputAction.CallbackContext context);
        void OnLookBack(InputAction.CallbackContext context);
        void OnUseItem(InputAction.CallbackContext context);
    }
    public interface ISystemActions
    {
        void OnChangePlayer(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
    }
}
