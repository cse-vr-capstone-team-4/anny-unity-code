using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.MagicLeap;
using UnityEngine;
using TMPro;

public enum CameraState
{
    OverviewMain, CloseUpMain, FirstPersonMain, OverviewIslandOne, CloseUpIslandOne, FirstPersonIslandOne,
    OverviewIslandTwo, CloseUpIslandTwo, FirstPersonIslandTwo, OverviewResIslandOne, CloseUpResIslandOne,
    OverviewResIslandTwo, CloseUpResIslandTwo
};

public class ControllerScript : MonoBehaviour
{
    private MLInputController _controller;
    private const float _rotationSpeed = 30.0f;
    private const float _distance = 2.0f;
    private const float _moveSpeed = 1.2f;
    private bool _bumper = false;
    private bool _zoomin = false;
    private bool _trigger = false;
    private bool _zoomout = false;
    private bool animating = false;
    private bool zoomedIn = false;
    private bool hasRun = false;
    private int ctr = 0;
    private CameraState _target;

    public GameObject resource;
    public GameObject soldier;
    public GameObject zoomoutReady;
    private TextMeshPro zoomoutReadyTM;

    private Vector3 _startPosition;
    private Vector3 _startScale;
    private Vector3 _endPosition;
    private Vector3 _endScale;
    private Dictionary<CameraState, CameraState> zoominDic = new Dictionary<CameraState, CameraState>();
    private Dictionary<CameraState, CameraState> zoomoutDic = new Dictionary<CameraState, CameraState>();





    public static CameraState currentState = CameraState.OverviewMain;
    public static CameraState targetstate;
    private GameObject retState;

    public float secondsBetweenSpawn = 5.0f;
    private float elapsedTime = 0.0f;
    private float elapsedTimeSoldier = 0.0f;
    private float soldierDisappearTime = 3.0f;
    private bool _alreadyIncremented = false;


    void Awake()
    {
        MLInput.Start();
        MLInput.OnControllerButtonDown += OnButtonDown;
        MLInput.OnControllerButtonUp += OnButtonUp;
        soldier.SetActive(false);

        _controller = MLInput.GetController(MLInput.Hand.Left);
        zoominDic.Add(CameraState.OverviewMain, CameraState.CloseUpMain);
        zoominDic.Add(CameraState.CloseUpMain, CameraState.FirstPersonMain);
        zoominDic.Add(CameraState.OverviewIslandOne, CameraState.CloseUpIslandOne);
        zoominDic.Add(CameraState.CloseUpIslandOne, CameraState.FirstPersonIslandOne);
        zoominDic.Add(CameraState.OverviewIslandTwo, CameraState.CloseUpIslandTwo);
        zoominDic.Add(CameraState.CloseUpIslandTwo, CameraState.FirstPersonIslandTwo);
        zoominDic.Add(CameraState.OverviewResIslandOne, CameraState.CloseUpResIslandOne);
        zoominDic.Add(CameraState.OverviewResIslandTwo, CameraState.CloseUpResIslandTwo);

        zoomoutDic.Add(CameraState.FirstPersonMain, CameraState.CloseUpMain);
        zoomoutDic.Add(CameraState.CloseUpMain, CameraState.OverviewMain);
        zoomoutDic.Add(CameraState.FirstPersonIslandOne, CameraState.CloseUpIslandOne);
        zoomoutDic.Add(CameraState.CloseUpIslandOne, CameraState.OverviewIslandOne);
        zoomoutDic.Add(CameraState.FirstPersonIslandTwo, CameraState.CloseUpIslandTwo);
        zoomoutDic.Add(CameraState.CloseUpIslandTwo, CameraState.OverviewIslandTwo);
        zoomoutDic.Add(CameraState.CloseUpResIslandOne, CameraState.OverviewResIslandOne);
        zoomoutDic.Add(CameraState.CloseUpResIslandTwo, CameraState.OverviewResIslandTwo);

        zoomoutReadyTM = zoomoutReady.GetComponent<TextMeshPro>();
        zoomoutReadyTM.text = "Zoomout Ready";
        zoomoutReady.SetActive(false);
    }

    void OnDestroy()
    {
        MLInput.OnControllerButtonDown -= OnButtonDown;
        MLInput.OnControllerButtonUp -= OnButtonUp;
        MLInput.Stop();
    }

