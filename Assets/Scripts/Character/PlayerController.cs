using UnityEngine;
using static Models;


public class PlayerController : MonoBehaviour
{
    CharacterController characterController;

    Animator CharacterAnimator;

    PlayerInputActions PlayerInputActions;
    [HideInInspector]
    public Vector2 input_Movement;
    [HideInInspector]
    public Vector2 input_View;

    Vector3 playerMovement;


    [Header("Settings")]
    public PlayerSettingsModel settings;
    public bool isTargetMode;

    [Header("Camera")]    
    public Transform cameraTarger;
    public CameraController cameraController;

    [Header("Movement")]
    public float movementSpeedOffset = 1 ;
    public float movementSmoothdamp = 0.3f;
    public bool isWalking;
    public bool isSprinting;

    private float verticalSpeed; 
    private float targetVerticalSpeed;
    private float verticalSpeedVelocity;

    private float horizontalSpeed;
    private float targetHorizontalSpeed;
    private float horizontalSpeedVelocity;

    public Vector3 relativePlayerVelocity;
    

    [Header("Stats")]
    public PlayerStatsModel playerStats;

    [Header("Gravity")]
    public float gravity = 10;

    private Vector3 gravityDirection;

    [Header("Falling / Jumping")]
    public float fallingSpeed;
    public float fallingSpeedPeak;
    public float fallingThreashold;

    public bool jumpingTriggered;
    public bool fallingTriggered;

  //  public bool isMoving; 



    #region - Awake -
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        CharacterAnimator = GetComponent<Animator>();

        PlayerInputActions = new PlayerInputActions();

        PlayerInputActions.Movement.Movement.performed += x => input_Movement = x.ReadValue<Vector2>();
        PlayerInputActions.Movement.View.performed += x => input_View = x.ReadValue<Vector2>();



        PlayerInputActions.Actions.Jump.performed += x => Jump();
        


        PlayerInputActions.Actions.WalkingToggle.performed += x => ToggleWalking();

        PlayerInputActions.Actions.Sprinting.performed += x => Sprint();

