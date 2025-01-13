using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sprite Library")]
public class SpriteLibrary : ScriptableObject
{
    [SerializeField] Sprite Apple;
    [SerializeField] Sprite Bird;
    [SerializeField] Sprite Bow;
    [SerializeField] Sprite Button;
    [SerializeField] Sprite Cactus;
    [SerializeField] Sprite Can;
    [SerializeField] Sprite Candy;
    [SerializeField] Sprite Carrot;
    [SerializeField] Sprite Case;
    [SerializeField] Sprite Cheese;
    [SerializeField] Sprite Cup;
    [SerializeField] Sprite Donut;
    [SerializeField] Sprite Foot;
    [SerializeField] Sprite Hut;
    [SerializeField] Sprite Jelly;
    [SerializeField] Sprite Lemon;
    [SerializeField] Sprite MagnifyingGlass;
    [SerializeField] Sprite Notebook;
    [SerializeField] Sprite Pepper;
    [SerializeField] Sprite Picture;
    [SerializeField] Sprite Pie;
    [SerializeField] Sprite Pizza;
    [SerializeField] Sprite Pot;
    [SerializeField] Sprite Spoon;
    [SerializeField] Sprite Spray;
    [SerializeField] Sprite Strawberry;
    [SerializeField] Sprite Tomato;

    [SerializeField] Sprite OneBooster;
    [SerializeField] Sprite TwoBoosters;

    [SerializeField] Sprite Magnet;


    public Sprite GetSpriteByMatchingType(MatchingType type) => type switch
    {
        MatchingType.APPLE => Apple,
        MatchingType.BIRD => Bird,
        MatchingType.BOW => Bow,
        MatchingType.BUTTON => Button,
        MatchingType.CACTUS => Cactus,
        MatchingType.CAN => Can,
        MatchingType.CANDY => Candy,
        MatchingType.CARROT => Carrot,
        MatchingType.CASE => Case,
        MatchingType.CHEESE => Cheese,
        MatchingType.CUP => Cup,
        MatchingType.DONUT => Donut,
        MatchingType.FOOT => Foot,
        MatchingType.HUT => Hut,
        MatchingType.JELLY => Jelly,
        MatchingType.LEMON => Lemon,
        MatchingType.MAGNIFYING_GLASS => MagnifyingGlass,
        MatchingType.NOTEBOOK => Notebook,
        MatchingType.PEPPER => Pepper,
        MatchingType.PICTURE => Picture,
        MatchingType.PIE => Pie,
        MatchingType.PIZZA => Pizza,
        MatchingType.POT => Pot,
        MatchingType.SPOON => Spoon,
        MatchingType.SPRAY => Spray,
        MatchingType.STRAWBERRY => Strawberry,
        MatchingType.TOMATO => Tomato,
        _ => null
    };

    public Sprite GetSpriteByBoosterType(BoosterType type) => type switch
    {
        BoosterType.MAGNET => Magnet,
        _ => null
    };
    public Sprite GetSpriteForBoosterCounter(int counter) => counter switch
    {
        1 => OneBooster,
        2 => TwoBoosters,
        _ => null
    };
}
