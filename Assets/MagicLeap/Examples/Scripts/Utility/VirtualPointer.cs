// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using MagicLeap;

namespace MagicLeap
{
    /// <summary>
    /// This class represents the controller and its interactions.
    /// This works hand-in-hand with WorldButton.
    /// </summary>
    public class VirtualPointer : MonoBehaviour
    {
        #region Private Variables


        [SerializeField, Tooltip("ControllerConnectionHandler reference.")]
        private ControllerConnectionHandler _controllerConnectionHandler;

        [SerializeField, Space, Tooltip("Pointer Ray")]
        private Transform _pointerRay;

        [SerializeField, Tooltip("Pointer Light")]
        private Light _pointerLight;

        [SerializeField, Tooltip("Color of the pointer light when no hit is detected")]
        private Color _pointerLightColorNoHit = Color.black;
        [SerializeField, Tooltip("Color of the pointer light when a hit is detected")]
        private Color _pointerLightColorHit = Color.yellow;
        [SerializeField, Tooltip("Color of the pointer light when a button is pressed while a hit is detected")]
        private Color _pointerLightColorHitPress = Color.green;

        private MediaPlayerButton _lastButtonHit;
        private bool _isGrabbing = false;

        private GameObject[] islands;
        private GameObject cube;
        #endregion // Private Properties

        public static GameObject island;
        public static bool crate;
        public static GameObject crateObject;
        public static GameObject knight;

        #region Unity Methods
        void Awake()
        {
            if (_controllerConnectionHandler == null)
            {
                Debug.LogError("Error: VirtualPointer._controllerConnectionHandler is not set, disabling script.");
                enabled = false;
                return;
            }

            islands = new GameObject[5];
            for (int i = 1; i <= 5; i++)
            {
                if (GameObject.Find("GameObject" + i) != null)
                {
                    Debug.Log("Gameobject" + i + "created!");
                    Debug.unityLogger.Log("islands", "Gameobject" + i + " created!");
                    islands[i - 1] = GameObject.Find("GameObject" + i);
                }
            }

            MLInput.OnControllerButtonDown += HandleControllerButtonDown;
            MLInput.OnControllerButtonUp += HandleControllerButtonUp;
            MLInput.OnTriggerDown += HandleTriggerDown;
            MLInput.OnTriggerUp += HandleTriggerUp;

        }

        void OnDestroy()
        {
            MLInput.OnControllerButtonDown -= HandleControllerButtonDown;
            MLInput.OnControllerButtonUp -= HandleControllerButtonUp;
            MLInput.OnTriggerDown -= HandleTriggerDown;
            MLInput.OnTriggerUp -= HandleTriggerUp;
        }

        void Update()
        {
            if (!_isGrabbing)
            {
                RaycastHit[] hit = new RaycastHit[1];

                if (Physics.RaycastNonAlloc(_pointerRay.position, _pointerRay.forward, hit) > 0)
                {

                    if (hit[0].transform && hit[0].transform.gameObject)
                    {

                        GameObject g = hit[0].transform.gameObject; //  "GrameObject1-5"
                        Debug.Log("DEBUG: hitted object name: " + g.name);

                        if (g.CompareTag("Knight") || g.transform.parent.gameObject.CompareTag("Knight") || g.transform.parent.parent.gameObject.CompareTag("Knight"))
                        {
                            if (g.CompareTag("Knight"))
                            {
                                knight = g;
                            }
                            else if (g.transform.parent.gameObject.CompareTag("Knight"))
                            {
                                knight = g.transform.parent.gameObject;
                            }
                            else 
                            {
                                knight = g.transform.parent.parent.gameObject;
                            }
                            crate = false;
                        } else if (g.CompareTag("Crate") || g.transform.parent.gameObject.CompareTag("Crate") || g.transform.parent.gameObject.CompareTag("Crate")) 
                        {
                            crate = true;
                            if (g.CompareTag("Crate"))
                            {
                                crateObject = g;
                            }
                            else if (g.transform.parent.gameObject.CompareTag("Crate"))
                            {
                                crateObject = g.transform.parent.gameObject;
                            }
                            else
                            {
                                crateObject = g.transform.parent.parent.gameObject;
                            }
                            knight = null;
                        }
                        else
                        {
                            knight = null;
                            crate = false;
                            GameObject g_parent = null;
                            if (g.transform.parent && g.transform.parent.gameObject)
                            {
                                g_parent = g.transform.parent.gameObject; // "Scene"
                            }

                            while (g_parent && g_parent.name != "Scene")
                            {
                                g = g_parent;
                                g_parent = g.transform.parent.gameObject;
                            }

                            if (g.name.StartsWith("GameObject"))
                            {
                                // g should have the full island
                                // Outline of the full island
                                Outline o = g.GetComponent<Outline>();
                                island = g;
                                // Check for null
                                if (o != null)
                                {
                                    Debug.Log("Hit " + hit[0].collider.gameObject.name);
                                    if (!o.isActiveAndEnabled)
                                        o.enabled = true;
                                }
                            }
                        }

                    }

                    UpdatePointer(hit[0].point);
                }
                else
                {
                    //_lastButtonHit = null;
                    ClearPointer();

                    //clear all outlines
                    foreach (GameObject island in islands)
                    {
                        Outline outline = island.GetComponent<Outline>();
                        if (outline.isActiveAndEnabled)
                        {
                            Debug.Log("Clear outline of " + island.name);
                            outline.enabled = false;
                        }
                    }

                }
            }
            else if (_isGrabbing)
            {
                Debug.Log("isgrabbing");
                Debug.unityLogger.Log("islands", "Isgrabbing");

                // _isGrabbing already guarantees that _lastButtonHit is not null
                // but just in case the actual button gets destroyed in
                // the middle of the grab, let's still check

                if (_lastButtonHit != null && _lastButtonHit.OnControllerDrag != null)
                {
                    _lastButtonHit.OnControllerDrag(_controllerConnectionHandler.ConnectedController);
                }
            }
        }
        #endregion // Unity Methods

