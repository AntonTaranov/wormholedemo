using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TruckPart : MonoBehaviour {

    public Sprite[] sprites;

    private Image imageHolder;

	void Start () {
        imageHolder = GetComponent<Image>();
	}
	
    public void SetWeight(int value){
        if (imageHolder != null){
            int frameValue = Mathf.Min(value, sprites.Length - 1);
            if (frameValue >= 0)
                imageHolder.sprite = sprites[frameValue];
        }    
    }

}
