using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static EyeTracking.EyeTrackingUtils;

namespace EyeTracking
{
    public class GazeRaycastRing : MonoBehaviour
    {   
        [SerializeField]
        private InputActionReference m_EyePose = null;
        [SerializeField]
        private bool useEyeTracking = true;
        [SerializeField]
        private GameObject indicatorPrefab = null;
        private Vector3 pointerLocalOffset;
        private Vector3 physicsWorldPosition;
        private Vector2 graphicScreenPosition;
        private GameObject indicator;

        private void Start()
        {
            // instantiate the indicator prefab
            indicator = Instantiate(indicatorPrefab);
    }

        bool UseEyeData(out Vector3 direction)
        {
            bool tracked = GetTracked(m_EyePose);
            bool useEye = useEyeTracking && tracked;

            direction = GetDirection(m_EyePose);

            if (!useEye)
            {
                //Vector3.forward
                direction = Vector3.forward;
            }

            return useEye;
        }

        private void Update()
        {
            UpdatePointerDataPosition();
        }

        private void UpdatePointerDataPosition()
        {
            /// 1. Calculate the pointer offset in "local" space.
            pointerLocalOffset = Vector3.forward;
            if (UseEyeData(out Vector3 direction))
            {
                pointerLocalOffset = direction;

                // Revise the offset from World space to Local space.
                // OpenXR always uses World space.
                pointerLocalOffset = Quaternion.Inverse(transform.rotation) * pointerLocalOffset;
                // if looking forward, the pointerLocalOffset is (0, 0, 1)
                //print(direction);
            }
            
            /// 2. Calculate the pointer position in "world" space.
            Vector3 rotated_offset = transform.rotation * pointerLocalOffset;
            physicsWorldPosition = transform.position + rotated_offset;
            indicator.transform.position = physicsWorldPosition;
            // draw a sphere on the screen
            
                
        }
    }
}