    void Update()
    {

        retState = MagicLeap.VirtualPointer.island;
        if (retState == GameObject.Find("GameObject3") && (currentState == CameraState.OverviewMain || currentState == CameraState.OverviewIslandOne
                                                           || currentState == CameraState.OverviewIslandTwo || currentState == CameraState.OverviewResIslandOne || currentState == CameraState.OverviewResIslandTwo))
        {
            currentState = CameraState.OverviewIslandOne;
        }
        else if (retState == GameObject.Find("GameObject2") && (currentState == CameraState.OverviewMain || currentState == CameraState.OverviewIslandOne
                                                           || currentState == CameraState.OverviewIslandTwo || currentState == CameraState.OverviewResIslandOne || currentState == CameraState.OverviewResIslandTwo))
        {
            currentState = CameraState.OverviewIslandTwo;
        }
        else if (retState == GameObject.Find("GameObject1") && (currentState == CameraState.OverviewMain || currentState == CameraState.OverviewIslandOne
                                                           || currentState == CameraState.OverviewIslandTwo || currentState == CameraState.OverviewResIslandOne || currentState == CameraState.OverviewResIslandTwo))
        {
            currentState = CameraState.OverviewMain;
        }
        else if (retState == GameObject.Find("GameObject4") && (currentState == CameraState.OverviewMain || currentState == CameraState.OverviewIslandOne
                                                         || currentState == CameraState.OverviewIslandTwo || currentState == CameraState.OverviewResIslandOne || currentState == CameraState.OverviewResIslandTwo))
        {
            currentState = CameraState.OverviewResIslandOne;
        }
        else if (retState == GameObject.Find("GameObject5") && (currentState == CameraState.OverviewMain || currentState == CameraState.OverviewIslandOne
                                                         || currentState == CameraState.OverviewIslandTwo || currentState == CameraState.OverviewResIslandOne || currentState == CameraState.OverviewResIslandTwo))
        {
            currentState = CameraState.OverviewResIslandTwo;
        }


        if (_zoomin && !hasRun)
        {
            //CameraState targetstate;
            if (zoominDic.TryGetValue(currentState, out targetstate))
            {
                switchCamera(targetstate);
                hasRun = true;
            }
        }
        else if (_zoomout && !hasRun)
        {
            //CameraState targetstate;
            if (zoomoutDic.TryGetValue(currentState, out targetstate))
            {
                switchCamera(targetstate);
                hasRun = true;
            }
        }
        if (!do_animation())
        {
            currentState = _target;
        }
        CheckControl();

        elapsedTime += Time.deltaTime;
        elapsedTimeSoldier += Time.deltaTime;


        if (elapsedTime > secondsBetweenSpawn)
        {
            // Debug.Log(true); 
             //resource.SetActive(true);
            resource.transform.localScale = new Vector3(3, 3, 3);
        }
        if (elapsedTimeSoldier > soldierDisappearTime)
        {
            soldier.SetActive(false);
        }
       
    }

