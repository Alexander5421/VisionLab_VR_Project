using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using VIVE.OpenXR;

public class EyeDataText : MonoBehaviour
{
    public InputActionReference gazeInput;
    public InputActionAsset actionAsset;
    public TextMeshProUGUI text;
    public GameObject GazePlane;
    public GameObject runner;

    private string filePath;

    private void Start()
    {
        // Define the file path to the Assets/UntrackData_Eye folder
        filePath = Path.Combine(Application.dataPath, "UntrackData_Eye", "EyeGazeData.csv");

        // Check if the file exists, if not, write headers
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Timestamp,ExperimentType,TrialID,GazeOriginX,GazeOriginY,GazeOriginZ,GazeDirectionX,GazeDirectionY,GazeDirectionZ\n");
        }
    }


    bool IsTracked(InputActionReference actionReference)
    {
        bool tracked = false;
        if (OpenXRHelper.VALIDATE(actionReference, out string value))
        {
            if (actionReference.action.activeControl.valueType == typeof(UnityEngine.InputSystem.XR.PoseState))
            {
                tracked = actionReference.action.ReadValue<UnityEngine.InputSystem.XR.PoseState>().isTracked;
                Debug.Log(tracked);
            }
        }
        else
        {
            Debug.Log(value);
        }

        return tracked;
    }
    bool getTracked(InputActionReference actionReference)
    {
        bool tracked = false;

        if (OpenXRHelper.VALIDATE(actionReference, out string value))
        {
#if USE_INPUT_SYSTEM_POSE_CONTROL // Scripting Define Symbol added by using OpenXR Plugin 1.6.0.
            if (actionReference.action.activeControl.valueType == typeof(UnityEngine.InputSystem.XR.PoseState))
#else
            if (actionReference.action.activeControl.valueType == typeof(UnityEngine.XR.OpenXR.Input.Pose))
#endif
            {
#if USE_INPUT_SYSTEM_POSE_CONTROL // Scripting Define Symbol added by using OpenXR Plugin 1.6.0.
                tracked = actionReference.action.ReadValue<UnityEngine.InputSystem.XR.PoseState>().isTracked;
#else
                tracked = actionReference.action.ReadValue<UnityEngine.XR.OpenXR.Input.Pose>().isTracked;
#endif
            }
        }
        else
        {
            print(value);
        }

        return tracked;
    }

    private void OnEnable()
    {
        if (actionAsset == null)
        {
            Debug.LogError("Please assign a gaze input action to the EyeDataText component");
        }
        // enable gaze input
        actionAsset.Enable();
    }

    private void OnDisable()
    {
        // disable gaze input
        actionAsset.Disable();
    }

    // Update text with eye tracking data
    void Update()
    {
        bool tracked = getTracked(gazeInput);
        string eyeDataInfo = "";
        if (tracked)
        {
            var pose = gazeInput.action.ReadValue<UnityEngine.XR.OpenXR.Input.Pose>();
            var gazeDirection = pose.rotation * Vector3.forward;
            var gazeOrigin = pose.position;

            Ray gazeRay = new Ray(gazeOrigin, gazeDirection);
            if (Physics.Raycast(gazeRay, out RaycastHit hit))
            {
                if (hit.collider != null && hit.collider.gameObject == GazePlane)
                {
                    Vector3 hitPosition = hit.point;
                    print("Gaze at " + hitPosition);
                }
            }
            eyeDataInfo = "GazeDirection: " + gazeDirection + "\n" +
                          "GazeOrigin: " + gazeOrigin + "\n";
            string experiment_info = runner.GetComponent<TrialRunner>().GetExperimentInfo();
            string[] parts = experiment_info.Split(new[] { ' ' }, StringSplitOptions.None);
            // Write data to CSV file
            if (parts[0] != null && parts[0] != "")
            {
                string timestamp = DateTime.UtcNow.ToString("o"); // ISO 8601 format for timestamp
                string dataLine = $"{timestamp},{parts[0]},{parts[1]},{gazeOrigin.x},{gazeOrigin.y},{gazeOrigin.z}," +
                                   $"{gazeDirection.x},{gazeDirection.y},{gazeDirection.z}\n";
                File.AppendAllText(filePath, dataLine);
            }
            

            Debug.DrawRay(gazeOrigin, gazeDirection * 10.0f, Color.red);
        }
        else
        {
            eyeDataInfo = "Eye tracking data not available";
        }
        text.text = eyeDataInfo;
    }
}
