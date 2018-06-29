using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Truck : GameObjectWithWeight
{

    public static readonly int RADIUS = 20;
    private static readonly float BOUNCE_DURATION = 0.2f;

    public TruckPart truckPartPrefab;

    public Text Label;

    private float _scale;
    private float _canvasWidth;

    private Vector2[] tail;
    private bool _bounce = false;
    private float _bounceTime;

    private TruckPart[] truckParts;

    void Start()
    {
        tail = new Vector2[300];
        float halfRadius = RADIUS * 0.5f;
        for (int i = 0; i < tail.Length; i++)
        {
            tail[i] = new Vector2(0, -halfRadius);
        }

        RectTransform canvas = gameObject.transform.parent as RectTransform;
        if (canvas != null)
        {
            _canvasWidth = canvas.rect.width;
            _scale = _canvasWidth / 320;
            gameObject.transform.localScale = new Vector3(_scale, _scale, 1);
        }
        SetWeight(10);
    }

    void Update()
    {
        if (_bounce)
        {
            _bounceTime += Time.deltaTime;
            Vector3 position = gameObject.transform.localPosition;
            if (_bounceTime >= BOUNCE_DURATION)
            {
                _bounce = false;
                position.y = 0;
            }
            else
            {
                float halfTime = BOUNCE_DURATION * 0.5f;
                if (_bounceTime <= halfTime)
                {
                    position.y = Mathf.Lerp(0, -45 * _scale, Mathf.Pow(_bounceTime / halfTime, 2));
                }
                else
                {
                    float reverseTime = BOUNCE_DURATION - _bounceTime;
                    position.y = Mathf.Lerp(0, -45 * _scale, Mathf.Pow(reverseTime / halfTime, 2));
                }
            }
            gameObject.transform.localPosition = position;
        }
        redrawTail();
    }

    private void redrawTail()
    {
        //set length by weight
        int length = 1 + (GetWeight() / 6);

        int fullWeight = GetWeight();
        if (fullWeight < 0) return;

        if (truckParts == null){
            truckParts = new TruckPart[length];
            for (int i = 0; i < truckParts.Length; i++){
                TruckPart part = (TruckPart)Instantiate(truckPartPrefab, transform);
                truckParts[i] = part;
            }
        }else if (truckParts.Length > length){
            TruckPart[] tmpParts = new TruckPart[length];
            for (int i = 0; i < truckParts.Length; i++)
            {
                if (i < length)
                {
                    tmpParts[i] = truckParts[i];
                }
                else
                {
                    Destroy(truckParts[i].gameObject);
                }
            }
            truckParts = tmpParts;
        }else if (truckParts.Length < length){
            TruckPart[] tmpParts = new TruckPart[length];
            for (int i = 0; i < tmpParts.Length; i++)
            {
                if (i < truckParts.Length)
                {
                    tmpParts[i] = truckParts[i];
                }
                else
                {
                    TruckPart part = (TruckPart)Instantiate(truckPartPrefab, transform);
                    tmpParts[i] = part;
                }
            }
            truckParts = tmpParts;
        }

        float sqrRadius = RADIUS * RADIUS * 4;
        int trailPointerIndex = 0;
        float trailLength = 0;
        float truckStartX = 0;
        float truckStartY = -RADIUS;
        float truckEndX = truckStartX;
        float truckEndY = truckStartY;

        for (int i = 0; i < length; i++)
        {
            TruckPart part = truckParts[i];

            if (fullWeight > 6)
            {
                part.SetWeight(6);
                fullWeight = fullWeight - 6;
            }
            else
            {
                part.SetWeight(fullWeight);
                fullWeight = 0;
            }
            // calculate position and rotation for every truck
            while (trailLength < sqrRadius)
            {
                if (trailPointerIndex >= tail.Length) return;
                truckEndX += tail[trailPointerIndex].x;
                truckEndY += tail[trailPointerIndex].y;
                trailLength = (truckEndX - truckStartX) * (truckEndX - truckStartX)
                        + (truckEndY - truckStartY) * (truckEndY - truckStartY);
                trailPointerIndex++;

            }
            float scaleTrail = (float)Mathf.Sqrt(trailLength) / (2 * RADIUS);
            float deltaX = (truckEndX - truckStartX) / scaleTrail;
            float deltaY = (truckEndY - truckStartY) / scaleTrail;
            float truckPositionX = truckStartX + 0.5f * deltaX;
            float truckPositionY = truckStartY + 0.5f * deltaY;

            Vector3 position = part.transform.localPosition;
            position.x = truckPositionX;
            position.y = truckPositionY;
            part.transform.localPosition = position;

            Vector3 eulerAngles = part.transform.localEulerAngles;
            eulerAngles.z = Mathf.Atan2(deltaY, deltaX) * 180 / Mathf.PI + 90;
            part.transform.localEulerAngles = eulerAngles;

            trailLength = trailLength - sqrRadius;
            truckStartX = truckStartX + deltaX;
            truckStartY = truckStartY + deltaY;
        }
    }

    public void WarpProgress(float value)
    {
        Vector3 position = gameObject.transform.localPosition;
        float scale = _scale;
        if(value > 0){
            position.y = 600 * value * _scale;
            scale = Mathf.Lerp(_scale, _scale * 5, value);
        }else{
            position.y = 0;
        }
        gameObject.transform.localScale = new Vector3(scale, scale, 1);
        gameObject.transform.localPosition = position;
    }

    public override void SetWeight(int value)
    {
        base.SetWeight(value);
        Label.text = "" + _weight;
    }

    public void AddDeltaY(float deltaY)
    {
        AddPosition(0, deltaY / _scale);
    }

    private void AddPosition(float deltaX, float deltaY)
    {
        for (int i = tail.Length - 1; i > 0; i--)
        {
            tail[i].x = tail[i - 1].x;
            tail[i].y = tail[i - 1].y;
        }
        tail[0].x = deltaX;
        tail[0].y = deltaY;
    }

    public void MoveBy(float deltaX)
    {
        Vector3 position = gameObject.transform.localPosition;
        float halfWidth = _canvasWidth * 0.5f;
        float circleRadius = RADIUS * _scale;
        if (position.x - circleRadius + deltaX < -halfWidth) deltaX = -halfWidth + circleRadius - position.x;
        else if (position.x + circleRadius + deltaX > halfWidth) deltaX = halfWidth - circleRadius - position.x;
        position.x += deltaX;
        gameObject.transform.localPosition = position;

        AddPosition(-deltaX / _scale, 0);
    }

    public bool Hit()
    {
        if (_weight <= 0) return false;
        SetWeight(_weight - 1);
        _bounce = true;
        _bounceTime = 0;
        return true;
    }

    public bool IsBounce()
    {
        return _bounce;
    }
}
