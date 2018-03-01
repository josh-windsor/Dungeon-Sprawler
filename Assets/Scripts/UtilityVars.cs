using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityVars {
	public const float ScrollSpeed = 2;
	public static bool ScrollingInProgress = false;
	public static KeyValuePair<int, GameObject> CurrentItem;
    public static bool Paused = false;
    public static bool GameOver = false;
}
