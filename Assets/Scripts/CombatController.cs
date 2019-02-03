using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.MagicLeap;
using UnityEngine;


public class CombatController : MonoBehaviour
{
    private MLInputController _controller;
    //private bool _trigger = false;
    public static int soldierCount = 0;
    public static int enemyCount = 0;
    private GameObject Island;
    public static int canBuildSoldier = 0;
    public static int remainCastle = 2;

    public GameObject playerObject1;
    public GameObject playerObject2;
    public GameObject playerObject3;
    private GameObject[] soldierList;
    public GameObject enemyObject;
    public static int gold;

    // For enemy spawn
    public float secondsBetweenSpawn = 5.0f;
    private float elapsedTime = 0.0f;
    private bool _alreadyIncremented = false;

    void Awake()
    {
        MLInput.Start();
        MLInput.OnControllerButtonUp += OnButtonUp;
        _controller = MLInput.GetController(MLInput.Hand.Left);

        canBuildSoldier = 3; 
        soldierList = new GameObject[3];
        soldierList[0] = playerObject1;
        soldierList[1] = playerObject2;
        soldierList[2] = playerObject3;
        EnemySpawn(4);
    }

    void OnDestroy()
    {
        MLInput.OnControllerButtonUp -= OnButtonUp;
        MLInput.Stop();
    }

    void EnemySpawn(int n) {
        int islandNum = UnityEngine.Random.Range(2, 4);
        string islandString = "GameObject" + islandNum;
        GameObject enemyIsland = GameObject.Find(islandString);
        for (int i = 0; i < n; i++)
        {
            GameObject newEnemy = Instantiate(enemyObject) as GameObject;
            newEnemy.transform.parent = enemyIsland.transform;
            newEnemy.transform.localPosition = new Vector3(-1 * i, 0, 4);
            newEnemy.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            enemyCount += 1;
        }
    } 


    void Update()
    {
        Island = MagicLeap.VirtualPointer.island;
        CheckControl();

        // If all enemy dies, spawn new enemies into the battle field
        if (enemyCount == 0) {
            /**(Menu.mission1 != null || Menu.mission2 != null)) { **/
            elapsedTime += Time.deltaTime;
            if (elapsedTime > secondsBetweenSpawn) {
                elapsedTime = 0.0f;
                EnemySpawn(4);
            }
        } else {
            elapsedTime = 0.0f;
        }
    }

    void CheckControl()
    {
        // Transporation of the unit

        if (_controller.TriggerValue > 0.2f  && MagicLeap.VirtualPointer.knight != null)
        {
            if (!_alreadyIncremented)
            {
                CombatController.canBuildSoldier += 1;
                Destroy(MagicLeap.VirtualPointer.knight);
                _alreadyIncremented = true;
            }
        }
        else
        {
            _alreadyIncremented = false;
        }
    }

    // Home button to spawn soldier
    void OnButtonUp(byte controller_id, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.HomeTap && canBuildSoldier > 0 && Island != GameObject.Find("GameObject1")
            && Island != GameObject.Find("GameObject4") && Island != GameObject.Find("GameObject5"))
        {
            GameObject newSoldier = Instantiate(soldierList[soldierCount % 3]) as GameObject;

            soldierCount = soldierCount + 1;
            canBuildSoldier = canBuildSoldier - 1;

            // at some position of the island

            newSoldier.transform.parent = Island.transform;


            newSoldier.transform.localPosition = new Vector3(-((soldierCount / 5) % 3) - 1, 0, (soldierCount % 5) * (-1) + 2);

            newSoldier.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }



}
