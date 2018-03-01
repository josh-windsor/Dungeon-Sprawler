using System.Collections;
using System.Collections.Generic;
using MORPH3D;
using UnityEngine;

public class UpgradeController : MonoBehaviour {

    private static ContentPack _helmet;
    public static GameObject Sword { get; private set; }
    private static List<ContentPack> _armour;
    private static GameObject _shield;
    private static List<ContentPack> _legs;
    private static M3DCharacterManager _characterManager;

    public static Vector3 SwordHoldPos;
    public static Quaternion SwordHoldRot;



    public static void LoadAllUpgradesMm()
    {
        _characterManager = MainMenuPlayerScript.CharacterManager;
        LoadAllUpgrades();
    }

    public static void LoadAllUpgradesMg()
    {
        _characterManager = PlayerController.CharacterManager;
        _helmet = null;
        Sword = null;
        _armour = null;
        _shield = null;
        _legs = null;
        LoadAllUpgrades();
    }

    private static void LoadAllUpgrades()
    {
        LoadHelmet(GameController.PlayerStats.UpgradesPurchased[0]);
        LoadWeapon(GameController.PlayerStats.UpgradesPurchased[1]);
        LoadArmour(GameController.PlayerStats.UpgradesPurchased[2]);
        LoadShield(GameController.PlayerStats.UpgradesPurchased[3]);
        LoadLegs(GameController.PlayerStats.UpgradesPurchased[4]);
    }

    public static void LoadUpgrade(int upgradeSection)
    {
        switch (upgradeSection)
        {
            case 0:
                LoadHelmet(GameController.PlayerStats.UpgradesPurchased[upgradeSection]);
                break;
            case 1:
                LoadWeapon(GameController.PlayerStats.UpgradesPurchased[upgradeSection]);
                break;
            case 2:
                LoadArmour(GameController.PlayerStats.UpgradesPurchased[upgradeSection]);
                break;
            case 3:
                LoadShield(GameController.PlayerStats.UpgradesPurchased[upgradeSection]);
                break;
            case 4:
                LoadLegs(GameController.PlayerStats.UpgradesPurchased[upgradeSection]);
                break;
        }

    }


    private static void LoadHelmet(int weaponLevel)
    {
        if (_helmet != null)
        {
            _characterManager.RemoveContentPack(_helmet);

        }
        _helmet = new ContentPack();

        switch (weaponLevel)
        {
            case 0:
                _helmet.setupWithGameObject(Resources.Load("Armour/Helmets/0Hair/HelmetMod") as GameObject);
                _characterManager.AddContentPack(_helmet);
                break;
            case 1:
                _helmet.setupWithGameObject(Resources.Load("Armour/Helmets/1Rusted/HelmetMod") as GameObject);
                _helmet.RootGameObject.GetComponentInChildren<Renderer>().material = Resources.Load("Materials/Rust") as Material;
                _characterManager.AddContentPack(_helmet);
                break;
            case 2:
                _helmet.setupWithGameObject(Resources.Load("Armour/Helmets/1Rusted/HelmetMod") as GameObject);
                _helmet.RootGameObject.GetComponentInChildren<Renderer>().material = Resources.Load("Armour/Helmets/2Iron/1Perceval") as Material;
                _characterManager.AddContentPack(_helmet);
                break;
        }

    }

    private static void LoadWeapon(int weaponLevel)
    {
        Destroy(Sword);
        switch (weaponLevel)
        {
            case 0:
                Sword = Instantiate(Resources.Load("Weapons/Swords/0Plank") as GameObject, GameObject.Find("chestUpperAttachmentPoint").transform);
                break;
            case 1:
                Sword = Instantiate(Resources.Load("Weapons/Swords/1Rusted/SwordMod") as GameObject, GameObject.Find("chestUpperAttachmentPoint").transform);
                Sword.GetComponentInChildren<Renderer>().material = Resources.Load("Materials/Rust") as Material;
                break;
            case 2:
                Sword = Instantiate(Resources.Load("Weapons/Swords/1Rusted/SwordMod") as GameObject, GameObject.Find("chestUpperAttachmentPoint").transform);
                break;
        }
        SwordHoldPos = Sword.transform.localPosition;
        SwordHoldRot = Sword.transform.localRotation;

    }

