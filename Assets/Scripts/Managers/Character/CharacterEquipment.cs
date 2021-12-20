using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterEquipment : MonoBehaviour
{
    private Character character;

    private Tilemap tilemap;

    public EquipmentType equipmentType;
    
    private void OnValidate()
    {
        /*if(character == null)
            character = GetComponentInParent<Character>();
        if(tilemap == null)
            tilemap = GetComponent<Tilemap>();

        if(_item != null)
        {
            tilemap.SetTile(Vector3Int.zero, _item.Tile);
        }*/
    }

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponentInParent<Character>();
        tilemap = GetComponent<Tilemap>();
        
        if(_item != null)
        {
            tilemap.SetTile(Vector3Int.zero, _item.Tile);
        }
    }

    [SerializeField] private Item _item;
    public Item item
    {
        get{return _item;}
        set
        {
            _item = value;
            if(value == null)
                tilemap.SetTile(Vector3Int.zero, null);
            else
                tilemap.SetTile(Vector3Int.zero, value.Tile);
        }
    }
}
