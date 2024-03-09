using UnityEngine;
using UnityEngine.InputSystem;
using VIVE.OpenXR;

namespace EyeTracking
{
    public static class EyeTrackingUtils
    {
        public static bool GetTracked(InputActionReference actionReference)
        {
            bool tracked = false;

            if (OpenXRHelper.VALIDATE(actionReference, out string value))
            {
                if (actionReference.action.activeControl.valueType == typeof(UnityEngine.XR.OpenXR.Input.Pose))
                {
                    tracked = actionReference.action.ReadValue<UnityEngine.XR.OpenXR.Input.Pose>().isTracked;
                }
            }
            else
            {
                Debug.Log(value);
            }

            return tracked;
        }

        public static Vector3 GetDirection(InputActionReference actionReference)
        {
            Quaternion rotation = Quaternion.identity;

            if (OpenXRHelper.VALIDATE(actionReference, out string value))
            {
                if (actionReference.action.activeControl.valueType == typeof(UnityEngine.XR.OpenXR.Input.Pose))
                {
                    rotation = actionReference.action.ReadValue<UnityEngine.XR.OpenXR.Input.Pose>().rotation;
                    return (rotation * Vector3.forward);
                }
            }
            return Vector3.forward;
        }
    }
}