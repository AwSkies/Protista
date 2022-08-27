// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""GameBoard"",
            ""id"": ""8b35c3e0-ce5c-4a08-ade7-3cd5c27905e9"",
            ""actions"": [
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""12d05c5d-5578-478e-849b-a104b3ddd67b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveHover"",
                    ""type"": ""Value"",
                    ""id"": ""ae667d5a-da7f-4611-a392-e84e8396df8b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": ""Tap(duration=0.05)""
                },
                {
                    ""name"": ""DeselectAll"",
                    ""type"": ""Button"",
                    ""id"": ""fb343437-2491-477d-857a-1a83736e2168"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PauseMenu"",
                    ""type"": ""Button"",
                    ""id"": ""b979d7ea-7585-4d79-b04b-1be445bcb12a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Single"",
                    ""type"": ""Button"",
                    ""id"": ""8183949e-6a46-41af-be0a-1ea712149100"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Unstack"",
                    ""type"": ""Button"",
                    ""id"": ""4b718b5f-1bb9-4487-bd62-aadb8626bee0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Contiguous"",
                    ""type"": ""Button"",
                    ""id"": ""3eeee1f6-6ef1-4509-9992-8d695490a974"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cannon"",
                    ""type"": ""Button"",
                    ""id"": ""83224f3b-a01e-4e5f-bd5f-6bcbd88dc55a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""V"",
                    ""type"": ""Button"",
                    ""id"": ""6308f02f-960e-4b98-aedf-626278f6fc55"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Wave"",
                    ""type"": ""Button"",
                    ""id"": ""b74ed59c-5bc6-4616-a437-8623d2a2d8c3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Random"",
                    ""type"": ""Button"",
                    ""id"": ""7056e5a3-3edc-4ba0-abab-64fd8a85c4cb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4d8794de-f62e-4724-82b2-db3836defbc8"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c743270b-e259-47e0-8050-5b3a4171fbb0"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c2b7524-8535-4b60-9b51-5a9239424f89"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""1cbc23b8-2d8e-42a2-8fca-cd81e38a3de3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""daf412b4-a5c0-4c96-94ab-cb1bd7a530f9"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c7c638bb-b00b-483b-b8b5-18901e9e8a02"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a11c46e4-386a-49f0-82ea-c13c8c0b995b"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9df3c3c4-1002-4fc9-b505-c0d6fc9b6582"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow Keys"",
                    ""id"": ""18de87b3-fc57-466e-8cea-f81631eed5ff"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e156af2b-0f1b-424c-b833-589df5932fc6"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f8621b87-360a-40bd-b0dc-57cc5d3a6d7b"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""9f8423a6-3625-4898-8e01-045ff5237f1d"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""79e05818-e7a6-4e58-8c86-89ad07703175"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a32612e3-4538-4232-b3e0-a7920c203536"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Dpad"",
                    ""id"": ""eddb3230-94fc-41a3-a62c-f4c92fc3d3ed"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""68a6640a-7504-4f1b-b844-1a17f2baf661"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""a33fae7f-411b-49c7-a0ca-455b3af8ef2e"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""4a443bfa-3662-4f1d-8d0f-dc39409ddb03"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a8dbac36-337e-4974-8fa2-250b622069b0"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHover"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""76041a07-fb91-4b7a-a442-ded1b0dac988"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DeselectAll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""074624f4-a09a-4d58-b85b-603a39b6258d"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DeselectAll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7fe3474d-0523-4c64-97df-c639483201f9"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PauseMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d722ba8d-b1a5-4d8c-beb0-2094b7c13bbd"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PauseMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""27cb6b97-381d-4b72-882c-831c58578019"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Single"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Controller"",
                    ""id"": ""5ea220d4-ec92-4e6e-8043-4d23eed91e50"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Single"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""a63bbd8b-5bf9-47f2-9a3f-5b4933ce0d5a"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Single"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""56ac8597-f78f-4532-8db7-4fed9355ad00"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Single"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""511a56c8-8c3e-4f31-96e2-4268e6cb2187"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Unstack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Controller"",
                    ""id"": ""e05bee6d-4b81-4f9a-bca6-557d43e6db12"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Unstack"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""15544a5f-6f4f-40aa-8c0c-a73b2c2669ca"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Unstack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""9402fae7-5e11-4177-9522-12fd6af93ec2"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Unstack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""be2d867b-f397-486c-bdef-70bd4ba3a23d"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Contiguous"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Controller"",
                    ""id"": ""ff1581e9-328c-4a4f-bb9e-167f45305e89"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Contiguous"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""1760f770-b5c7-48ae-991f-43f48f6e0fdc"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Contiguous"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""f6ae1f95-7265-4493-99f5-3e3f462dd397"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Contiguous"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""faba28d1-1abc-4c42-aac1-4976f9c33c18"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cannon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Controller"",
                    ""id"": ""72663c70-b382-4081-9e09-57a8fd9116cd"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cannon"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""e395702d-63d5-4e76-8a49-54343b0d8d75"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cannon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""1ae21881-96cc-4737-8807-40685a127955"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cannon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""7ebefa18-1b28-4982-b34d-52dd474902d0"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""V"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Controller"",
                    ""id"": ""2748693f-5d66-4406-9b9f-18fa8b24c8e7"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""V"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""883e9bd0-58f1-426a-883a-8feead348d5a"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""V"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""b8ce4bc5-2779-4df3-9793-2a6d032855d3"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""V"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""0c791a2c-6ac6-47e6-9626-048f1cbeb4a6"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Wave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Controller"",
                    ""id"": ""5bb04ec3-e6f3-4a5b-b887-1d76cd2cfdb3"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Wave"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""f584aab7-67df-40d0-b9f2-87759ef72e53"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Wave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""78f57e88-a4f5-459a-aa40-f16046919852"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Wave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""93423aba-eaa7-411a-8914-c76a9a25e458"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Random"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Controller"",
                    ""id"": ""f04cf00a-245a-45df-aff7-895dbfeb7100"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Random"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""b679ea49-0e28-499a-9433-235dae027548"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Random"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""69723d2d-7f2f-4656-ae4b-475eedc8ac5c"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Random"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // GameBoard
        m_GameBoard = asset.FindActionMap("GameBoard", throwIfNotFound: true);
        m_GameBoard_Select = m_GameBoard.FindAction("Select", throwIfNotFound: true);
        m_GameBoard_MoveHover = m_GameBoard.FindAction("MoveHover", throwIfNotFound: true);
        m_GameBoard_DeselectAll = m_GameBoard.FindAction("DeselectAll", throwIfNotFound: true);
        m_GameBoard_PauseMenu = m_GameBoard.FindAction("PauseMenu", throwIfNotFound: true);
        m_GameBoard_Single = m_GameBoard.FindAction("Single", throwIfNotFound: true);
        m_GameBoard_Unstack = m_GameBoard.FindAction("Unstack", throwIfNotFound: true);
        m_GameBoard_Contiguous = m_GameBoard.FindAction("Contiguous", throwIfNotFound: true);
        m_GameBoard_Cannon = m_GameBoard.FindAction("Cannon", throwIfNotFound: true);
        m_GameBoard_V = m_GameBoard.FindAction("V", throwIfNotFound: true);
        m_GameBoard_Wave = m_GameBoard.FindAction("Wave", throwIfNotFound: true);
        m_GameBoard_Random = m_GameBoard.FindAction("Random", throwIfNotFound: true);
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

    // GameBoard
    private readonly InputActionMap m_GameBoard;
    private IGameBoardActions m_GameBoardActionsCallbackInterface;
    private readonly InputAction m_GameBoard_Select;
    private readonly InputAction m_GameBoard_MoveHover;
    private readonly InputAction m_GameBoard_DeselectAll;
    private readonly InputAction m_GameBoard_PauseMenu;
    private readonly InputAction m_GameBoard_Single;
    private readonly InputAction m_GameBoard_Unstack;
    private readonly InputAction m_GameBoard_Contiguous;
    private readonly InputAction m_GameBoard_Cannon;
    private readonly InputAction m_GameBoard_V;
    private readonly InputAction m_GameBoard_Wave;
    private readonly InputAction m_GameBoard_Random;
    public struct GameBoardActions
    {
        private @PlayerControls m_Wrapper;
        public GameBoardActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Select => m_Wrapper.m_GameBoard_Select;
        public InputAction @MoveHover => m_Wrapper.m_GameBoard_MoveHover;
        public InputAction @DeselectAll => m_Wrapper.m_GameBoard_DeselectAll;
        public InputAction @PauseMenu => m_Wrapper.m_GameBoard_PauseMenu;
        public InputAction @Single => m_Wrapper.m_GameBoard_Single;
        public InputAction @Unstack => m_Wrapper.m_GameBoard_Unstack;
        public InputAction @Contiguous => m_Wrapper.m_GameBoard_Contiguous;
        public InputAction @Cannon => m_Wrapper.m_GameBoard_Cannon;
        public InputAction @V => m_Wrapper.m_GameBoard_V;
        public InputAction @Wave => m_Wrapper.m_GameBoard_Wave;
        public InputAction @Random => m_Wrapper.m_GameBoard_Random;
        public InputActionMap Get() { return m_Wrapper.m_GameBoard; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameBoardActions set) { return set.Get(); }
        public void SetCallbacks(IGameBoardActions instance)
        {
            if (m_Wrapper.m_GameBoardActionsCallbackInterface != null)
            {
                @Select.started -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnSelect;
                @MoveHover.started -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnMoveHover;
                @MoveHover.performed -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnMoveHover;
                @MoveHover.canceled -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnMoveHover;
                @DeselectAll.started -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnDeselectAll;
                @DeselectAll.performed -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnDeselectAll;
                @DeselectAll.canceled -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnDeselectAll;
                @PauseMenu.started -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnPauseMenu;
                @PauseMenu.performed -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnPauseMenu;
                @PauseMenu.canceled -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnPauseMenu;
                @Single.started -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnSingle;
                @Single.performed -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnSingle;
                @Single.canceled -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnSingle;
                @Unstack.started -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnUnstack;
                @Unstack.performed -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnUnstack;
                @Unstack.canceled -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnUnstack;
                @Contiguous.started -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnContiguous;
                @Contiguous.performed -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnContiguous;
                @Contiguous.canceled -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnContiguous;
                @Cannon.started -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnCannon;
                @Cannon.performed -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnCannon;
                @Cannon.canceled -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnCannon;
                @V.started -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnV;
                @V.performed -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnV;
                @V.canceled -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnV;
                @Wave.started -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnWave;
                @Wave.performed -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnWave;
                @Wave.canceled -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnWave;
                @Random.started -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnRandom;
                @Random.performed -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnRandom;
                @Random.canceled -= m_Wrapper.m_GameBoardActionsCallbackInterface.OnRandom;
            }
            m_Wrapper.m_GameBoardActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
                @MoveHover.started += instance.OnMoveHover;
                @MoveHover.performed += instance.OnMoveHover;
                @MoveHover.canceled += instance.OnMoveHover;
                @DeselectAll.started += instance.OnDeselectAll;
                @DeselectAll.performed += instance.OnDeselectAll;
                @DeselectAll.canceled += instance.OnDeselectAll;
                @PauseMenu.started += instance.OnPauseMenu;
                @PauseMenu.performed += instance.OnPauseMenu;
                @PauseMenu.canceled += instance.OnPauseMenu;
                @Single.started += instance.OnSingle;
                @Single.performed += instance.OnSingle;
                @Single.canceled += instance.OnSingle;
                @Unstack.started += instance.OnUnstack;
                @Unstack.performed += instance.OnUnstack;
                @Unstack.canceled += instance.OnUnstack;
                @Contiguous.started += instance.OnContiguous;
                @Contiguous.performed += instance.OnContiguous;
                @Contiguous.canceled += instance.OnContiguous;
                @Cannon.started += instance.OnCannon;
                @Cannon.performed += instance.OnCannon;
                @Cannon.canceled += instance.OnCannon;
                @V.started += instance.OnV;
                @V.performed += instance.OnV;
                @V.canceled += instance.OnV;
                @Wave.started += instance.OnWave;
                @Wave.performed += instance.OnWave;
                @Wave.canceled += instance.OnWave;
                @Random.started += instance.OnRandom;
                @Random.performed += instance.OnRandom;
                @Random.canceled += instance.OnRandom;
            }
        }
    }
    public GameBoardActions @GameBoard => new GameBoardActions(this);
    public interface IGameBoardActions
    {
        void OnSelect(InputAction.CallbackContext context);
        void OnMoveHover(InputAction.CallbackContext context);
        void OnDeselectAll(InputAction.CallbackContext context);
        void OnPauseMenu(InputAction.CallbackContext context);
        void OnSingle(InputAction.CallbackContext context);
        void OnUnstack(InputAction.CallbackContext context);
        void OnContiguous(InputAction.CallbackContext context);
        void OnCannon(InputAction.CallbackContext context);
        void OnV(InputAction.CallbackContext context);
        void OnWave(InputAction.CallbackContext context);
        void OnRandom(InputAction.CallbackContext context);
    }
}
