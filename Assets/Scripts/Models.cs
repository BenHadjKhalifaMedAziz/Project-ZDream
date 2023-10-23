using System;
using UnityEngine;

public static class Models
{
    #region - Player -


    [Serializable]
    public class CameraSettingsModel
    {
        [Header("Camera Settings")]
        public float SensitivityX;
        public bool InvertedX;

        public float SensitivityY;
        public bool InvertedY;

        public float YClampMin = -40f;
        public float YClampMax = 40f;

        [Header("character")]
        public float CharacterRotationSmoothdamp = 1f;
    }


    [Serializable]
    public class PlayerSettingsModel
    {
        
        public float CharacterRotationSmoothdamp = 0.6f;
        [Header ("Movement speed")]
        public float WalkingSpeed ;
        public float RunningSpeed;
        public float WalkingBackwardSpeed;
        public float RunningBackwardSpeed;

        public float WalkingStrafingSpeed;
        public float RunningStrafingSpeed;

        public float SprintingSpeed;

        [Header("Jumping")]
        public float JumpingForce;

    }

    [Serializable]
    public class PlayerStatsModel
    {
        public float Stamina;
        public float MaxStamina; 
        public float StaminaDrain;
        public float StaminaRestore;
        public float StaminaDelay;
        public float StaminaCurrentDelay;

    }

    #endregion

}