        #region Private Methods
        private void UpdatePointer(Vector3 hitPosition)
        {
            Vector3 pointerScale = _pointerRay.localScale;
            pointerScale.z = Vector3.Distance(_pointerRay.position, hitPosition);
            _pointerRay.localScale = pointerScale;

            _pointerLight.transform.position = hitPosition;
        }

        private void ClearPointer()
        {
            Vector3 pointerScale = _pointerRay.localScale;
            pointerScale.z = 1.0f;
            _pointerRay.localScale = pointerScale;

            _pointerLight.transform.position = transform.position;
            _pointerLight.color = _pointerLightColorNoHit;
        }
        #endregion // Private Methods

        #region Event Handlers
        private void HandleControllerButtonDown(byte controllerId, MLInputControllerButton button)
        {
            if (_controllerConnectionHandler.IsControllerValid(controllerId) && _lastButtonHit != null && !_isGrabbing)
            {
                if (_lastButtonHit.OnControllerButtonDown != null)
                {
                    _lastButtonHit.OnControllerButtonDown(button);
                }
                _pointerLight.color = _pointerLightColorHitPress;
                _isGrabbing = true;
            }
        }

        private void HandleControllerButtonUp(byte controllerId, MLInputControllerButton button)
        {
            if (_controllerConnectionHandler.IsControllerValid(controllerId))
            {
                if (_lastButtonHit != null)
                {
                    if (_lastButtonHit.OnControllerButtonUp != null)
                    {
                        _lastButtonHit.OnControllerButtonUp(button);
                    }
                    _pointerLight.color = _pointerLightColorHit;
                    _isGrabbing = false;
                }
                else
                {
                    _pointerLight.color = _pointerLightColorNoHit;
                }
            }
        }

        private void HandleTriggerDown(byte controllerId, float triggerValue)
        {
            if (_controllerConnectionHandler.IsControllerValid(controllerId) && _lastButtonHit != null && !_isGrabbing)
            {
                if (_lastButtonHit.OnControllerTriggerDown != null)
                {
                    _lastButtonHit.OnControllerTriggerDown(triggerValue);
                }
                _pointerLight.color = _pointerLightColorHitPress;
                _isGrabbing = true;
            }
        }

        private void HandleTriggerUp(byte controllerId, float triggerValue)
        {
            if (_controllerConnectionHandler.IsControllerValid(controllerId))
            {
                if (_lastButtonHit != null)
                {
                    if (_lastButtonHit.OnControllerTriggerUp != null)
                    {
                        _lastButtonHit.OnControllerTriggerUp(triggerValue);
                    }
                    _pointerLight.color = _pointerLightColorHit;
                    _isGrabbing = false;
                }
                else
                {
                    _pointerLight.color = _pointerLightColorNoHit;
                }
            }
        }
        #endregion // Event Handlers
    }
}