    private static void LoadArmour(int weaponLevel)
    {
        if (_armour != null)
        {
            foreach (ContentPack armourPiece in _armour)
            {
                _characterManager.RemoveContentPack(armourPiece);
            }
        }
        _armour = new List<ContentPack>();
        ContentPack cp;
        switch (weaponLevel)
        {
            case 0:
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Body/0Tunic/Belt/BodyMod") as GameObject);
                _armour.Add(cp);
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Body/0Tunic/Tunic/BodyMod") as GameObject);
                _armour.Add(cp);
                break;
            case 1:
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Body/1OverTunic/BodyMod") as GameObject);
                _armour.Add(cp);
                goto case 0;
            case 2:
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Body/2Bracers/BodyModL") as GameObject);
                _armour.Add(cp);
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Body/2Bracers/BodyModR") as GameObject);
                _armour.Add(cp);
                goto case 1;
            case 3:
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Body/3BetterTunic/BodyMod") as GameObject);
                _armour.Add(cp);
                break;
            case 4:
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Body/4Chain/Gloves/BodyModR") as GameObject);
                _armour.Add(cp);
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Body/4Chain/Gloves/BodyModL") as GameObject);
                _armour.Add(cp);
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Body/4Chain/Shirt/BodyMod") as GameObject);
                _armour.Add(cp);
                goto case 3;
            case 5:
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Body/5Leather/BodyMod") as GameObject);
                _armour.Add(cp);
                goto case 4;
            case 6:
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Body/6Tabard/BodyMod") as GameObject);
                _armour.Add(cp);
                goto case 5;
        }

        foreach (ContentPack armourPiece in _armour)
        {
            _characterManager.AddContentPack(armourPiece);
        }

    }

    private static void LoadShield(int weaponLevel)
    {
        Destroy(_shield);
        switch (weaponLevel)
        {
            case 0:
                _shield = Instantiate(Resources.Load("Weapons/Shields/0Wood") as GameObject, GameObject.Find("lHandAttachmentPoint").transform);
                break;
            case 1:
                _shield = Instantiate(Resources.Load("Weapons/Shields/1Rusted/ShieldMod") as GameObject, GameObject.Find("lHandAttachmentPoint").transform);
                _shield.GetComponentInChildren<Renderer>().material = Resources.Load("Materials/Rust") as Material;
                break;
            case 2:
                _shield = Instantiate(Resources.Load("Weapons/Shields/1Rusted/ShieldMod") as GameObject, GameObject.Find("lHandAttachmentPoint").transform);
                _shield.GetComponentInChildren<Renderer>().material = Resources.Load("Materials/Iron") as Material;
                break;
            case 3:
                _shield = Instantiate(Resources.Load("Weapons/Shields/1Rusted/ShieldMod") as GameObject, GameObject.Find("lHandAttachmentPoint").transform);
                break;
        }
    }

    private static void LoadLegs(int weaponLevel)
    {
        if (_legs != null)
        {
            foreach (ContentPack legsPiece in _legs)
            {
                _characterManager.RemoveContentPack(legsPiece);
            }
        }
        _legs = new List<ContentPack>();
        ContentPack cp;
        switch (weaponLevel)
        {
            case 0:
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Legs/0Tunic/Pants/LegsMod") as GameObject);
                _legs.Add(cp);
                if (weaponLevel != 0)
                {
                    break;
                }
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Legs/0Tunic/Boots/LegsModR") as GameObject);
                _legs.Add(cp);
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Legs/0Tunic/Boots/LegsModL") as GameObject);
                _legs.Add(cp);
                break;
            case 1:
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Legs/1Leather/LegsModL") as GameObject);
                _legs.Add(cp);
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Legs/1Leather/LegsModR") as GameObject);
                _legs.Add(cp);
                if (weaponLevel != 1)
                {
                    break;
                }
                goto case 0;
            case 2:
                cp = new ContentPack();
                cp.setupWithGameObject(Resources.Load("Armour/Legs/2Chain/LegsMod") as GameObject);
                _legs.Add(cp);
                goto case 1;
        }

        foreach (ContentPack legsPiece in _legs)
        {
            _characterManager.AddContentPack(legsPiece);
        }
    }

}
