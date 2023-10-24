using UnityEngine;
using static Models;


public class PlayerController : MonoBehaviour
{
    Rigidbody characterRigidBody;

    Animator characterAnimator;


    PlayerInputActions playerInputActions;

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
    [HideInInspector]
    public bool isAiming;
    private bool isAimingButtonHold = false;


    [Header("Movement")]
    public float movementSpeedOffset = 1;
    public float movementSmoothdamp = 0.3f;

    public bool isWalking;
    [HideInInspector]
    public bool isSprinting;

    private float verticalSpeed;
    private float targetVerticalSpeed;
    private float verticalSpeedVelocity;

    private float horizontalSpeed;
    private float targetHorizontalSpeed;
    private float horizontalSpeedVelocity;

    private Vector3 relativePlayerVelocity;



    [Header("Stats")]
    public PlayerStatsModel playerStats;

    [Header("Gravity")]
    public float gravity = 10;
    public LayerMask groundMask;
    private Vector3 gravityDirection;

    [Header("Falling / Jumping")]
    private float fallingSpeed;
    private float fallingSpeedPeak;
    public float fallingThreshold;
    public float fallingMovementSpeed;
    public float fallingRunningMovementSpeed;
    private bool jumpingTriggered;
    private bool fallingTriggered;

    //  public bool isMoving; 



    #region - Awake -
    private void Awake()
    {
        characterRigidBody = GetComponent<Rigidbody>();
        characterAnimator = GetComponent<Animator>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Movement.Movement.performed += x => input_Movement = x.ReadValue<Vector2>();
        playerInputActions.Movement.View.performed += x => input_View = x.ReadValue<Vector2>();

        playerInputActions.Actions.Jump.performed += x => Jump();

        playerInputActions.Actions.WalkingToggle.performed += x => ToggleWalking();
        playerInputActions.Actions.Sprinting.performed += x => Sprint();

        playerInputActions.Actions.Aiming.performed += x => OnAimingPerformed();
        playerInputActions.Actions.Aiming.canceled += x => OnAimingCanceled();

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
        jumpingTriggered = true;

        if (IsMoving() && !isWalking)
        {
            characterAnimator.SetTrigger("RunningJump");

        }

        else
        {
            characterAnimator.SetTrigger("Jump");

        }
    }

    public void ApplyJumpForce()
    {
        if (!IsGrounded())
        {
            return;
        }

        characterRigidBody.AddForce(transform.up * settings.JumpingForce, ForceMode.Impulse);
        fallingTriggered = true;
    }
    #endregion

    #region - Aiming -

    public void OnAimingPerformed()
    {
        if (!isAimingButtonHold)
        {
            isTargetMode = true ;
          
        }
    }
    public void OnAimingCanceled()
    {
        isTargetMode = false;
    }

    #endregion

