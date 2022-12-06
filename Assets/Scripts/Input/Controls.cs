//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Input/Controls.inputactions
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

namespace Bluaniman.SpaceGame.Input
{
    public partial class @Controls : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @Controls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""501554fb-0946-467f-a099-653f8c855f59"",
            ""actions"": [
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""4206aac1-aec4-4d77-8b43-39cd3a4cb920"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""e85a7158-90c0-4672-a09e-ddebf6284928"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Pitch"",
                    ""type"": ""Value"",
                    ""id"": ""c2f4dc35-96ce-44de-82fc-350cfee695f1"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Yaw"",
                    ""type"": ""Value"",
                    ""id"": ""f1e0d2f4-3a91-43ac-b2e2-c71213075e16"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Roll"",
                    ""type"": ""Value"",
                    ""id"": ""ad8b4cdd-919f-401a-bd0e-2da38857123d"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""ForwardThrust"",
                    ""type"": ""Value"",
                    ""id"": ""afb3cbc9-41c5-4c9f-98ad-832b9406bde7"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""HorizontalThrust"",
                    ""type"": ""Value"",
                    ""id"": ""b2d06d7a-3d90-4a2e-b6e7-1bf1db424f55"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""VerticalThrust"",
                    ""type"": ""Value"",
                    ""id"": ""75c8b85e-ba72-43b3-a997-d79a0c59d3c3"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Stop"",
                    ""type"": ""Value"",
                    ""id"": ""d85868d9-2935-46c8-aab8-9b9358a4027b"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""SnapMove"",
                    ""type"": ""Value"",
                    ""id"": ""1de23ccf-3b62-47b1-b98f-9af92340e3e7"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""FreeCamera"",
                    ""type"": ""Value"",
                    ""id"": ""315f3d91-080c-48b2-a832-4cdecf91f9d1"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b66e31eb-d1ab-4ae9-b537-05950604bdf8"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""559a6705-3518-4ce2-a846-88cf6a7c2e23"",
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
                    ""id"": ""d2c18d97-eec1-403d-83bd-33a63656d183"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1a44db62-ccec-4f32-bfd8-86e5e031a04f"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""1bfdb89f-fd74-4de1-a1b3-4a70395987bd"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""cba42939-0580-4bfb-833f-3fa0e79091b6"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Axis With Modifiers"",
                    ""id"": ""8c5c3abe-7e5b-4a4c-b467-c7c5f29b429b"",
                    ""path"": ""AxisWithModifiers"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalThrust"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""axis"",
                    ""id"": ""cf86cc5b-8f41-43c3-bd91-36689a9b006d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""HorizontalThrust"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""axis"",
                    ""id"": ""745d2b8a-f0e2-4e2d-a49f-a9ed7321b9ed"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""HorizontalThrust"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""9e41201f-7fd9-4493-b4d4-9bc203e2bff1"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""HorizontalThrust"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Axis With Modifiers"",
                    ""id"": ""a0c2f77b-f7f7-4f46-9b5f-701ec585d0db"",
                    ""path"": ""AxisWithModifiers"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ForwardThrust"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""axis"",
                    ""id"": ""3b48603f-6a46-4607-991c-db3008686832"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""ForwardThrust"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""axis"",
                    ""id"": ""2c08096f-822a-407d-bd6b-37eb8a44386e"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""ForwardThrust"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""1866a4d7-2e87-4030-846e-ad7de7b930f9"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""ForwardThrust"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Axis With Modifiers"",
                    ""id"": ""a7fb048d-c600-45c6-a350-eee25abdcde5"",
                    ""path"": ""AxisWithModifiers"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""VerticalThrust"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""axis"",
                    ""id"": ""b7e070dd-529d-473d-b8dd-195a87767609"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""VerticalThrust"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""axis"",
                    ""id"": ""9e3059a3-292a-4b99-8be0-46dd1b98cf00"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""VerticalThrust"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""29ed9e23-c3fb-4dc6-9711-0324514013ad"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""VerticalThrust"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Axis With Modifiers"",
                    ""id"": ""9779bc23-935e-400f-b69e-c6f0e3231e6b"",
                    ""path"": ""AxisWithModifiers"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Roll"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""axis"",
                    ""id"": ""189b6708-801c-480c-93a3-64e5c4bf5841"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Roll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""axis"",
                    ""id"": ""42eda7c0-5295-4a04-8001-f7d58808ccc0"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Roll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""9fdf84f1-2bb0-4c37-8593-0ffa9633ef96"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": ""NegateButton"",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Roll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Axis With Modifiers"",
                    ""id"": ""167edcdc-b8a2-4124-9cdb-77522fb0f91b"",
                    ""path"": ""AxisWithModifiers"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pitch"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""axis"",
                    ""id"": ""e40ad068-c8cc-4b42-9b08-81048b5994f7"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Pitch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""axis"",
                    ""id"": ""4398e13a-ee35-4420-8954-669cbe188db2"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Pitch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""86db195a-affb-4a4c-b6e4-9b5a384fc157"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": ""NegateButton"",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Pitch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Axis With Modifiers"",
                    ""id"": ""d55f6365-fe19-4300-b005-825ee6cf8fba"",
                    ""path"": ""AxisWithModifiers"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Yaw"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""axis"",
                    ""id"": ""8c8bea19-a502-46bf-bd47-e8127a06c93b"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Yaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""axis"",
                    ""id"": ""bb565f54-d630-4570-8eb8-9247313a6e03"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Yaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""321f5af6-20ae-4774-985a-17f5657ddd1d"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": ""NegateButton"",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Yaw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""7103b2ef-6da4-4c15-a3eb-0c99b4c3c2df"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Stop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""130bc0d5-e5f4-47d8-aada-2f30c8b1602e"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""SnapMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""97251bfa-bdd0-41e3-abc0-a5b967a99455"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""FreeCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard & Mouse"",
            ""bindingGroup"": ""Keyboard & Mouse"",
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
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
            m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
            m_Player_Pitch = m_Player.FindAction("Pitch", throwIfNotFound: true);
            m_Player_Yaw = m_Player.FindAction("Yaw", throwIfNotFound: true);
            m_Player_Roll = m_Player.FindAction("Roll", throwIfNotFound: true);
            m_Player_ForwardThrust = m_Player.FindAction("ForwardThrust", throwIfNotFound: true);
            m_Player_HorizontalThrust = m_Player.FindAction("HorizontalThrust", throwIfNotFound: true);
            m_Player_VerticalThrust = m_Player.FindAction("VerticalThrust", throwIfNotFound: true);
            m_Player_Stop = m_Player.FindAction("Stop", throwIfNotFound: true);
            m_Player_SnapMove = m_Player.FindAction("SnapMove", throwIfNotFound: true);
            m_Player_FreeCamera = m_Player.FindAction("FreeCamera", throwIfNotFound: true);
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

        // Player
        private readonly InputActionMap m_Player;
        private IPlayerActions m_PlayerActionsCallbackInterface;
        private readonly InputAction m_Player_Look;
        private readonly InputAction m_Player_Move;
        private readonly InputAction m_Player_Pitch;
        private readonly InputAction m_Player_Yaw;
        private readonly InputAction m_Player_Roll;
        private readonly InputAction m_Player_ForwardThrust;
        private readonly InputAction m_Player_HorizontalThrust;
        private readonly InputAction m_Player_VerticalThrust;
        private readonly InputAction m_Player_Stop;
        private readonly InputAction m_Player_SnapMove;
        private readonly InputAction m_Player_FreeCamera;
        public struct PlayerActions
        {
            private @Controls m_Wrapper;
            public PlayerActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Look => m_Wrapper.m_Player_Look;
            public InputAction @Move => m_Wrapper.m_Player_Move;
            public InputAction @Pitch => m_Wrapper.m_Player_Pitch;
            public InputAction @Yaw => m_Wrapper.m_Player_Yaw;
            public InputAction @Roll => m_Wrapper.m_Player_Roll;
            public InputAction @ForwardThrust => m_Wrapper.m_Player_ForwardThrust;
            public InputAction @HorizontalThrust => m_Wrapper.m_Player_HorizontalThrust;
            public InputAction @VerticalThrust => m_Wrapper.m_Player_VerticalThrust;
            public InputAction @Stop => m_Wrapper.m_Player_Stop;
            public InputAction @SnapMove => m_Wrapper.m_Player_SnapMove;
            public InputAction @FreeCamera => m_Wrapper.m_Player_FreeCamera;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    @Look.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                    @Look.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                    @Look.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                    @Move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    @Pitch.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPitch;
                    @Pitch.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPitch;
                    @Pitch.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPitch;
                    @Yaw.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnYaw;
                    @Yaw.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnYaw;
                    @Yaw.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnYaw;
                    @Roll.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRoll;
                    @Roll.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRoll;
                    @Roll.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRoll;
                    @ForwardThrust.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnForwardThrust;
                    @ForwardThrust.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnForwardThrust;
                    @ForwardThrust.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnForwardThrust;
                    @HorizontalThrust.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHorizontalThrust;
                    @HorizontalThrust.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHorizontalThrust;
                    @HorizontalThrust.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnHorizontalThrust;
                    @VerticalThrust.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVerticalThrust;
                    @VerticalThrust.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVerticalThrust;
                    @VerticalThrust.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnVerticalThrust;
                    @Stop.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnStop;
                    @Stop.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnStop;
                    @Stop.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnStop;
                    @SnapMove.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSnapMove;
                    @SnapMove.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSnapMove;
                    @SnapMove.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnSnapMove;
                    @FreeCamera.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFreeCamera;
                    @FreeCamera.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFreeCamera;
                    @FreeCamera.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFreeCamera;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Look.started += instance.OnLook;
                    @Look.performed += instance.OnLook;
                    @Look.canceled += instance.OnLook;
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Pitch.started += instance.OnPitch;
                    @Pitch.performed += instance.OnPitch;
                    @Pitch.canceled += instance.OnPitch;
                    @Yaw.started += instance.OnYaw;
                    @Yaw.performed += instance.OnYaw;
                    @Yaw.canceled += instance.OnYaw;
                    @Roll.started += instance.OnRoll;
                    @Roll.performed += instance.OnRoll;
                    @Roll.canceled += instance.OnRoll;
                    @ForwardThrust.started += instance.OnForwardThrust;
                    @ForwardThrust.performed += instance.OnForwardThrust;
                    @ForwardThrust.canceled += instance.OnForwardThrust;
                    @HorizontalThrust.started += instance.OnHorizontalThrust;
                    @HorizontalThrust.performed += instance.OnHorizontalThrust;
                    @HorizontalThrust.canceled += instance.OnHorizontalThrust;
                    @VerticalThrust.started += instance.OnVerticalThrust;
                    @VerticalThrust.performed += instance.OnVerticalThrust;
                    @VerticalThrust.canceled += instance.OnVerticalThrust;
                    @Stop.started += instance.OnStop;
                    @Stop.performed += instance.OnStop;
                    @Stop.canceled += instance.OnStop;
                    @SnapMove.started += instance.OnSnapMove;
                    @SnapMove.performed += instance.OnSnapMove;
                    @SnapMove.canceled += instance.OnSnapMove;
                    @FreeCamera.started += instance.OnFreeCamera;
                    @FreeCamera.performed += instance.OnFreeCamera;
                    @FreeCamera.canceled += instance.OnFreeCamera;
                }
            }
        }
        public PlayerActions @Player => new PlayerActions(this);
        private int m_KeyboardMouseSchemeIndex = -1;
        public InputControlScheme KeyboardMouseScheme
        {
            get
            {
                if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard & Mouse");
                return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
            }
        }
        public interface IPlayerActions
        {
            void OnLook(InputAction.CallbackContext context);
            void OnMove(InputAction.CallbackContext context);
            void OnPitch(InputAction.CallbackContext context);
            void OnYaw(InputAction.CallbackContext context);
            void OnRoll(InputAction.CallbackContext context);
            void OnForwardThrust(InputAction.CallbackContext context);
            void OnHorizontalThrust(InputAction.CallbackContext context);
            void OnVerticalThrust(InputAction.CallbackContext context);
            void OnStop(InputAction.CallbackContext context);
            void OnSnapMove(InputAction.CallbackContext context);
            void OnFreeCamera(InputAction.CallbackContext context);
        }
    }
}
