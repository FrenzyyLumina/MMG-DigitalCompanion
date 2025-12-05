using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemIconManager : MonoBehaviour
{
    public static ItemIconManager Instance { get; private set; }

    [Header("Mechanic Items")]
    [SerializeField] private Sprite diceIcon;
    [SerializeField] private Sprite mcGuffinIcon;

    [Header("Defensive Items")]
    [SerializeField] private Sprite bodyArmorIcon;
    [SerializeField] private Sprite personalRadarIcon;
    [SerializeField] private Sprite rationsIcon;
    [SerializeField] private Sprite adrenalineIcon;
    [SerializeField] private Sprite activeProjectileDomeIcon;
    [SerializeField] private Sprite revivalPillIcon;

    [Header("Offensive Items")]
    [SerializeField] private Sprite hushPuppyIcon;
    [SerializeField] private Sprite poisonBlowdartIcon;
    [SerializeField] private Sprite m9BayonetIcon;
    [SerializeField] private Sprite tripmineIcon;

    [Header("Utility Items")]
    [SerializeField] private Sprite universalMultiToolIcon;
    [SerializeField] private Sprite proximityDetectorIcon;
    [SerializeField] private Sprite truthSerumIcon;
    [SerializeField] private Sprite sneakingSuitIcon;
    [SerializeField] private Sprite cardboardBoxIcon;
    [SerializeField] private Sprite coinIcon;
    [SerializeField] private Sprite theDoohickeyIcon;

    [Header("Default")]
    [SerializeField] private Sprite defaultIcon;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Sprite GetItemIcon(GameEnums.Item itemType)
    {
        switch (itemType)
        {
            // Mechanic
            case GameEnums.Item.Dice:
                return diceIcon;
            case GameEnums.Item.McGuffin:
                return mcGuffinIcon;

            // Defensive
            case GameEnums.Item.Body_Armor:
                return bodyArmorIcon;
            case GameEnums.Item.Personal_Radar:
                return personalRadarIcon;
            case GameEnums.Item.Rations:
                return rationsIcon;
            case GameEnums.Item.Adrenaline:
                return adrenalineIcon;
            case GameEnums.Item.Active_Projectile_Dome:
                return activeProjectileDomeIcon;
            case GameEnums.Item.Revival_Pill:
                return revivalPillIcon;

            // Offensive
            case GameEnums.Item.Hush_Puppy:
                return hushPuppyIcon;
            case GameEnums.Item.Poison_Blowdart:
                return poisonBlowdartIcon;
            case GameEnums.Item.M9_Bayonet:
                return m9BayonetIcon;
            case GameEnums.Item.Tripmine:
                return tripmineIcon;

            // Utility
            case GameEnums.Item.Universal_Multi_Tool:
                return universalMultiToolIcon;
            case GameEnums.Item.Proximity_Detector:
                return proximityDetectorIcon;
            case GameEnums.Item.Truth_Serum:
                return truthSerumIcon;
            case GameEnums.Item.Sneaking_Suit:
                return sneakingSuitIcon;
            case GameEnums.Item.Cardboard_Box:
                return cardboardBoxIcon;
            case GameEnums.Item.Coin:
                return coinIcon;
            case GameEnums.Item.The_Doohickey:
                return theDoohickeyIcon;

            default:
                return defaultIcon;
        }
    }

    // Helper method to set an icon on an Image component
    public void SetIconOnImage(Image imageComponent, GameEnums.Item itemType)
    {
        if (imageComponent != null)
        {
            imageComponent.sprite = GetItemIcon(itemType);
        }
    }
}
