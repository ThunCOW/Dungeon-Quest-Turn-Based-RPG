using UnityEngine;
using UnityEditor;
using System.Text;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;

public enum ItemType{
    Common,
    Rare,
    Legendary,
    Quest_Item,
}

public enum StatType{
    Health,
    Strength,
    Agility,
    Intelligence,
    Vitality,
}

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [SerializeField] string id;
    public string ID {get{return id;}}
    public string Name;
    public Sprite Icon;
    public Tile Tile;
    [Range(1,999)]
    public int MaximumStacks = 1;

    public ItemType itemType;
    public AudioClip Sound;

    protected static readonly StringBuilder sb = new StringBuilder();

    protected virtual void OnValidate()
    {
        #if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        id = AssetDatabase.AssetPathToGUID(path);
        #endif
    }

    public virtual Item GetCopy(){
        return this;
    }

    public virtual void Destroy(){

    }

    public virtual string GetItemType(){
        return "";
    }

    public virtual string GetDescription(){
        return "";
    }

    public virtual string CompareDescription(EquippableItem equippableItem = null){
        return "";
    }
}

//Skills
/*
    Base Value = 10 Dmg / 10 Heal / Increase STR 5 / 
        * Scalability? Does level increase it? Does stats like STR increase DMG? If only stats increase it
        * Dependant Stat (maybe)

    Cooldown = 1
        * How is it gonna scale? -1 every 2 point for a skill? maybe not gonna scale?
        * - 49.9% Dependant Class/Talent/Stat(?)

    Example
    Skill 1
    Swipe:
    5 physical damage - 0 magical damage
        * STR - 100%
        * + 120% Weapon Damage
    2 Bleed Damage for 2 sec



    Cooldown = 3
        * -49.9% Physical Talent

        

*/