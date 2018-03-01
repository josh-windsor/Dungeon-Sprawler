using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{

    public struct EnemyType
    {
        public string Name { get; private set; }
        public float Hp { get; private set; }
        public float AttackDamage { get; private set; }
        public float AttackChargeTime { get; private set; }
        public float AttackSpeedMin { get; private set; }
        public float AttackSpeedMax { get; private set; }
        public int TimeReward { get; private set; }
        public int CoinReward { get; private set; }


        public EnemyType(string iName, float iHp, float iAttackDamage, float iAttackChargeTime, float iAttackSpeedMin, float iAttackSpeedMax, int iTimeReward, int iCoinReward) : this()
        {
            Name = iName;
            Hp = iHp;
            AttackDamage = iAttackDamage;
            AttackChargeTime = iAttackChargeTime;
            AttackSpeedMin = iAttackSpeedMin;
            AttackSpeedMax = iAttackSpeedMax;
            TimeReward = iTimeReward;
            CoinReward = iCoinReward;
        }
    }


    public static readonly List<EnemyType> EnemyTypes = new List<EnemyType>
    {
        new EnemyType("Easy Enemy", 10, 20, 3, 1, 1.5f, 3, 1),
        new EnemyType("Medium Enemy", 10, 30, 1, 0.2f, 2, 3, 4),
        new EnemyType("Hard Enemy", 20, 40, 1.5f, 0.2f, 0.5f, 6, 7)
    };

    public struct ChestType
    {
        public int Type { get; private set; }
        public float MinReward { get; private set; }
        public float MaxReward { get; private set; }
        public float ChanceReward { get; private set; }

        public ChestType(int iType, float iMinReward, float iMaxReward, float iChanceReward) : this()
        {
            Type = iType;
            MinReward = iMinReward;
            MaxReward = iMaxReward;
            ChanceReward = iChanceReward;
        }
    }

    public static readonly List<ChestType> ChestTypes = new List<ChestType>
    {
        new ChestType(0, 0, 4, 10),
        new ChestType(0, 0, 10, 2),
        new ChestType(7, 0, 3, 2)
    };


}