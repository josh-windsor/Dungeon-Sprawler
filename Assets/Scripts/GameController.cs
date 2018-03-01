using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController _thisLevelController;

    public struct PlayerStats
    {
        public static int TimeBonus;
        public static float Damage;
        public static float Armour;
        public static float ParryStun;
        public static int[] UpgradesPurchased = new int[5];
        public static List<float> LevelScores = new List<float>();
        public static int Coins;

    }

    public static int LevelMode { private set; get; }

    private void Start ()
    {
	    if (_thisLevelController != null)
	    {
	        Destroy(this);
	    }
	    else
	    {
	        _thisLevelController = this;
	    }
        DontDestroyOnLoad(this);

        LoadGame();
    }


    public void StartLevel(int level)
    {

        LevelMode = level;
        SceneManager.LoadScene("MainGame");
    }

    public static void EndLevel(float newScore)
    {
        SaveScore(newScore);
        SceneManager.LoadScene("MainMenu");
    }

    private static void LoadGame()
    {
        PlayerStats.Coins = PlayerPrefs.GetInt("PlayerCoins", 3);
        for (int i = 0; i < Levels.LevelsList.Count + 1; i++)
        {
            string levelSave = "LevelScore" + i;
            PlayerStats.LevelScores.Add(PlayerPrefs.GetFloat(levelSave, -1));
        }
        for (int i = 0; i < 5; i++)
        {
            PlayerStats.UpgradesPurchased[i] = PlayerPrefs.GetInt("Upgrades" + i, 0);
        }
    }

    public static void SaveScore(float newScore)
    {
        if (PlayerStats.LevelScores[LevelMode] < newScore)
        {
            PlayerStats.LevelScores[LevelMode] = newScore;
            string levelSave = "LevelScore" + LevelMode;
            PlayerPrefs.SetFloat(levelSave, newScore);
        }
        PlayerPrefs.SetInt("PlayerCoins", PlayerStats.Coins);
        PlayerPrefs.Save();
    }

    public static void SaveUpgrades()
    {
        PlayerPrefs.SetInt("PlayerCoins", PlayerStats.Coins);
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetInt("Upgrades" + i, PlayerStats.UpgradesPurchased[i]);
        }
        PlayerPrefs.Save();

    }
}
