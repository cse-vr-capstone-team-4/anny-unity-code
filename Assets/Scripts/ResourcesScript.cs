//using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.MagicLeap;
using UnityEngine;
//using TMPro;

public class ResourcesScript : MonoBehaviour
{
    private MLInputController _controller;
    private bool _trigger = false;

    //public static List<GameObject> resList;
    public GameObject[] resArray;
    public GameObject res1;
    public GameObject res2;
    public GameObject res3;
    public GameObject res4;
    public GameObject res5;
    public GameObject res6;
    public GameObject res7;
    public GameObject res8;
    public GameObject res9;
    public GameObject res10;
    //public static List<GameObject> soldierList;
    public GameObject[] soldierArray;
    public GameObject s1;
    public GameObject s2;
    public GameObject s3;
    public GameObject s4;
    public GameObject s5;
    public GameObject s6;
    public GameObject s7;
    public GameObject s8;
    public GameObject s9;
    public GameObject s10;
    public GameObject soldier;

    public GameObject[] currentResources;

    private float secondsBetweenSpawn = 5.0f;
    private float elapsedTime = 0.0f;
    private float elapsedTimeSoldier = 0.0f;
    private float soldierDisappearTime = 3.0f;

    void Awake()
    {
        MLInput.Start();

        foreach (GameObject res in resArray)
        {
            res.SetActive(false);
        }
        foreach (GameObject s in soldierArray)
        {
            s.SetActive(false);
        }

        resArray = new GameObject[10];
        resArray[0] = res1;
        resArray[1] = res2;
        resArray[2] = res3;
        resArray[3] = res4;
        resArray[4] = res5;
        resArray[5] = res6;
        resArray[6] = res7;
        resArray[7] = res8;
        resArray[8] = res9;
        resArray[9] = res10;
        soldierArray = new GameObject[10];
        soldierArray[0] = s1;
        soldierArray[1] = s2;
        soldierArray[2] = s3;
        soldierArray[3] = s4;
        soldierArray[4] = s5;
        soldierArray[5] = s6;
        soldierArray[6] = s7;
        soldierArray[7] = s8;
        soldierArray[8] = s9;
        soldierArray[9] = s10;

        currentResources = new GameObject[10];
        soldier.SetActive(false);
        //ResourceSpawn();
        _controller = MLInput.GetController(MLInput.Hand.Left);
    }

    void OnDestroy()
    {
        MLInput.Stop();
    }

    void Update()
    {
        CheckControl();

        elapsedTime += Time.deltaTime;
        elapsedTimeSoldier += Time.deltaTime;

        if (elapsedTime > secondsBetweenSpawn)
        {
            //resource.transform.localScale = new Vector3(3, 3, 3);
            ResourceSpawn();
        }

        if (elapsedTimeSoldier > soldierDisappearTime)
        {
            soldier.SetActive(false);
        }  
    }

    // spawn resources
    void ResourceSpawn()
    {
        int idx = UnityEngine.Random.Range(0, 10);
        Debug.Log(idx);
        //GameObject resToSpawn = resList[0];
        //foreach (GameObject g in resList) 
        //{
        //    if (resList.IndexOf(g) == idx)
        //    {
        //        resToSpawn = g;
        //    }
        //}
        //GameObject gName = GameObject.Find("Resource" + idx);
        //resToSpawn = gName;

        GameObject resToSpawn = resArray[idx];
        //while (currentResources.Contains(resToSpawn)){
        //    resToSpawn = resList[UnityEngine.Random.Range(0, 11)];
        //}
        if (!resToSpawn.activeSelf)
        {
            currentResources[idx] = resToSpawn;
            resToSpawn.SetActive(true);
        }

    }

    // collect resources / check trigger
    void CheckControl()
    {
        //if (_controller.TriggerValue > 0.2f  && MagicLeap.VirtualPointer.crate)
        //{
        //    GameObject selectedRes = MagicLeap.VirtualPointer.crateObject;
        //    string resName = selectedRes.name;

        //    if (selectedRes.activeSelf && !_trigger && currentResources.(selectedRes))
        //    {
        //        selectedRes.SetActive(false);
        //        currentResources.Remove(selectedRes);
        //        int idx = currentResources.IndexOf(selectedRes);
        //        soldier = soldierArray[idx];
        //        soldier.transform.localPosition = selectedRes.transform.localPosition;
        //        soldier.SetActive(true);

        //        CombatController.canBuildSoldier += 2;

        //        elapsedTimeSoldier = 0;
        //        elapsedTime = 0;
        //        _trigger = true;
        //    }
        //}
        //else
        //{
        //    _trigger = false;
        //}
    }
}