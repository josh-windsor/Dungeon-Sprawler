using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DigitalRubyShared;
using UnityEngine.Advertisements;

public class MainMenuController : MonoBehaviour {

#if UNITY_IOS
    private string gameId = "1549541";
#elif UNITY_ANDROID
    private string gameId = "1549540";
#endif



    public FingersScript FingersScript;
    private PanGestureRecognizer _panGesture;
    private GameObject _mainCamera;
    private static Animator _playerAnimator;

    private static GameObject[] _purchaseButtons;
    private static GameObject _purchasePanel;
    private static GameObject _upgradePanel;
    private static int _skippedReward;
    private static int _watchedReward;

    private static TextMeshProUGUI _coinText1;
    private static TextMeshProUGUI _coinText2;


    // Use this for initialization
    private void Start ()
    {
        Advertisement.Initialize(gameId);
        RemoteSettings.Updated += RemoteSettingsUpdated;
        if (Application.isEditor)
        {
            RemoteSettings.ForceUpdate();
        }
        _playerAnimator = GameObject.Find("Player").GetComponent<Animator>();

        _mainCamera = Camera.main.gameObject;
        GameObject scrollerPanel = GameObject.Find("ScrollerPanel");

        _panGesture = new PanGestureRecognizer();
        _panGesture.Updated += PanCharacter;
        _panGesture.PlatformSpecificView = scrollerPanel;
        FingersScript.AddGesture(_panGesture);



        _coinText1 = GameObject.Find("CoinText").GetComponent<TextMeshProUGUI>();
        _coinText1.text = GameController.PlayerStats.Coins.ToString();
        _coinText2 = GameObject.Find("CoinText2").GetComponent<TextMeshProUGUI>();
        _coinText2.text = GameController.PlayerStats.Coins.ToString();

        GameObject buttonPanel = GameObject.Find("ButtonPanel");
	    GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
	    for (int i = 0; i < Levels.LevelsList.Count; i++)
	    {
	        GameObject goButton = (GameObject)Instantiate(Resources.Load("LevelButton"));
	        goButton.transform.SetParent(buttonPanel.transform, false);
	        goButton.transform.localScale = new Vector3(1, 1, 1);


	        int buttonNo = i + 1;
	        TextMeshProUGUI[] buttonTexts = goButton.GetComponentsInChildren<TextMeshProUGUI>();
	        buttonTexts[0].text = buttonNo.ToString();
	        if ((int) GameController.PlayerStats.LevelScores[buttonNo] != -1)
	        {
	            buttonTexts[1].text = GameController.PlayerStats.LevelScores[buttonNo].ToString("n2");
	        }
	        else
	        {
	            buttonTexts[1].text = "No Score Yet!";
	        }

            goButton.GetComponent<Button>().onClick.AddListener(() => gameController.StartLevel(buttonNo));
	    }

        //Upgrade panel init
	    _upgradePanel = GameObject.Find("UpgradePanel");
        _purchasePanel = GameObject.Find("PurchasePanel");
        _purchaseButtons = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            int i1 = i;
            _purchaseButtons[i] = _upgradePanel.transform.GetChild(i1 + 1).gameObject;
            _purchaseButtons[i].GetComponent<Button>().onClick.AddListener(() => FirstUpgradesMenu(i1));
        }
	    GameObject mainMenu = GameObject.Find("MainMenu");
        _upgradePanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => ResetPanel(mainMenu));
        Button[] buttons = _purchasePanel.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(PurchaseToggle);
        _purchasePanel.SetActive(false);
	    _upgradePanel.SetActive(false);

        //Hides Level Select screen
	    GameObject.Find("LevelSelect").SetActive(false);

        //Sets endless button function
        GameObject endlessButton = GameObject.Find("StartEndlessButton");
	    endlessButton.GetComponent<Button>().onClick.AddListener(() => gameController.StartLevel(0));

        //Sets endless score
	    if ((int)GameController.PlayerStats.LevelScores[0] != -1)
	    {
	        endlessButton.GetComponentInChildren<TextMeshProUGUI>().text = GameController.PlayerStats.LevelScores[0].ToString();
        }
	    else
	    {
	        endlessButton.GetComponentInChildren<TextMeshProUGUI>().text = 0.ToString();
	    }

        UpgradeController.LoadAllUpgradesMm();
    }

    private  void RemoteSettingsUpdated()
    {
        _skippedReward = RemoteSettings.GetInt("UpgradesAdSkippedCoins", 2); 
        _watchedReward = RemoteSettings.GetInt("UpgradesAdWatchedCoins", 15);
    }


    private static void ResetPanel(GameObject mainMenu)
    {
        if (_purchasePanel.activeInHierarchy)
        {
            PurchaseToggle();
        }
        _upgradePanel.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void FirstUpgradesMenu(int upgradeTapped)
    {
        UpgradesMenu(upgradeTapped);
        PurchaseToggle();
    }

    private void UpgradesMenu(int upgradeTapped)
    {
        _playerAnimator.SetBool("Sheathed", false);
        int upgradeLevel = GameController.PlayerStats.UpgradesPurchased[upgradeTapped];
        Button[] buttons = _purchasePanel.GetComponentsInChildren<Button>();
        string upgradeText;

        if (Upgrades.UpgradesList[upgradeTapped].Count > upgradeLevel)
        {
            Upgrades.UpgradeDetails upgradesDetails = Upgrades.UpgradesList[upgradeTapped][upgradeLevel];
            upgradeText = upgradesDetails.Text + "\n\n+" + upgradesDetails.SkillIncrease + " " + upgradesDetails.SkillName;


            buttons[1].onClick.RemoveAllListeners();
            buttons[1].onClick.AddListener(() => MakePurchase(upgradesDetails.Cost, upgradeTapped, upgradeLevel + 1));
            buttons[1].GetComponentInChildren<Text>().text = upgradesDetails.Cost.ToString();
            buttons[1].interactable = true;

        }
        else
        {
            upgradeText = "You have reached the highest upgrade level";

            buttons[1].interactable = false;
            buttons[1].GetComponentInChildren<Text>().text = "Max";
        }
        _purchasePanel.GetComponentInChildren<TextMeshProUGUI>().text = upgradeText;




    }

    private void MakePurchase(int cost, int upgradeTapped, int purchaseLevel)
    {
        if (GameController.PlayerStats.Coins > cost)
        {
            GameController.PlayerStats.Coins -= cost;

            _coinText1.text = GameController.PlayerStats.Coins.ToString();
            _coinText2.text = GameController.PlayerStats.Coins.ToString();

            GameController.PlayerStats.UpgradesPurchased[upgradeTapped] = purchaseLevel;
            UpgradesMenu(upgradeTapped);
            GameController.SaveUpgrades();

            if (upgradeTapped == 1)
            {
                MainMenuPlayerScript.PreviewUpgrade(upgradeTapped);
                StartCoroutine(DrawNewSword(upgradeTapped));
            }
            else
            {
                UpgradeController.LoadUpgrade(upgradeTapped);
            }

        }
        else
        {

            _purchasePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Not enough cash, tap here to watch a video & earn some coins!";
            Button[] buttons = _purchasePanel.GetComponentsInChildren<Button>();
            buttons[1].onClick.RemoveAllListeners();
            buttons[1].onClick.AddListener(ShowAd);
            buttons[1].GetComponentInChildren<Text>().text = "Watch";
        }

    }

    private static void ShowAd()
    {
        ShowOptions options = new ShowOptions
        {
            resultCallback = HandleShowResult
        };

        Advertisement.Show("rewardedVideo", options);

    }
    private static void HandleShowResult(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            
            _purchasePanel.GetComponentInChildren<TextMeshProUGUI>().text = "You have recieved " + _watchedReward + " coins!";
            GameController.PlayerStats.Coins += _watchedReward;

        }
        else if (result == ShowResult.Skipped)
        {

            _purchasePanel.GetComponentInChildren<TextMeshProUGUI>().text = "Video Skipped, watch till the end to earn more coins! (Recieved " + _skippedReward + " coins)";
            GameController.PlayerStats.Coins += _skippedReward;

        }
        else if (result == ShowResult.Failed)
        {
            _purchasePanel.GetComponentInChildren<TextMeshProUGUI>().text = "An Error Has Occured";
        }
        _coinText1.text = GameController.PlayerStats.Coins.ToString();
        _coinText2.text = GameController.PlayerStats.Coins.ToString();

        Button[] buttons = _purchasePanel.GetComponentsInChildren<Button>();
        buttons[1].GetComponentInChildren<Text>().text = "Watch Another";

    }


    private static IEnumerator DrawNewSword(int upgradeTapped)
    {
        _playerAnimator.SetBool("Sheathed", true);
        yield return new WaitForSeconds(1);
        _playerAnimator.SetBool("Sheathed", false);
    }

    private static void PurchaseToggle()
    {
        for (int i = 0; i < _purchaseButtons.Length; i++)
        {
            _purchaseButtons[i].SetActive(!_purchaseButtons[i].activeInHierarchy);
        }
        _purchasePanel.SetActive(!_purchasePanel.activeInHierarchy);
        if (!_purchasePanel.activeInHierarchy)
        {
            _playerAnimator.SetBool("Sheathed", true);
        }
    }

    private void PanCharacter(GestureRecognizer gesture, ICollection<GestureTouch> touches)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            float deltaX = _panGesture.DeltaX / 25.0f;
            _mainCamera.transform.LookAt(new Vector3(0,1.1f,0));
            _mainCamera.transform.RotateAround(new Vector3(0, 1.1f, 0), Vector3.up, deltaX * 5);
        }
    }


    //DEBUG ONNLY
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && Input.GetKey(KeyCode.LeftShift))
        {
            Debug.Log("Clear");
            PlayerPrefs.DeleteAll();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameController.PlayerStats.Coins += 10;
            _coinText1.text = GameController.PlayerStats.Coins.ToString();
            _coinText2.text = GameController.PlayerStats.Coins.ToString();


        }
    }


}
