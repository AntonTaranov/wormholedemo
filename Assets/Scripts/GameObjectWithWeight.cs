using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectWithWeight : MonoBehaviour {

    protected int _weight;

    public int GetWeight(){
        return _weight;
    }

    public virtual void SetWeight(int value){
        _weight = value;
    }
}
