//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/Input settings/CameraControls.inputactions
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

public partial class @CameraControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @CameraControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""CameraControls"",
    ""maps"": [
        {
            ""name"": ""ControlCamera"",
            ""id"": ""1d8518b4-a497-475a-bd51-e7e9225c0b5b"",
            ""actions"": [
                {
                    ""name"": ""MoveCamera"",
                    ""type"": ""Value"",
                    ""id"": ""5e0b5e21-06c1-4f77-8853-ea3b982b84d7"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""RotateCamera"",
                    ""type"": ""Value"",
                    ""id"": ""4a3eb6dc-3b72-4e8e-8d6d-0ac6a5f4b8d5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""ZoomCamera"",
                    ""type"": ""Value"",
                    ""id"": ""a2ff92a5-cb4f-496a-a9ec-cf317615dbb3"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""c0a4fa2e-822c-4422-99d6-28b930e0d85f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c5770a53-b19f-40cb-b7fe-7ec2a14fdde9"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7210227b-2857-419d-82b1-80360e856afe"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""f285962f-5efa-4fef-a703-e452c6393251"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""beb3a718-94e9-4ed0-8ca1-c18df9a2d720"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""bb3fd4b5-46a7-43c4-92c8-bfbc19f475ea"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b1d776f1-02e8-48d2-aeed-ce13bce30df6"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ZoomCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // ControlCamera
        m_ControlCamera = asset.FindActionMap("ControlCamera", throwIfNotFound: true);
        m_ControlCamera_MoveCamera = m_ControlCamera.FindAction("MoveCamera", throwIfNotFound: true);
        m_ControlCamera_RotateCamera = m_ControlCamera.FindAction("RotateCamera", throwIfNotFound: true);
        m_ControlCamera_ZoomCamera = m_ControlCamera.FindAction("ZoomCamera", throwIfNotFound: true);
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

    // ControlCamera
    private readonly InputActionMap m_ControlCamera;
    private List<IControlCameraActions> m_ControlCameraActionsCallbackInterfaces = new List<IControlCameraActions>();
    private readonly InputAction m_ControlCamera_MoveCamera;
    private readonly InputAction m_ControlCamera_RotateCamera;
    private readonly InputAction m_ControlCamera_ZoomCamera;
    public struct ControlCameraActions
    {
        private @CameraControls m_Wrapper;
        public ControlCameraActions(@CameraControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveCamera => m_Wrapper.m_ControlCamera_MoveCamera;
        public InputAction @RotateCamera => m_Wrapper.m_ControlCamera_RotateCamera;
        public InputAction @ZoomCamera => m_Wrapper.m_ControlCamera_ZoomCamera;
        public InputActionMap Get() { return m_Wrapper.m_ControlCamera; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ControlCameraActions set) { return set.Get(); }
        public void AddCallbacks(IControlCameraActions instance)
        {
            if (instance == null || m_Wrapper.m_ControlCameraActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_ControlCameraActionsCallbackInterfaces.Add(instance);
            @MoveCamera.started += instance.OnMoveCamera;
            @MoveCamera.performed += instance.OnMoveCamera;
            @MoveCamera.canceled += instance.OnMoveCamera;
            @RotateCamera.started += instance.OnRotateCamera;
            @RotateCamera.performed += instance.OnRotateCamera;
            @RotateCamera.canceled += instance.OnRotateCamera;
            @ZoomCamera.started += instance.OnZoomCamera;
            @ZoomCamera.performed += instance.OnZoomCamera;
            @ZoomCamera.canceled += instance.OnZoomCamera;
        }

        private void UnregisterCallbacks(IControlCameraActions instance)
        {
            @MoveCamera.started -= instance.OnMoveCamera;
            @MoveCamera.performed -= instance.OnMoveCamera;
            @MoveCamera.canceled -= instance.OnMoveCamera;
            @RotateCamera.started -= instance.OnRotateCamera;
            @RotateCamera.performed -= instance.OnRotateCamera;
            @RotateCamera.canceled -= instance.OnRotateCamera;
            @ZoomCamera.started -= instance.OnZoomCamera;
            @ZoomCamera.performed -= instance.OnZoomCamera;
            @ZoomCamera.canceled -= instance.OnZoomCamera;
        }

        public void RemoveCallbacks(IControlCameraActions instance)
        {
            if (m_Wrapper.m_ControlCameraActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IControlCameraActions instance)
        {
            foreach (var item in m_Wrapper.m_ControlCameraActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_ControlCameraActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public ControlCameraActions @ControlCamera => new ControlCameraActions(this);
    public interface IControlCameraActions
    {
        void OnMoveCamera(InputAction.CallbackContext context);
        void OnRotateCamera(InputAction.CallbackContext context);
        void OnZoomCamera(InputAction.CallbackContext context);
    }
}
