using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class PlayerInputHandler : MonoBehaviour //all player inputs
    {
        #region "Public fields for Inspector" 
        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 1f;

        [Tooltip("Additional sensitivity multiplier for WebGL")]
        public float WebglLookSensitivityMultiplier = 0.25f;

        [Tooltip("Limit to consider an input when using a trigger on a controller")]
        public float TriggerAxisThreshold = 0.4f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;
        #endregion

        GameFlowManager m_GameFlowManager; //declares GameFlowManager script to be accessed later
        PlayerCharacterController m_PlayerCharacterController; //declares PlayerCharacterController script to be accessed later
        bool m_FireInputWasHeld; 

        void Start()
        {
            m_PlayerCharacterController = GetComponent<PlayerCharacterController>(); //accesses PlayerCharacterController script
            DebugUtility.HandleErrorIfNullGetComponent<PlayerCharacterController, PlayerInputHandler>(
                m_PlayerCharacterController, this, gameObject); //returns error if the necessary scripts are missing
            m_GameFlowManager = FindObjectOfType<GameFlowManager>(); //accesses GameFlowManager script
            DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, PlayerInputHandler>(m_GameFlowManager, this); 

            Cursor.lockState = CursorLockMode.Locked; //sets cursor to locked in center of screen
            Cursor.visible = false; //sets cursor to invisible
        }

        void LateUpdate()
        {
            m_FireInputWasHeld = GetFireInputHeld(); //sets true if fire is held
        }

        #region "Input Functions"

        public bool CanProcessInput() //ensures cursor is locked in center of screen and level is not ending
        {
            return Cursor.lockState == CursorLockMode.Locked && !m_GameFlowManager.GameIsEnding;
        }

        public Vector3 GetMoveInput() //MOVE
        {
            if (CanProcessInput())
            {
                Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 0f,
                    Input.GetAxisRaw(GameConstants.k_AxisNameVertical)); //horizontal is left/right, veritcal is forward/back?

                // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
                move = Vector3.ClampMagnitude(move, 1);

                return move;
            }

            return Vector3.zero;
        }

        public float GetLookInputsHorizontal() //HORIZONTAL LOOK
        {
            return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameHorizontal,
                GameConstants.k_AxisNameJoystickLookHorizontal); //returns horizontal axis float after necessary sensitivity multipliers
        }

        public float GetLookInputsVertical() //VERTICAL LOOK
        {
            return GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameVertical,
                GameConstants.k_AxisNameJoystickLookVertical); //returns vertical axis float after necessary sensitivity multipliers
        }

        public bool GetJumpInputDown() //JUMP DOWN
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonNameJump);
            }

            return false;
        }

        public bool GetJumpInputHeld() //JUMP HELD (Jetpack usage)
        {
            if (CanProcessInput())
            {
                return Input.GetButton(GameConstants.k_ButtonNameJump);
            }

            return false;
        }

        public bool GetFireInputDown() //FIRE DOWN
        {
            return GetFireInputHeld() && !m_FireInputWasHeld; //returns true if fire button went from not being pressed to being pressed
        }

        public bool GetFireInputReleased() //FIRE RELEASED
        {
            return !GetFireInputHeld() && m_FireInputWasHeld; //returns true if fire button went from being pressed to not being pressed
        }

        public bool GetFireInputHeld() //FIRE HELD
        {
            if (CanProcessInput())
            {
                bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadFire) != 0f; //set to true if fire button is being pressed on gamepad
                if (isGamepad)
                {
                    return Input.GetAxis(GameConstants.k_ButtonNameGamepadFire) >= TriggerAxisThreshold; //returns true if the trigger passes the threshold to be considered as input
                }
                else
                {
                    return Input.GetButton(GameConstants.k_ButtonNameFire); //returns true if fire button on kbm is pressed
                }
            }

            return false;
        }

        public bool GetAimInputHeld() //ADS HELD
        {
            if (CanProcessInput())
            {
                bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadAim) != 0f; //checks if gamepad
                bool i = isGamepad
                    ? (Input.GetAxis(GameConstants.k_ButtonNameGamepadAim) > 0f)
                    : Input.GetButton(GameConstants.k_ButtonNameAim); //if gamepad, returns whether or not the aim button is being pressed, else, checks if aim button is being pressed on keyboard 
                return i; //returns true if button is being pressed, false if not
            }

            return false;
        }

        public bool GetSprintInputHeld() //SPRINT HELD
        {
            if (CanProcessInput())
            {
                return Input.GetButton(GameConstants.k_ButtonNameSprint);
            }

            return false;
        }

        public bool GetCrouchInputDown() //CROUCH DOWN
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonNameCrouch);
            }

            return false;
        }

        public bool GetCrouchInputReleased() //CROUCH RELEASED
        {
            if (CanProcessInput())
            {
                return Input.GetButtonUp(GameConstants.k_ButtonNameCrouch);
            }

            return false;
        }

        public bool GetReloadButtonDown() //RELOAD DOWN
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown(GameConstants.k_ButtonReload);
            }

            return false;
        }

        public int GetSwitchWeaponInput() //SWITCH WEAPON -> used in PlayerWeaponsManager
        {
            if (CanProcessInput())
            {

                bool isGamepad = Input.GetAxis(GameConstants.k_ButtonNameGamepadSwitchWeapon) != 0f;
                string axisName = isGamepad
                    ? GameConstants.k_ButtonNameGamepadSwitchWeapon
                    : GameConstants.k_ButtonNameSwitchWeapon; //checks if gamepad switch button or keyboard switch button

                if (Input.GetAxis(axisName) > 0f) 
                    return -1;
                else if (Input.GetAxis(axisName) < 0f)
                    return 1;
                else if (Input.GetAxis(GameConstants.k_ButtonNameNextWeapon) > 0f)
                    return -1;
                else if (Input.GetAxis(GameConstants.k_ButtonNameNextWeapon) < 0f)
                    return 1;
            }

            return 0;
        }

        public int GetSelectWeaponInput() //cycle through weapons via number keys
        {
            if (CanProcessInput()) //ensures the cursor is locked in the center of the window and the game is not ending
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    return 1;
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    return 2;
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                    return 3;
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                    return 4;
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                    return 5;
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                    return 6;
                else if (Input.GetKeyDown(KeyCode.Alpha7))
                    return 7;
                else if (Input.GetKeyDown(KeyCode.Alpha8))
                    return 8;
                else if (Input.GetKeyDown(KeyCode.Alpha9))
                    return 9;
                else
                    return 0;
            }

            return 0;
        }
        #endregion

        float GetMouseOrStickLookAxis(string mouseInputName, string stickInputName) //works for both mouse + gamepad
        {
            if (CanProcessInput())
            {
                // Check if this look input is coming from the mouse
                bool isGamepad = Input.GetAxis(stickInputName) != 0f; //set to true if axis of stickInputName argument is not 0
                float i = isGamepad ? Input.GetAxis(stickInputName) : Input.GetAxisRaw(mouseInputName); //i is set to axis of gamepad or mouse

                // handle inverting vertical input
                if (InvertYAxis)
                    i *= -1f;

                // apply sensitivity multiplier
                i *= LookSensitivity;

                if (isGamepad)
                {
                    // since mouse input is already deltaTime-dependant, only scale input with frame time if it's coming from sticks
                    i *= Time.deltaTime;
                }
                else
                {
                    // reduce mouse input amount to be equivalent to stick movement
                    i *= 0.01f;
#if UNITY_WEBGL
                    // Mouse tends to be even more sensitive in WebGL due to mouse acceleration, so reduce it even more
                    i *= WebglLookSensitivityMultiplier;
#endif
                }

                return i;
            }

            return 0f;
        } 
    }
}