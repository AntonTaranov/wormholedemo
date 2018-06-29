using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour, IDragHandler
{
    public static readonly int MAX_WEIGHT = 10;
    private static readonly float WARP_DURATION = 0.5f;

    public BlocksManager blocks;
    public Truck truck;
    public Fruit fruitPrefab;
    public Text scoreLabel;
    public Text destinationLabel;

    private float _speed;
    private float _scale;

    private float warpTime;
    private int score = 0;
    private int nextFruitScore = 0;
    private int dest = 2000;
    private bool warp = false;

    public void OnDrag(PointerEventData eventData)
    {
        if (truck != null && !warp)
        {
            truck.MoveBy(eventData.delta.x);
        }
    }

    void Start()
    {
        RectTransform canvas = gameObject.transform as RectTransform;
        if (canvas != null)
        {
            _scale = canvas.rect.width / 320;
            _speed = canvas.rect.width * 3 / 8;
            blocks.SetSpeed(_speed);
            createRandomFruit();
        }
        score = 0;
        dest = 2000;
        updateStatistics();
    }

    private void updateStatistics(){
        scoreLabel.text = "Score: " + score;
        destinationLabel.text = "Dest: " + dest;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        float stepDistance = _speed * deltaTime;
        truck.AddDeltaY(-stepDistance * (warp ? 3 : 1));

        if (warp){
            warpTime += deltaTime;
            if (warpTime >= WARP_DURATION){
                warp = false;
                truck.WarpProgress(0);
                blocks.GenerateNextWall();
                createRandomFruit();
                if (nextFruitScore > 15)
                {
                    createRandomFruit();
                    nextFruitScore = 0;
                }
            }else{
                truck.WarpProgress(warpTime / WARP_DURATION);
            }
            return;
        }
        if (blocks.IsStopped()){
            if(truck.IsBounce()){
                //wait for bounce finish
            }else{
                if (onWallCollision()){
                    if (truck.Hit())
                    {
                        score++;
                        dest += blocks.IsPositiveHit() ? 1 : -1;
                        updateStatistics();
                    }
                    else{ //game over
                        SceneManager.LoadScene(0);
                    }
                }else{
                    warp = true;
                    warpTime = 0;
                }
            }
        }
    }

    private void createRandomFruit(){
        int fruitWeight = Random.Range(1, 6);
        Fruit fruit = Instantiate<Fruit>(fruitPrefab, transform);
        fruit.SetWeight(fruitWeight);
        fruit.SetSpeed(_speed);
        float fruitX = 30 + (320 - 60) * Random.Range(0, 1f) - 160;
        float fruitY = 240 - 60 - (480 * 0.25f) * Random.Range(0, 1f);
        nextFruitScore += fruitWeight;
        Vector3 position = fruit.transform.localPosition;
        position.x = fruitX * _scale;
        position.y = fruitY * _scale;
        fruit.transform.localPosition = position;
        fruit.transform.localScale = new Vector3(_scale, _scale, 1);
        fruit.SetScale(_scale);
        fruit.AddTruckReference(truck);
    }

    private bool onWallCollision(){
        Vector3 truckPosition = truck.transform.localPosition;
        return blocks.BlockHit(truckPosition.x);
    }
}
