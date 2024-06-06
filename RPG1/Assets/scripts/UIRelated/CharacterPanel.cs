using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPanel : MonoBehaviour
{
    private static CharacterPanel instance;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private CharButton helmet, chest, shield, sword, feet,shoulders;

    public CharButton MySelectedButton { get; set; }

    public static CharacterPanel MyInstance
    {
        get
        {

            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<CharacterPanel>();
            }
            return instance;
        }
    }
    public void OpenClose()
    {
        if (canvasGroup.alpha <= 0)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1;
        }
        else
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;

        }
    }

    public void EquipArmor(Armor armor)
    {
        switch (armor.MyArmorType)
        {
            case ArmorType.Helmet:
                helmet.EquipArmor(armor);
                break;
            case ArmorType.Chest:
                chest.EquipArmor(armor);
                break;
            case ArmorType.Shield:
                shield.EquipArmor(armor);
                break;
            case ArmorType.Sword:
                sword.EquipArmor(armor);
                break;
            case ArmorType.Feet:
                feet.EquipArmor(armor);
                break;
            case ArmorType.Shoulders:
                shoulders.EquipArmor(armor);
                break;
            case ArmorType.MainHand:
                break;
            case ArmorType.OffHand:
                break;

                
        }

    }
}