    void switchCamera(CameraState target)
    {
        Vector3 startPosition = Vector3.zero;
        Vector3 endPosition = Vector3.zero;
        //Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);
        //Vector3 endScale = new Vector3(1.0f, 1.0f, 1.0f);

        if (!animating)
        {
            animating = true;
            _target = target;
            if (currentState == CameraState.OverviewMain)
            {
                _startPosition = new Vector3(0.17f, -0.45f, 1.67f);
                _startScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            else if (currentState == CameraState.CloseUpMain)
            {
                _startPosition = new Vector3(5.57f, -5.02f, 3.7f);
                _startScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else if (currentState == CameraState.FirstPersonMain)
            {
                _startPosition = new Vector3(0.76f, -4.71f, -9.54f);
                _startScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else if (currentState == CameraState.OverviewIslandOne)
            {
                _startPosition = new Vector3(0.17f, -0.45f, 1.67f);
                _startScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            else if (currentState == CameraState.CloseUpIslandOne)
            {
                _startPosition = new Vector3(21.02f, -2.73f, 8.54f);
                _startScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else if (currentState == CameraState.FirstPersonIslandOne)
            {
                _startPosition = new Vector3(41.35f, -0.37f, 3.14f);
                _startScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else if (currentState == CameraState.OverviewIslandTwo)
            {
                _startPosition = new Vector3(0.17f, -0.45f, 1.67f);
                _startScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            else if (currentState == CameraState.CloseUpIslandTwo)
            {
                _startPosition = new Vector3(-12.39f, -2.4f, 11.65f);
                _startScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else if (currentState == CameraState.FirstPersonIslandTwo)
            {
                _startPosition = new Vector3(-24.12f, -0.7f, 10.56f);
                _startScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else if (currentState == CameraState.OverviewResIslandOne)
            {
                _startPosition = new Vector3(0.17f, -0.45f, 1.67f);
                _startScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            else if (currentState == CameraState.CloseUpResIslandOne)
            {
                _startPosition = new Vector3(6.33f, -2.34f, -6.53f);
                _startScale = new Vector3(0.5f, 0.5f, 0.5f);
            }

            else if (currentState == CameraState.OverviewResIslandTwo)
            {
                _startPosition = new Vector3(0.17f, -0.45f, 1.67f);
                _startScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            else if (currentState == CameraState.CloseUpResIslandTwo)
            {
                _startPosition = new Vector3(-7.74f, -0.3f, -12.87f);
                _startScale = new Vector3(0.5f, 0.5f, 0.5f);
            }


            if (target == CameraState.OverviewMain)
            {
                _endPosition = new Vector3(0.17f, -0.45f, 1.67f);
                _endScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            else if (target == CameraState.CloseUpMain)
            {
                _endPosition = new Vector3(5.57f, -5.02f, 3.7f);
                _endScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else if (target == CameraState.FirstPersonMain)
            {
                _endPosition = new Vector3(0.76f, -4.71f, -9.54f);
                _endScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else if (target == CameraState.OverviewIslandOne)
            {
                _endPosition = new Vector3(0.17f, -0.45f, 1.67f);
                _endScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            else if (target == CameraState.CloseUpIslandOne)
            {
                _endPosition = new Vector3(21.02f, -2.73f, 8.54f);
                _endScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else if (target == CameraState.FirstPersonIslandOne)
            {
                _endPosition = new Vector3(41.35f, -0.37f, 3.14f);
                _endScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else if (target == CameraState.OverviewIslandTwo)
            {
                _endPosition = new Vector3(0.17f, -0.45f, 1.67f);
                _endScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            else if (target == CameraState.CloseUpIslandTwo)
            {
                _endPosition = new Vector3(-12.39f, -2.4f, 11.65f);
                _endScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else if (target == CameraState.FirstPersonIslandTwo)
            {
                _endPosition = new Vector3(-24.12f, -0.7f, 10.56f);
                _endScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else if (target == CameraState.OverviewResIslandOne)
            {
                _endPosition = new Vector3(0.17f, -0.45f, 1.67f);
                _endScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            else if (target == CameraState.CloseUpResIslandOne)
            {
                _endPosition = new Vector3(6.33f, -2.34f, -6.53f);
                _endScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else if (target == CameraState.OverviewResIslandTwo)
            {
                _endPosition = new Vector3(0.17f, -0.45f, 1.67f);
                _endScale = new Vector3(0.05f, 0.05f, 0.05f);
            }
            else if (target == CameraState.CloseUpResIslandTwo)
            {
                _endPosition = new Vector3(-7.74f, -0.3f, -12.87f);
                _endScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }
    }

    bool do_animation()
    {
        Vector3 startPosition = Vector3.zero;
        Vector3 endPosition = Vector3.zero;
        Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 endScale = new Vector3(1.0f, 1.0f, 1.0f);

        if (animating)
        {

            startPosition = _startPosition;
            endPosition = _endPosition;
            startScale = _startScale;
            endScale = _endScale;
            float t = ctr / 100f;
            GameObject.Find("TotalScene").transform.localPosition = new Vector3(Mathf.SmoothStep(startPosition.x, endPosition.x, t), Mathf.SmoothStep(startPosition.y, endPosition.y, t), Mathf.SmoothStep(startPosition.z, endPosition.z, t));
            GameObject.Find("TotalScene").transform.localScale = new Vector3(Mathf.SmoothStep(startScale.x, endScale.x, t), Mathf.SmoothStep(startScale.y, endScale.y, t), Mathf.SmoothStep(startScale.z, endScale.z, t));

            ctr++;

            if (ctr > 100)
            {
                animating = false;
                ctr = 0;
                return false;
            }
        }
        return true;
    }

    // spawn resources check trigger
    void CheckControl()
    {
        if (_controller.TriggerValue > 0.2f  && MagicLeap.VirtualPointer.crate)
        {
            if (resource.activeSelf) {

                if (!_alreadyIncremented)
                {
                    //resource.SetActive(false);
                    resource.transform.localScale = new Vector3(0, 0, 0);
                    soldier.SetActive(true);
                    CombatController.canBuildSoldier += 2;
                    Debug.Log("DEBUG: Open Crate, Can build soldier:" + CombatController.canBuildSoldier);
                    elapsedTimeSoldier = 0;
                    elapsedTime = 0;
                    _alreadyIncremented = true;
                }
            }
        }
        else
        {
            _alreadyIncremented = false;
        }

        // A note: zoomout ready
        if (_bumper && !animating && (DateTime.Now - pressTime).TotalMilliseconds > 500)
        {
            zoomoutReady.SetActive(true);
        } else {
            zoomoutReady.SetActive(false);
        }
    }

    DateTime pressTime;

    void OnButtonDown(byte controller_id, MLInputControllerButton button)
    {
        if ((button == MLInputControllerButton.Bumper))
        {
            _bumper = true;
            pressTime = DateTime.Now;
        }
    }

    void OnButtonUp(byte controller_id, MLInputControllerButton button)
    {

        if ((button == MLInputControllerButton.Bumper))
        {
            if (_bumper)
            {
                zoomoutReady.SetActive(false);
                if ((DateTime.Now - pressTime).TotalMilliseconds > 500)
                {
                    
                    // bumper has been long pressed and released
                    _zoomout = true;
                    _zoomin = false;
                }
                else
                {
                    //bumper has been short pressed and released
                    _zoomin = true;
                    _zoomout = false;
                }

                //bumper has been released
                _bumper = false;
                hasRun = false;
            }
        }
    }

}