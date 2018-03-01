using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour
{
    private GameObject _corridorSectionA;
    private GameObject _corridorSectionB;
    private bool _locationSwitch = false;
    private Vector3 _behindLocation;
    private Vector3 _currentLocation;
    private Vector3 _aheadLocation;

    private ItemSpawner _itemSpawner;
    private GameObject _playerGameObject;

    private bool _firstTime = true;
    private int _levelMode;
    private int _levelIterator;
    private int[] _levelList;

    public static int GameTime;

    // Use this for initialization
    private void Start()
    {

        _levelMode = GameController.LevelMode;
        if (_levelMode != 0)
        {
            GameTime = Levels.LevelsList[_levelMode - 1].GameTime;
            _levelList = Levels.LevelsList[_levelMode - 1].LevelList;
            _levelList = Shuffle(_levelList);
        }
        else
        {
            GameTime = 200;
        }
        _itemSpawner = GameObject.Find("ItemSpawner").GetComponent<ItemSpawner>();
        _playerGameObject = GameObject.Find("Player");

        _corridorSectionA = GameObject.Find("CorridorSectionA");
        _corridorSectionB = GameObject.Find("CorridorSectionB");
        _currentLocation = _corridorSectionA.transform.position;
        //16.84
        _aheadLocation = _corridorSectionB.transform.position;
        _behindLocation = new Vector3(0,0, -9.84f);

    }

    private static int[] Shuffle(int[] a)
    {
        for (int i = a.Length - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i);

            int temp = a[i];

            a[i] = a[rnd];
            a[rnd] = temp;
        }
        return a;
    }

    public void UpdateSpawn()
    {
        if (_firstTime)
        {
            _firstTime = false;
            GameObject.Find("UI").GetComponent<UiController>().StartGame();
        }
        UtilityVars.ScrollingInProgress = true;

        PlayerController.PlayerAnimator.SetTrigger("StartRun");

        if (_levelMode != 0)
        {
            if (_levelIterator < _levelList.Length)
            {
                _itemSpawner.SpawnItem(_levelList[_levelIterator]);
                _levelIterator++;
            }
            else
            {
                GameObject.Find("UI").GetComponent<UiController>().EndGame(0);
            }
        }
        else
        {
            _itemSpawner.SpawnItem(Random.Range(0, 13));
        }

        PanelMoverStarter();

    }

    public void PanelMoverStarter()
    {
        StartCoroutine(PanelMover());
    }


    private IEnumerator PanelMover()
    {
        Vector3 start1;
        Vector3 start2;
        Vector3 end1;
        Vector3 end2;
        float currentTime = 0f; // Time.time contains current frame time, so remember starting point
        if (_locationSwitch)
        {
            start1 = _aheadLocation;
            start2 = _currentLocation;
            end1 = _currentLocation;
            end2 = _behindLocation;
        }
        else
        {
            start1 = _currentLocation;
            start2 = _aheadLocation;
            end1 = _behindLocation;
            end2 = _currentLocation;
        }
        _locationSwitch = !_locationSwitch;
        while (currentTime < UtilityVars.ScrollSpeed)
        {
            _corridorSectionA.transform.position = Vector3.Lerp(start1, end1, currentTime / UtilityVars.ScrollSpeed);
            _corridorSectionB.transform.position = Vector3.Lerp(start2, end2, currentTime / UtilityVars.ScrollSpeed);
            // until one second passed
            currentTime += Time.deltaTime;

            yield return null; // wait for next frame
        }
        if (_playerGameObject != null)
        {
            PlayerController.PlayerAnimator.SetTrigger("StopRun");
        }
        UtilityVars.ScrollingInProgress = false;
    }
}
