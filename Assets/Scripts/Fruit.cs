using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fruit : GameObjectWithWeight {

    public Text Label;

    private float _speed = 250;
    private float _scale;
    private Truck _truck;
	
	// Update is called once per frame
	void Update () {
        Vector3 position = transform.localPosition;
        position.y -= _speed * Time.deltaTime;
        transform.localPosition = position;
        if (position.y < 40 * _scale && position.y > - 60 * _scale){
            Vector2 truckPosition = _truck.transform.localPosition;
            truckPosition.y -= Truck.RADIUS * _scale;
            float distance = Vector2.Distance(position, truckPosition);
            if (distance < 35 * _scale)
            {
                _truck.SetWeight(_truck.GetWeight() + _weight);
                Destroy(gameObject);
            }
        }else if (position.y < -260 * _scale){
            Destroy(gameObject);
        }
	}

	public override void SetWeight(int value)
	{
        base.SetWeight(value);
        Label.text = "" + _weight;
	}

    public void SetSpeed(float value){
        _speed = value;
    }

    public void SetScale(float value){
        _scale = value;
    }

    public void AddTruckReference(Truck truck){
        _truck = truck;
    }
}
