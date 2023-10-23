using static Models;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    private Vector3 targetRotation;
    public GameObject yGimbal;
    private Vector3 yGimbalRotation;

    [Header("Settings")]
    public CameraSettingsModel settings;


    #region - Update -
    private void Update()
    {  
        CameraRotation();
        FollowPlayerCaameraTarget();
    }

    #endregion

    #region - Position/Rotation -
    private void CameraRotation()
    {
        var viewInput = playerController.input_View;


        targetRotation.y += (settings.InvertedX ? -(viewInput.x * settings.SensitivityX) : (viewInput.x * settings.SensitivityX)) * Time.deltaTime;


        transform.rotation = Quaternion.Euler(targetRotation);


        yGimbalRotation.x += (settings.InvertedY ? (viewInput.y * settings.SensitivityY) : -(viewInput.y * settings.SensitivityY)) * Time.deltaTime;
        yGimbalRotation.x = Mathf.Clamp(yGimbalRotation.x, settings.YClampMin, settings.YClampMax);
        yGimbal.transform.localRotation = Quaternion.Euler(yGimbalRotation); //rotation vs localRotation ? 

        if (playerController.isTargetMode)
        {
        
            var currentRotation = playerController.transform.rotation;


            var newRotation = currentRotation.eulerAngles;
            newRotation.y = targetRotation.y;  //difference between lerp and smoothdmp ? 

            currentRotation = Quaternion.Lerp(currentRotation, Quaternion.Euler(newRotation), settings.CharacterRotationSmoothdamp);
            playerController.transform.rotation = currentRotation;
        }

    }
    private void FollowPlayerCaameraTarget()
    {
        //   transform.position = Vector3.SmoothDamp(transform.position, playerController.cameraTarger.position, ref movementVelocity, movementSmoothTime);
        transform.position = playerController.cameraTarger.position;

    }
    #endregion
}
