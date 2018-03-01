using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour {
    public struct LevelsInfo
    {
        public int GameTime { get; private set; }
        public int[] LevelList { get; private set; }


        public LevelsInfo(int iGameTime, int[] iLevelList) : this()
        {
            GameTime = iGameTime;
            LevelList = iLevelList;
        }
    }
    public static readonly List<LevelsInfo> LevelsList = new List<LevelsInfo>
    {
        new LevelsInfo(20, new []{13,13}),
        new LevelsInfo(200, new[]{0,1,10, 1, 0, 10, 1, 2}),
        new LevelsInfo(40, new[]{1,1,1})
    };
}
