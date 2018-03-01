using System.Collections;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    private int _itemType;
    private float _maxReward;
    private float _minReward;
    private float _chanceReward;
    private bool _opened;
    public void Instantiate(int iItemType, float iMaxReward, float iMinReward, float iChanceReward)
    {
        _itemType = iItemType;
        _maxReward = iMaxReward;
        _minReward = iMinReward;
        _chanceReward = iChanceReward;
    }

    public void Open()
    {
        if (!_opened)
        {
            _opened = true;
            StartCoroutine(OpenChest());
        }
    }

    private IEnumerator OpenChest()
    {
        Transform chestLid = transform.Find("ChestLid");
        float time = 0;
        while (time < 1.5)

        {
            time += Time.deltaTime;
            chestLid.Rotate(Vector3.down * Time.deltaTime * 11);
            yield return null;
        }


        if ((int)Random.Range(0, _chanceReward) != 0)
        {
            float rewardBonus = Random.Range(_minReward, _maxReward);
            if (_itemType < 5)
            {
                AddTimeBonus(rewardBonus);
            }
            else
            {
                AddCoinBonus(rewardBonus);
            }

        }
        UiController.ItemsFound++;
        UiController.SectionsFinished++;
        Destroy(gameObject);
    }

    private void AddTimeBonus(float reward)
    {
        Debug.Log("timesreward:" + reward);
        GameObject.Find("UI").GetComponent<UiController>().UpdateTimer(reward);
    }
    private void AddCoinBonus(float reward)
    {
        Debug.Log("coinsreward:" + (int)reward);
        GameObject.Find("UI").GetComponent<UiController>().UpdateCoins((int)reward);
    }
}
