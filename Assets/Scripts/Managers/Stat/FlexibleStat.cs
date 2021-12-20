using UnityEngine;
using System;

[Serializable]
public class FlexibleStat : StatWithDepency
{
    [SerializeField] protected float _currentStat;
    public float currentStat{
        get{return _currentStat;}
        set{
            // if new value is in between 0 and calculated value
            if(value <= calculatedValue && value > 0)
            {
                _currentStat = value;
            }
            // for example trying to overheal
            else if(value > calculatedValue)
            {
                _currentStat = calculatedValue;
            }
            // for example dead
            else if(_currentStat <= 0)
            {
                _currentStat = 0;
            }
        }
    }
}
