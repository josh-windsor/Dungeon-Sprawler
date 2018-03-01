using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UiController : MonoBehaviour
{
    private struct EnemyUi
    {
        public GameObject EnemyPanel;
        public Slider EnemyHealthBar;
        public TextMeshProUGUI EnemyName;
    }

    private static GameObject _finishPanel;
    private static GameObject _deathPanel;
    private static GameObject _endButtons;
    private static GameObject _pauseMenu;
    private readonly List<TextMeshPro> _statsValues = new List<TextMeshPro>();
    private static GameObject _pause;
    private static GameObject _play;



    private EnemyUi _enemyUi;

    private TextMeshProUGUI _timerText;
    private TextMeshProUGUI _timerEventText;
    private TextMeshProUGUI _coinText;
    private static float _gameTime;

    private bool _finished;

    public static int EnemiesDefeated;
    public static int ItemsFound;
    public static int SectionsFinished;



    private void Start()
    {
        EnemiesDefeated = 0;
        ItemsFound = 0;
        SectionsFinished = 0;

        _enemyUi.EnemyPanel = GameObject.Find("EnemyInfo");
        _enemyUi.EnemyHealthBar = GameObject.Find("EnemyHealth").GetComponent<Slider>();
        _enemyUi.EnemyName = GameObject.Find("EnemyText").GetComponent<TextMeshProUGUI>();
        _enemyUi.EnemyPanel.SetActive(false);
        _timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        _timerEventText = GameObject.Find("TimerEventText").GetComponent<TextMeshProUGUI>();
        _coinText = GameObject.Find("CoinText").GetComponent<TextMeshProUGUI>();
        _finishPanel = GameObject.Find("FinishMenu");
        _deathPanel = GameObject.Find("DeathMenu");
        int i = 0;
        foreach (Transform childTransform in _finishPanel.transform)
        {
            _statsValues.Add(childTransform.gameObject.GetComponent<TextMeshPro>());
            i++;
            if (i == 4)
            {
                break;
            }
        }
        i = 0;
        foreach (Transform childTransform in _deathPanel.transform)
        {
            _statsValues.Add(childTransform.gameObject.GetComponent<TextMeshPro>());
            i++;
            if (i == 5)
            {
                break;
            }
        }
        _pauseMenu = GameObject.Find("PauseMenu");
        GameObject.Find("QuitButton").GetComponent<Button>().onClick.AddListener(EndGame);
        GameObject.Find("RestartButton").GetComponent<Button>().onClick.AddListener(RestartGame);
        _pauseMenu.SetActive(false);
        _pause = GameObject.Find("Pause");
        _play = GameObject.Find("Play");

        _pause.GetComponent<Button>().onClick.AddListener(() => PauseGame(true));
        _play.GetComponent<Button>().onClick.AddListener(() => PauseGame(false));
        _play.SetActive(false);


        _endButtons = GameObject.Find("EndButtonPanel");
        GameObject.Find("EndQuitButton").GetComponent<Button>().onClick
            .AddListener(EndGame);
        GameObject.Find("EndRestartButton").GetComponent<Button>().onClick.AddListener(RestartGame);
        _endButtons.SetActive(false);
        _gameTime = LevelController.GameTime;
        SetTimer((int) _gameTime);
        UpdateCoins(0);
    }

    public static void PauseGame(bool paused)
    {
        Time.timeScale = Convert.ToInt32(!paused);
        _pauseMenu.SetActive(paused);
        UtilityVars.Paused = paused;
        _play.SetActive(paused);
        _pause.SetActive(!paused);

    }


    private static void RestartGame()
    {
        Time.timeScale = 1;
        UtilityVars.Paused = false;
        UtilityVars.ScrollingInProgress = false;
        UtilityVars.GameOver = false;
        if (GameController.LevelMode != 0)
        {
            GameController.SaveScore(_gameTime);
        }
        else
        {
            GameController.SaveScore(SectionsFinished);
        }
        SceneManager.LoadScene("MainGame");
    }

    public void StartGame()
    {
        StartCoroutine(RunTimer());
    }

    private static void EndGame()
    {
        Time.timeScale = 1;
        UtilityVars.Paused = false;
        UtilityVars.ScrollingInProgress = false;
        UtilityVars.GameOver = false;

        if (GameController.LevelMode != 0)
        {
            GameController.EndLevel(_gameTime);
        }
        else
        {
            GameController.EndLevel(SectionsFinished);
        }
    }


    public void ShowEnemyInfo(string iName)
    {
        _enemyUi.EnemyPanel.SetActive(true);
        _enemyUi.EnemyHealthBar.value = 1f;
        _enemyUi.EnemyName.text = iName;
    }

    public void UpdateHealthBar(float iNewHp)
    {
        if (iNewHp > 0)
        {
            _enemyUi.EnemyHealthBar.value = iNewHp;
        }
        else
        {
            _enemyUi.EnemyPanel.SetActive(false);
        }
    }

    public void TakeDamage(float damage)
    {
        _gameTime -= damage;
        if (_gameTime <= 0)
        {
            StopAllCoroutines();
            _gameTime = 0;
            EndGame(2);
        }
        else
        {
            PlayerController.PlayerAnimator.SetTrigger("TakeDamage");
        }
        StartCoroutine(ShowTimerEvent("-" + damage));
    }

    private void SetTimer(int iTimerStart)
    {
        _timerText.text = iTimerStart.ToString();
    }

    public void UpdateTimer(float time)
    {
        _gameTime += time;
        if (time > 0)
        {
            StartCoroutine(ShowTimerEvent("+" + time.ToString("n0")));
        }
        else
        {
            StartCoroutine(ShowTimerEvent(time.ToString("n0")));
        }
    }
    public void UpdateCoins(int reward)
    {
        GameController.PlayerStats.Coins += reward;
        _coinText.text = GameController.PlayerStats.Coins.ToString();
    }

    private IEnumerator RunTimer()
    {
        while (_gameTime > 0 && !_finished)
        {
            _gameTime -= Time.deltaTime;

            _timerText.text = _gameTime.ToString("n2");
            yield return null;
        }
        if (_gameTime <= 0 )
        {
            _gameTime = 0;
            EndGame(1);
            _timerText.text = _gameTime.ToString("n2");
        }
    }

    private IEnumerator ShowTimerEvent(string eventText)
    {
        _timerEventText.text = eventText;
        yield return new WaitForSeconds(1.5f);
        _timerEventText.text = "";
    }

    public void EndGame(int endMode)
    {
        _finished = true;
        UtilityVars.GameOver = true;
        _endButtons.SetActive(true);
        PlayerController.Dead = true;

        if (UtilityVars.CurrentItem.Value != null)
        {
            Destroy(UtilityVars.CurrentItem.Value);
        }
        string[] outputStrings =
        {
            _gameTime.ToString("n2"),
            EnemiesDefeated.ToString(),
            ItemsFound.ToString(),
            SectionsFinished.ToString()
        };

        int i = 0;
        int j = 0;
        switch (endMode)
        {
            case 0:
                GameObject.Find("ItemSpawner").GetComponent<ItemSpawner>().MoveObject(_finishPanel, -1);
                break;
            case 1:
            case 2:
                j = 4;
                i++;
                GameObject.Find("ItemSpawner").GetComponent<ItemSpawner>().MoveObject(_deathPanel, -2);
                _statsValues[7].text = "Here Lies You";
                if (endMode == 2)
                {
                    _statsValues[8].text = "Killed by " + _enemyUi.EnemyName.text;
                }
                else
                {
                    _statsValues[8].text = "Killed by the Gods";
                }
                break;
        }
        while (i < 4)
        {
            _statsValues[j].text = outputStrings[i];
            j++;
            i++;
        }
    }
}
