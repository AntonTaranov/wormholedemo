using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : GameObjectWithWeight
{

    public Image Square;
    public Image Tail;
    public Text Label;

    private bool _positive;

    static readonly Color basePositiveColor = new Color(0.9f, 0.9f, 0.0f);
    static readonly Color baseNegativeColor = new Color(0.2f, 0.6f, 1);
    static readonly Color maxPositiveColor = new Color(0.6f, 0.6f, 0.2f);
    static readonly Color maxNegativeColor = new Color(0.2f, 0.6f, 0.6f);

    static readonly Color baseFullAlpha = new Color(1, 1, 1, 1);
    static readonly Color baseHalfAlpha = new Color(1, 1, 1, 0.5f);

    public void GenerateNew()
    {
        _positive = Random.Range(0, 2) > 0;
        SetWeight(Random.Range(1, 11));

        Vector3 rotation = Tail.transform.localEulerAngles;
        rotation.z = _positive ? 180 : 0;
        Tail.transform.localEulerAngles = rotation;

        Label.gameObject.SetActive(true);
        Square.gameObject.SetActive(true);
        Tail.color = baseHalfAlpha;
    }

    public override void SetWeight(int value)
    {
        base.SetWeight(value);
        if (_weight <= 0)
        {
            Label.gameObject.SetActive(false);
            Square.gameObject.SetActive(false);
            Tail.color = baseFullAlpha;
            return;
        }
        Label.text = "" + _weight;
        if (_positive)
            Square.color = getComponentForWeight(_weight, basePositiveColor, maxPositiveColor);
        else
            Square.color = getComponentForWeight(_weight, baseNegativeColor, maxNegativeColor);
    }

    public static Color getComponentForWeight(int weight, Color baseComponent, Color maxComponent)
    {
        return Color.Lerp(baseComponent, maxComponent, ((float)weight / GameController.MAX_WEIGHT));
    }

    public bool IsPositive()
    {
        return _positive;
    }
}
