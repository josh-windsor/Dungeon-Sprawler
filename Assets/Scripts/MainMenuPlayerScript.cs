using System.Collections.Generic;
using MORPH3D;
using UnityEngine;

public class MainMenuPlayerScript : MonoBehaviour
{

    private static int _currentUpgrade = -1;
    private static bool _switchToggle;
    public static M3DCharacterManager CharacterManager;

    private void Start()
    {
        CharacterManager = GetComponent<M3DCharacterManager>();



    }




    public void WeaponSwitch()
    {
        if (_currentUpgrade != -1)
        {
            switch (_currentUpgrade)
            {
                case 1:
                    UpgradeController.LoadUpgrade(1);
                    break;
            }
            _currentUpgrade = -1;
        }
        else
        {
            GameObject sword = UpgradeController.Sword;
            if (_switchToggle)
            {
                sword.transform.SetParent(GameObject.Find("chestUpperAttachmentPoint").transform);
                sword.transform.localPosition = UpgradeController.SwordHoldPos;
                sword.transform.localRotation = UpgradeController.SwordHoldRot;

            }
            else
            {
                sword.transform.SetParent(GameObject.Find("rHandAttachmentPoint").transform);
            }
        }
        _switchToggle = !_switchToggle;
    }

    public static void PreviewUpgrade(int upgrade)
    {
        _currentUpgrade = upgrade;
    }
}
