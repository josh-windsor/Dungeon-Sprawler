using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    private readonly Vector3 _startLocation = new Vector3(0f, 2.5f, 13f);
    private readonly Vector3 _endLocation = new Vector3(0f, 2.5f, 3f);

    private readonly List<Items.EnemyType> _enemyTypes = Items.EnemyTypes;
    private readonly List<Items.ChestType> _chestTypes = Items.ChestTypes;

    public void SpawnItem(int itemType)
    {
        var itemSwitch = new Dictionary<Func<int, bool>, Action>
        {
            {x => x < 10, () => SpawnEnemy(itemType)},
            {x => x < 20, () => SpawnChest(itemType)}
        };

        itemSwitch.First(sw => sw.Key(itemType)).Value();
    }

    private void SpawnEnemy(int itemType)
    {
        string enemyName;
        switch (itemType)
        {
            case 0:
                enemyName = "Cube";
                break;

            case 1:
                enemyName = "Sphere";
                break;

            case 2:
                enemyName = "Capsule";
                break;

            default:
                SpawnEnemy(Random.Range(0, 3));
                return;
        }

        GameObject enemy = Instantiate(Resources.Load(enemyName), _startLocation, new Quaternion()) as GameObject;

        Items.EnemyType type = _enemyTypes[itemType];
        enemy.AddComponent<EnemyController>().Instantiate(type.Name, type.Hp, type.AttackDamage, type.AttackChargeTime, type.AttackSpeedMin, type.AttackSpeedMax, type.TimeReward, type.CoinReward);


        enemy.transform.position = _startLocation;

        MoveObject(enemy, 0);
        UtilityVars.CurrentItem = new KeyValuePair<int, GameObject>(itemType, enemy);
    }

    private void SpawnChest(int itemType)
    {
        Items.ChestType type;
        try
        {
            type = _chestTypes[itemType - 10];
        }
        catch (Exception e)
        {
            Debug.Log(e);
            SpawnChest(Random.Range(10, 13));
            return;
        }
        GameObject chest = GameObject.Instantiate(Resources.Load("Chest")) as GameObject;
        chest.AddComponent<ChestController>().Instantiate(type.Type, type.MinReward, type.MaxReward, type.ChanceReward);
        chest.transform.position = _startLocation;
        MoveObject(chest, 10);
        UtilityVars.CurrentItem = new KeyValuePair<int, GameObject>(itemType, chest);
    }

    private void SpawnDecision()
    {

    }

    public void MoveObject(GameObject itemToMove, int itemType)
    {
        StartCoroutine(ItemMover(itemToMove, itemType));
    }

    private IEnumerator ItemMover(GameObject item, int itemType)
    {
        Vector3 startLocationLocal = _startLocation;
        Vector3 endLocationLocal = _endLocation;

        switch (itemType)
        {
            case -2:
                startLocationLocal = new Vector3(0, -2f, -2f);
                endLocationLocal = new Vector3(0, 1f, -2f);
                break;

            case -1:
                startLocationLocal += new Vector3(0, -0.25f, 0);
                endLocationLocal += new Vector3(0, -0.25f, 0);
                break;

            case 0:
                //Not needed for now
                startLocationLocal += new Vector3(0, 0, 0);
                endLocationLocal += new Vector3(0, 0, 0);
                break;

            case 10:
                startLocationLocal += new Vector3(0, -2, 0);
                endLocationLocal += new Vector3(0, -2, 0);
                break;
        }

        float currentTime = 0f; // Time.time contains current frame time, so remember starting point
        while (currentTime < UtilityVars.ScrollSpeed)
        { // until one second passed
            currentTime += Time.deltaTime;
            item.transform.position = Vector3.Lerp(startLocationLocal, endLocationLocal, currentTime / UtilityVars.ScrollSpeed);
            yield return null; // wait for next frame
        }
        switch (itemType)
        {
            case 0:
                item.GetComponent<EnemyController>().StartEnemy();
                break;
            case 10:
                break;
        }
    }
}
