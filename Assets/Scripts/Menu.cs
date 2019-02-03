using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.MagicLeap;
using MagicLeap;
using TMPro;

public class Menu : MonoBehaviour {

    private MLInputController _controller;
    public GameObject s1Count;
    public GameObject s1CountSelected;
    public GameObject state;
    public GameObject resCount;

    public GameObject mission1;
    public GameObject mission1Complete;
    public GameObject mission2;
    public GameObject mission2Complete;
    public GameObject allMissionsComplete;
    public GameObject startNote;
    public GameObject finalNote;
    public GameObject presshere;
    public GameObject island3Win;
    public GameObject island2Win;

    private TextMeshPro s1CountTM;
    private TextMeshPro s1CountSelectedTM;
    private TextMeshPro stateTM;
    //private TextMeshPro resCountTM;

    private TextMeshPro mission1CompleteTM;
    private TextMeshPro mission2CompleteTM;
    private TextMeshPro allMissionsCompleteTM;

    private int _i;
    private bool _alreadyIncremented;
    private bool _forceButton;
    private String stateName;
    private String last_stateName;

    void Awake()
    {
        MLInput.Start();
        _controller = MLInput.GetController(MLInput.Hand.Left);
        _alreadyIncremented = false;
        _forceButton = false;
        _i = 0;
        stateName = "Overview";
        last_stateName = "Overview";
        Debug.Log("Started Awake Method");

        s1CountTM = s1Count.GetComponent<TextMeshPro>();
        s1CountSelectedTM = s1CountSelected.GetComponent<TextMeshPro>();
        stateTM = state.GetComponent<TextMeshPro>();
        //resCountTM = resCount.GetComponent<TextMeshPro>();
        mission1CompleteTM = mission1Complete.GetComponent<TextMeshPro>();
        mission2CompleteTM = mission2Complete.GetComponent<TextMeshPro>();
        allMissionsCompleteTM = allMissionsComplete.GetComponent<TextMeshPro>();

        s1Count.SetActive(true);
        s1CountSelected.SetActive(false);
        state.SetActive(true);
        //resCount.SetActive(true);
        resCount.SetActive(false);

        mission1Complete.SetActive(true);
        mission2Complete.SetActive(true);
        allMissionsComplete.SetActive(false);
        startNote.SetActive(true);
        finalNote.SetActive(false);
        presshere.SetActive(true);
        island3Win.SetActive(false);
        island2Win.SetActive(false);

        s1CountTM.text = "<Soldier Availability>: " + CombatController.canBuildSoldier;
        s1CountSelectedTM.text = s1CountTM.text;
        stateTM.text = "<Current View>: " + stateName;
        //resCountTM.text = "<Resources To Collect>: " + ResourcesScript.currentResources.Count;
        mission1CompleteTM.text = "Misson 1: Conquer Island 3";
        mission2CompleteTM.text = "Misson 2: Conquer Island 2";
    }

    void OnDestroy()
    {
        MLInput.Stop();
    }

    void Update()
    {
        CheckControl();
        int remainCastlesCount = CombatController.remainCastle;
        if (remainCastlesCount != 0)
        {
            _i = CombatController.canBuildSoldier;
            s1CountTM.text = "<Soldier Availability>: " + _i;
            s1CountSelectedTM.text = "<Soldier Availability>: " + _i;

            // update mission state
            updateMissionState();
        }
        else {

            // update mission state
            updateMissionState();
        }

        int stateIdx = (int)ControllerScript.targetstate;
        String[] stateNames = {"Overview", "Closeup", "First Person"};
        // overview -> closeup -> firstperson -> closeup -> overview
        last_stateName = stateName;
        stateName = "";
        if (stateIdx < 9) 
        {
            stateName = stateNames[stateIdx % 3];
            // avoid from first person to overview
            if (stateName == "Overview" && last_stateName == "First Person")
            {
                stateName = "First Person";
            }

        } else { // >= 9
            stateName = stateNames[(stateIdx+1) % 2];
        }
        stateTM.text = "<Current View>: " + stateName;

        //resCountTM.text = "<Resources To Collect>: " + 1;
        //resCountTM.text = "<Resources To Collect>: " + ResourcesScript.currentResources.Count;

        if (_forceButton)
        {
            s1Count.SetActive(false);
            s1CountSelected.SetActive(true);
        }
        else
        {
            s1Count.SetActive(true);
            s1CountSelected.SetActive(false);
        }
    }

    private void updateMissionState() 
    {
        if (mission1 == null || !mission1.activeSelf)
        {
            mission1CompleteTM.text = "Misson 1: Conquer Island 3 COMPLETE!";
            island3Win.SetActive(true);
        }
        if (mission2 == null || !mission2.activeSelf)
        {
            mission2CompleteTM.text = "Misson 2: Conquer Island 2 COMPLETE!";
            island2Win.SetActive(true);

        }
        if ((mission1 == null || !mission1.activeSelf) && (mission2 == null || !mission2.activeSelf))
        {
            s1CountTM.text = "<All castles destroyed>";
            s1CountSelectedTM.text = "<All castles destroyed>";
            allMissionsComplete.SetActive(true);
            finalNote.SetActive(true);
        }
    }

    void CheckControl()
    {
        Vector3 touchPadVec = _controller.Touch1PosAndForce;
        float force = touchPadVec.z;
        if (force > 0.5)
        {
            if (! _alreadyIncremented)
            {
                _alreadyIncremented = true;
                //_i = (_i + 1) % 3;
                if (startNote.activeSelf)
                {
                    startNote.SetActive(false);
                    presshere.SetActive(false);
                }
                if (finalNote.activeSelf)
                {
                    finalNote.SetActive(false);
                }
            }
            _forceButton = true;
        }
        else 
        {
            _alreadyIncremented = false;
            _forceButton = false;
        }
    }

    //DateTime pressTime;

    //void OnButtonDown(byte controller_id, MLInputControllerButton button)
    //{
    //    if ((button == MLInputControllerButton.Bumper))
    //    {
    //        _bumper = true;
    //        pressTime = DateTime.Now;
    //    }


    //}

}