        gravityDirection = Vector3.down;

    }

    
    #endregion

    #region - Jumping -
    private void Jump()
    {
        if (jumpingTriggered)
        {
            return;
        }


        if (IsMoving() && !isWalking)
        {
            CharacterAnimator.SetTrigger("RunningJump");

        }
        else
        {
            CharacterAnimator.SetTrigger("Jump");

        }
        jumpingTriggered = true;
        fallingTriggered = true;
       
    }
    public void ApplyJumpForce()
    {
       // currentGravity = settings.JumpingForce;
    }
    #endregion


    #region - Sprinting -
    private void Sprint()
    {
        if (!CanSprint())
        {
            return;
        }
        if (playerStats.Stamina > (playerStats.MAxStamina / 4))
        {
            isSprinting = true;
        }
    }
    private bool CanSprint()
    {
        if (isTargetMode)
        {
            return false;
        }
        var SprintFalloff = 0.8f;

        if ((input_Movement.y < 0 ? input_Movement.y * -1 : input_Movement.y) < SprintFalloff && (input_Movement.x < 0 ? input_Movement.x * -1 : input_Movement.x) < SprintFalloff)
        {
            return false;
        }
        Debug.Log(input_Movement.y);

        return true;
    }
    public void CalculateSprint()
    {
        if (!CanSprint())
        {
            isSprinting = false;
        }
        if (isSprinting)
        {
            if (playerStats.Stamina > 0)
            {
                playerStats.Stamina -= playerStats.StaminaDrain * Time.deltaTime;
            }
            else
            {
                isSprinting = false;
            }
            playerStats.StaminaCurrentDelay = playerStats.StaminaDelay;
        }
        else
        {
            if (playerStats.StaminaCurrentDelay <= 0)
            {
                if (playerStats.Stamina < playerStats.MAxStamina)
                {
                    playerStats.Stamina += playerStats.StaminaRestore * Time.deltaTime;
                }
                else
                {
                    playerStats.Stamina = playerStats.MAxStamina;
                }
            }
            else
            {
                playerStats.StaminaCurrentDelay -= Time.deltaTime;
            }


        }
    }
    #endregion

    #region - Gravity -
    private bool isGrounded()
    {
        return characterController.isGrounded;
    }

    private bool isFalling()
    {

        if (fallingSpeed < fallingThreashold)
        {
            return true;
        }
        return false;

    }
    private void CalculateGravity()
    {
        Physics.gravity = gravityDirection * gravity;
    }

    private void CalculateFalling()
    {
        fallingSpeed = relativePlayerVelocity.y;
        if(isFalling() && fallingSpeed < fallingSpeedPeak )
        {
            fallingSpeedPeak = fallingSpeed;
        }
        if( isFalling() && !isGrounded() && !jumpingTriggered && !fallingTriggered)
        {
            //falling animation
            fallingTriggered = true;
            CharacterAnimator.SetTrigger("Falling");
        }
        if (fallingTriggered && isGrounded() && fallingSpeed < -0.1f)
        {
            fallingTriggered = false; 
            jumpingTriggered = false;

            if (fallingSpeedPeak < -7) 
            {
                CharacterAnimator.SetTrigger("HardLand");
              //  Debug.Log("hard land");
            }
            else
            {
                CharacterAnimator.SetTrigger("Land");
            //    Debug.Log("land");


            }

            fallingSpeedPeak = 0;
        }
    }

    #endregion


    #region - Movement -

    private void ToggleWalking()
    {
        isWalking = !isWalking;
    }

    public bool IsMoving()
    {   
        if (relativePlayerVelocity.x > 0.4f || relativePlayerVelocity.x < -0.4f)
        {
            return true;
        }
        if (relativePlayerVelocity.z > 0.4f || relativePlayerVelocity.z < -0.4f)
        {
            return true;
        }
            return false;
    }
    public void Movement()
    {
        CharacterAnimator.SetBool("IsTargetMode", isTargetMode);

        relativePlayerVelocity = transform.InverseTransformDirection(characterController.velocity);

        if (isTargetMode)
        {
            if (input_Movement.y > 0)
            {
                targetVerticalSpeed = (isWalking ? settings.WalkingSpeed : settings.RunningSpeed);
            }
            else
            {
                targetVerticalSpeed = (isWalking ? settings.WalkingBackwardSpeed : settings.RunningBackwardSpeed);
            }

            targetHorizontalSpeed = (isWalking ? settings.WalkingStrafingSpeed : settings.RunningStrafingSpeed);
        }
        else
        {
            var originalRotation = transform.rotation;
            transform.LookAt(playerMovement + transform.position, Vector3.up);
            var newRotation = transform.rotation;

            transform.rotation = Quaternion.Lerp(originalRotation, newRotation, settings.CharacterRotationSmoothdamp);
            float PlayerSpeed = 0;

            if (isSprinting)
            {
                PlayerSpeed = settings.SprintingSpeed;
            }
            else
            {
                PlayerSpeed = (isWalking ? settings.WalkingSpeed : settings.RunningSpeed);
            }
            targetVerticalSpeed = PlayerSpeed;
            targetHorizontalSpeed = PlayerSpeed;


        }
        targetVerticalSpeed = (targetVerticalSpeed * movementSpeedOffset) * input_Movement.y;      //// delete the ti;e.deltqti;e
        targetHorizontalSpeed = (targetHorizontalSpeed * movementSpeedOffset) * input_Movement.x; ////

        verticalSpeed = Mathf.SmoothDamp(verticalSpeed, targetVerticalSpeed, ref verticalSpeedVelocity, movementSmoothdamp);
        horizontalSpeed = Mathf.SmoothDamp(horizontalSpeed, targetHorizontalSpeed, ref horizontalSpeedVelocity, movementSmoothdamp);


        if (isTargetMode)
        {
            CharacterAnimator.SetFloat("Vertical", verticalSpeed);
            CharacterAnimator.SetFloat("Horizontal", horizontalSpeed);

        }
        else
        {
            float VerticalActualSpeed = verticalSpeed < 0 ? verticalSpeed * -1 : verticalSpeed;
            float HorizontalActualSpeed = horizontalSpeed < 0 ? horizontalSpeed * -1 : horizontalSpeed;


            float AnimatorVertical = VerticalActualSpeed > HorizontalActualSpeed ? VerticalActualSpeed : HorizontalActualSpeed;
            CharacterAnimator.SetFloat("Vertical", AnimatorVertical);

        }

        playerMovement = cameraController.transform.forward * verticalSpeed * Time.deltaTime;
        playerMovement += cameraController.transform.right * horizontalSpeed * Time.deltaTime;

     //   characterController.Move( );

    }
    
    #endregion


    #region -  Update -
    private void Update()
    {
        CalculateGravity();
        CalculateFalling();

        Movement();
        CalculateSprint();

       // isMoving = IsMoving();


    }
    #endregion


    #region - Enable/Desable -

    private void OnEnable()
    {
        PlayerInputActions.Enable();

    }
    private void OneDisable ()
    {
        PlayerInputActions.Disable();

    }

    #endregion
}
