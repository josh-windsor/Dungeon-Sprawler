using System.Collections.Generic;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    public static readonly List<List<UpgradeDetails>> UpgradesList = new List<List<UpgradeDetails>>
    {
        //Helmet
        new List<UpgradeDetails>
        {
            new UpgradeDetails
            {
                Cost = 40,
                Text = "Rusty Helm",
                SkillName = "Defence",
                SkillIncrease = 1
            },
            new UpgradeDetails
            {
                Cost = 60,
                Text = "Iron Helm",
                SkillName = "Defence",
                SkillIncrease = 3
            }
        },
        //Weapon
        new List<UpgradeDetails>
        {
            new UpgradeDetails
            {
                Cost = 20,
                Text = "Rusty Sword",
                SkillName = "Attack",
                SkillIncrease = 2
            },
            new UpgradeDetails
            {
                Cost = 100,
                Text = "Iron Sword",
                SkillName = "Attack",
                SkillIncrease = 5
            }
        },
        //Chest
        new List<UpgradeDetails>
        {
            new UpgradeDetails
            {
                Cost = 20,
                Text = "OverTunic",
                SkillName = "Defence",
                SkillIncrease = 2
            },
            new UpgradeDetails
            {
                Cost = 20,
                Text = "Leather Bracers",
                SkillName = "Defence",
                SkillIncrease = 1
            },
            new UpgradeDetails
            {
                Cost = 40,
                Text = "Reinforced Tunic",
                SkillName = "Defence",
                SkillIncrease = 2
            },
            new UpgradeDetails
            {
                Cost = 60,
                Text = "Chainmail Vest",
                SkillName = "Defence",
                SkillIncrease = 4
            },
            new UpgradeDetails
            {
                Cost = 60,
                Text = "Leather Overcoat",
                SkillName = "Defence",
                SkillIncrease = 5
            },
            new UpgradeDetails
            {
                Cost = 80,
                Text = "Tabard",
                SkillName = "Coin Increase",
                SkillIncrease = 2
            }



        },
        //Shield
        new List<UpgradeDetails>
        {
            new UpgradeDetails
            {
                Cost = 40,
                Text = "Rusty Shield",
                SkillName = "Parry",
                SkillIncrease = 2
            },
            new UpgradeDetails
            {
                Cost = 60,
                Text = "Iron Shield",
                SkillName = "Parry",
                SkillIncrease = 2
            },
            new UpgradeDetails
            {
                Cost = 40,
                Text = "Escutcheon",
                SkillName = "Coin Increase",
                SkillIncrease = 2
            }

        },
        //Legs
        new List<UpgradeDetails>
        {
            new UpgradeDetails
            {
                Cost = 70,
                Text = "Leather Boots",
                SkillName = "Speed",
                SkillIncrease = 3
            },
            new UpgradeDetails
            {
                Cost = 50,
                Text = "Chainmail Greaves",
                SkillName = "Defence",
                SkillIncrease = 3
            },

        }
    };

    public struct UpgradeDetails
    {
        public string Text;
        public int Cost;
        public string SkillName;
        public int SkillIncrease;
    }


}
