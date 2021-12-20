using UnityEngine;

public abstract class UsableItemEffectOld : ScriptableObject
{
    public abstract void ExecuteEffect(UsableItem parentItem, Character character);

    public abstract string GetDescription();
}
