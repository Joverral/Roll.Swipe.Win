using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class LevelGenerator : MonoBehaviour {

    // TODO:  I could probably re-work this to get rid of the singleton
    public static LevelGenerator Current;			//A public static reference to itself (make's it visible to other objects without a reference)

    public Vector3 PlayerStartPosition { get; private set; }

    // ideally i'd make these read only
    public GameObject Player;

    // ideally i'd make these read only
    public GameObject Goal;

    [SerializeField]
    GameObject GoalSuctionVortex; // This only exists because of weird android bug where it seems to only use one of two colliders (EVEN IF ITS A TRIGGER?!) in a heirarchy


    [SerializeField]
    GameObject ConveyorPrefab;

    [SerializeField]
    GameObject CoinPrefab;

    [SerializeField]
    GameObject[] BackgroundElements;

    [SerializeField]
    GameObject[] ObstaclePrefabs;

    [SerializeField]
    GameObject[] TutorialElements;

    [SerializeField]
    GameObject SpawnBubblePrefab;

    [SerializeField]
    int minNumberPlatforms = 5;
    [SerializeField]
    int maxNumberPlatforms = 20;

    [SerializeField]
    int minHorizontalScale = 3;
    [SerializeField]
    int maxHorizontalScale = 25;

    [SerializeField]
    float minSpaceBetweenPlatform = 5;
    [SerializeField]
    float maxSpaceBetweenPlatform = 40;

    [SerializeField]
    float minConveyorHeight = -10;
    [SerializeField]
    float maxConveyorHeight = 100;

    [SerializeField]
    float minConveyorRotZ = -30;
    [SerializeField]
    float maxConveyorRotZ = 100;

    [SerializeField]
    int minNumberObstacles = 0;
    [SerializeField]
    int maxNumberObstacles = 3;

    [SerializeField]
    int spawnAllOneObstacleTypeCadence = 5;

    public float LevelLength { get; private set;  }

    public float MaxLevelHeight { get { return maxConveyorHeight; } }
    public float MinLevelHeight { get { return minConveyorHeight; } }

    void Awake()
    {
        Debug.Log("Level Generator AWAKEN");
        //Ensure that there is only one object pool
        if(Current == this)
        {
            return;
        }
        if (Current == null)
        {
            Current = this;
        }
        else
        {
            Debug.LogError("Attempting to spawn a second LevelGenerator!", this);
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        //Restart();
    }

    public void AddAllPrefabsToPooler()
    {
        for(int i = 0; i < ObstaclePrefabs.Length; ++i)
        {
            GameObjectPooler.Current.AddPrefab(ObstaclePrefabs[i]);
        }

        GameObjectPooler.Current.AddPrefab(ConveyorPrefab);

        GameObjectPooler.Current.AddPrefab(CoinPrefab, 30);
    }

    public void GenerateLevel(int level)
    {
        SetTutorialElementVisibility(level == 1);

        LevelLength = 0.0f;

        GameObjectPooler.Current.PoolAllObjects();

        int newNumberOfConveyors = Random.Range(minNumberPlatforms, maxNumberPlatforms);

        for(int i = 0; i < newNumberOfConveyors; ++i)
        {
            var newConveyor = GameObjectPooler.Current.GetObject(ConveyorPrefab, false);
            newConveyor.transform.localScale = new Vector3(Random.Range(minHorizontalScale, maxHorizontalScale), 1.0f, 1.0f);
            newConveyor.transform.position = new Vector3(LevelLength, Random.Range(minConveyorHeight, maxConveyorHeight), 0.0f);
            newConveyor.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(minConveyorRotZ, maxConveyorRotZ));
            newConveyor.SetActive(true);

            var newCoin = GameObjectPooler.Current.GetObject(CoinPrefab);
            newCoin.transform.position = newConveyor.transform.position + Vector3.up * 3.0f;

            LevelLength += newConveyor.transform.localScale.x + Random.Range(minSpaceBetweenPlatform, maxSpaceBetweenPlatform);
        }

        var numObstacles = level + Random.Range(minNumberObstacles, maxNumberObstacles);

        int obstacleType = Random.Range(0, ObstaclePrefabs.Length);
        bool spawnMultipleObstacleTypes = level % spawnAllOneObstacleTypeCadence != 0;

        for (int i = 0; i < numObstacles; ++i )
        {
            obstacleType = spawnMultipleObstacleTypes ? Random.Range(0, ObstaclePrefabs.Length) : obstacleType;
            
            var bubble = GameObjectPooler.Current.GetObject(SpawnBubblePrefab).GetComponent<ProceduralSpawnBubble>();
            bubble.StartSpawn(ObstaclePrefabs[obstacleType]);
            bubble.transform.position = new Vector3(Random.Range(0.0f, LevelLength), Random.Range(minConveyorHeight, maxConveyorHeight), 0.0f);
        }

        int backgroudnBitField = Random.Range(1, BackgroundElements.Length);
        for(int i = 0; i < BackgroundElements.Length; ++i)
        {
            int shiftedBit = 1 << i;

            BackgroundElements[i].SetActive( (backgroudnBitField & shiftedBit) > 0);
        }

        // move the player to a set distance and set height.  (or just do nothing)
        PlayerStartPosition = Player.transform.position = new Vector3(0.0f, maxConveyorHeight + 10.0f);
        Player.GetComponent<Rigidbody2D>().angularVelocity = 0.0f;
        Player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // move the goal to a set distance and random height
        Goal.transform.position = new Vector3(LevelLength, Random.Range(minConveyorHeight, maxConveyorHeight) + 10.0f);
        GoalSuctionVortex.transform.position = Goal.transform.position;
    }

    public Vector2 GetRandomPoint()
    {
        return new Vector2(Random.Range(0.0f, LevelLength), Random.Range(minConveyorHeight, maxConveyorHeight));
    }

    private void SetActiveRecursively(GameObject targetObj, bool active)
    {
        targetObj.SetActive(active);
        for(int i = 0; i < targetObj.transform.childCount; ++i)
        {
            var child = targetObj.transform.GetChild(i);
            SetActiveRecursively(child.gameObject, active);
        }
    }

    private void SetTutorialElementVisibility(bool visible)
    {
        for(int i = 0; i < TutorialElements.Length; ++i)
        {
            TutorialElements[i].SetActive(visible);
        }
    }
}
