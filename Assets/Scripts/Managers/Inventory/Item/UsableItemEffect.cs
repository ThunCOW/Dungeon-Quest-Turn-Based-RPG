using UnityEngine;

[System.Serializable]
public abstract class UsableItemEffect
{
    public abstract void ExecuteEffect(UsableItem parentItem, Character character);

    public abstract string GetDescription();

    /*public bool deleteMe;
    protected virtual bool _deleteMe{
        get
        {
            if(_deleteMe)
                Debug.Log("get works");

            return _deleteMe;
        }
        set
        {
            Debug.Log("works");
        }
    }*/
}