    #region - Sprinting -
    private void Sprint()
    {
        if (!CanSprint())
        {
            return;
        }

        if (playerStats.Stamina > (playerStats.MaxStamina / 4))
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

        var sprintFalloff = 0.8f;

        if ((input_Movement.y < 0 ? input_Movement.y * -1 : input_Movement.y) < sprintFalloff && (input_Movement.x < 0 ? input_Movement.x * -1 : input_Movement.x) < sprintFalloff)
        {
            return false;
        }

        return true;
    }
    private void CalculateSprint()
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
                if (playerStats.Stamina < playerStats.MaxStamina)
                {
                    playerStats.Stamina += playerStats.StaminaRestore * Time.deltaTime;
                }
                else
                {
                    playerStats.Stamina = playerStats.MaxStamina;
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
    private bool IsGrounded()
    {
        if (Physics.CheckSphere(transform.position, 0.2f, groundMask))
        {
            return true;
        }

        return false;
    }

    private bool IsFalling()
    {

        if (fallingSpeed < fallingThreshold)
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

        if (fallingSpeed < fallingSpeedPeak && fallingSpeed < -0.1f && (fallingTriggered || jumpingTriggered))
        {
            fallingSpeedPeak = fallingSpeed;
        }

        if ((IsFalling() && !IsGrounded() && !jumpingTriggered && !fallingTriggered) || (jumpingTriggered && !fallingTriggered && !IsGrounded()))
        {
            fallingTriggered = true;
            characterAnimator.SetTrigger("Falling");
        }

        if (fallingTriggered && IsGrounded() && fallingSpeed < -0.1f)
        {
            fallingTriggered = false;
            jumpingTriggered = false;

            if (fallingSpeedPeak < -7)
            {
                characterAnimator.SetTrigger("HardLand");
            }
            else
            {
                characterAnimator.SetTrigger("Land");
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


    public bool IsInputMoving()
    {
        if (input_Movement.x > 0.2f || input_Movement.x < -0.2f)
        {
            return true;
        }

        if (input_Movement.y > 0.2f || input_Movement.y < -0.2f)
        {
            return true;
        }

        return false;
    }
    public void Movement()
    {
        characterAnimator.SetBool("IsTargetMode", isTargetMode);

        relativePlayerVelocity = transform.InverseTransformDirection(characterRigidBody.velocity);

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
            var orginalRotation = transform.rotation;
            transform.LookAt(playerMovement + transform.position, Vector3.up);
            var newRotation = transform.rotation;

            transform.rotation = Quaternion.Lerp(orginalRotation, newRotation, settings.CharacterRotationSmoothdamp);

            float playerSpeed = 0;

            if (isSprinting)
            {
                playerSpeed = settings.SprintingSpeed;
            }
            else
            {
                playerSpeed = (isWalking ? settings.WalkingSpeed : settings.RunningSpeed);
            }

            targetVerticalSpeed = playerSpeed;
            targetHorizontalSpeed = playerSpeed;
        }

        targetVerticalSpeed = (targetVerticalSpeed * movementSpeedOffset) * input_Movement.y;
        targetHorizontalSpeed = (targetHorizontalSpeed * movementSpeedOffset) * input_Movement.x;

        verticalSpeed = Mathf.SmoothDamp(verticalSpeed, targetVerticalSpeed, ref verticalSpeedVelocity, movementSmoothdamp);
        horizontalSpeed = Mathf.SmoothDamp(horizontalSpeed, targetHorizontalSpeed, ref horizontalSpeedVelocity, movementSmoothdamp);

        if (isTargetMode)
        {
            characterAnimator.SetFloat("Vertical", verticalSpeed);
            characterAnimator.SetFloat("Horizontal", horizontalSpeed);
        }
        else
        {
            float verticalActualSpeed = verticalSpeed < 0 ? verticalSpeed * -1 : verticalSpeed;
            float horizontalActualSpeed = horizontalSpeed < 0 ? horizontalSpeed * -1 : horizontalSpeed;

            float animatorVertical = verticalActualSpeed > horizontalActualSpeed ? verticalActualSpeed : horizontalActualSpeed;

            characterAnimator.SetFloat("Vertical", animatorVertical);
        }

        if (IsInputMoving())
        {
            playerMovement = cameraController.transform.forward * verticalSpeed;
            playerMovement += cameraController.transform.right * horizontalSpeed;
        }

        if (jumpingTriggered || IsFalling())
        {
            characterAnimator.applyRootMotion = false;
            characterRigidBody.AddForce(playerMovement * (isWalking ? fallingMovementSpeed : fallingRunningMovementSpeed));
        }
        else
        {
            characterAnimator.applyRootMotion = true;
        }

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
        playerInputActions.Enable();

    }
    private void OneDisable ()
    {
        playerInputActions.Disable();

    }

    #endregion

    #region - Gizmos -
    private void OnDrawGizmosSelected()
    {
       // Gizmos.color = Color.red;
       // Gizmos.DrawSphere(transform.position, 0.2f);
    }
    #endregion
}
