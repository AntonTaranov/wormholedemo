using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksManager : MonoBehaviour
{

    public Block[] blocks;

    private int _startY;
    private float _speed = 250;
    private float _scale;
    private bool _stopped = false;
    private bool _positiveHit;

    public void GenerateNextWall()
    {
        foreach (var block in blocks)
        {
            block.GenerateNew();
        }
        gameObject.transform.localPosition = new Vector3(0, _startY, 0);
        _stopped = false;
    }

    public bool BlockHit(float positionY)
    {
        positionY /= _scale;
        positionY += 160;
        int blockIndex = (int)(positionY / 65);
        if (blocks[blockIndex].GetWeight() > 0)
        {
            blocks[blockIndex].SetWeight(blocks[blockIndex].GetWeight() - 1);
            _positiveHit = blocks[blockIndex].IsPositive();
            return true;
        }
        else
            return false;
    }

    void Start()
    {
        RectTransform canvas = gameObject.transform.parent as RectTransform;
        if (canvas != null)
        {
            _scale = canvas.rect.width / 320;
            gameObject.transform.localScale = new Vector3(_scale, _scale, 1);

            _startY = (int)canvas.rect.height / 2;

            GenerateNextWall();
        }
    }

    void Update()
    {
        if (_stopped) return;

        Vector3 position = gameObject.transform.localPosition;
        position.y -= Time.deltaTime * _speed;
        if (position.y < 0)
        {
            position.y = 0;
            _stopped = true;
        }
        gameObject.transform.localPosition = position;
    }

    public void SetSpeed(float value)
    {
        _speed = value;
    }

    public bool IsStopped()
    {
        return _stopped;
    }

    public bool IsPositiveHit()
    {
        return _positiveHit;
    }
}
