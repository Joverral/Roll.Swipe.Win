using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwipeController : MonoBehaviour {

    [SerializeField]
    GameObject brickPrefab;
    GameObject ghostBrickPrefab; //created from brick

    List<GameObject> ghostBricks = new List<GameObject>();
    Queue<BrickDecay> instantiatedBricks = new Queue<BrickDecay>();
    [SerializeField]
    Material GhostMaterial;

    [SerializeField]
    Color ghostBrickColor;

    Vector3 lastPosition;

    [SerializeField]
    TransitionCanvas swipeTutorialElement;

    [SerializeField]
    TutorialManager tutorialManager;

    // Use this for initialization
    void Start () {

        // note, we don't actually use the gameobject pooler to instantiate the prefab
        ghostBrickPrefab = GameObject.Instantiate(brickPrefab) as GameObject;
        ghostBrickPrefab.name = "WooooooGhostBrick";
        // make it a ghost
        var meshRenderer = ghostBrickPrefab.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            meshRenderer.material = GhostMaterial;
        }
        else
        {
            ghostBrickPrefab.GetComponentInChildren<SpriteRenderer>().color = ghostBrickColor;
            ghostBrickPrefab.GetComponent<BrickDecay>().enabled = false;
            //var spriteRenderer = ghostBrickPrefab.GetComponentInChildren<SpriteRenderer>().enabled = false;
            //spriteRenderer.color = Color.black;
               //new Color(Color.gray.r, Color.gray.g, Color.gray.b, 0.4f);
            //spriteRenderer.material = GhostMaterial;
        }

        ghostBrickPrefab.GetComponent<Collider2D>().enabled = false;
        // default to inactive
        ghostBrickPrefab.SetActive(false);

        GameObjectPooler.Current.AddPrefab(ghostBrickPrefab, 64);
	}
	
	// Update is called once per frame

    void UpdateMouse()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0.0f;

        if (Input.GetMouseButtonDown(0))
        {
            lastPosition = mouseWorld;
        }
        else if (Input.GetMouseButton(0))
        {
            var dir = lastPosition - mouseWorld;
            var length = dir.magnitude;

            if (length > 2.0f)
            {
                var cappedLength = Mathf.Min(length, 20.0f);
                var newGhostBrick = GameObjectPooler.Current.GetObject(ghostBrickPrefab);

                newGhostBrick.transform.position = lastPosition - (dir / length) * cappedLength * 0.5f;
                newGhostBrick.transform.right = dir / cappedLength;
                newGhostBrick.transform.localScale = new Vector3(cappedLength, 0.3f, 1.0f);
                ghostBricks.Add(newGhostBrick);
                lastPosition = mouseWorld;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SpawnRealBricksFromGhosts();
        }
    }

    void UpdateTouch()
    {
        if(Input.touchCount == 0)
        {
            return;
        }

        Vector3 touchWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        touchWorld.z = 0.0f;

        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            lastPosition = touchWorld;
        }
        else if (Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            var dir = lastPosition - touchWorld;
            var length = dir.magnitude;

            if (length > 2.0f)
            {
                var cappedLength = Mathf.Min(length, 20.0f);
                var newGhostBrick = GameObjectPooler.Current.GetObject(ghostBrickPrefab);

                newGhostBrick.transform.position = lastPosition - (dir / length) * cappedLength * 0.5f;
                newGhostBrick.transform.right = dir / cappedLength;
                newGhostBrick.transform.localScale = new Vector3(cappedLength, 0.3f, 1.0f);
                ghostBricks.Add(newGhostBrick);
                lastPosition = touchWorld;
            }
        }
        else if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            SpawnRealBricksFromGhosts();
        }
        else if(Input.GetTouch(0).phase == TouchPhase.Canceled)
        {
            foreach (GameObject ghostBrick in ghostBricks)
            {
                GameObjectPooler.Current.PoolObject(ghostBrick);
            }
            ghostBricks.Clear();
        }
    }

    void SpawnRealBricksFromGhosts()
    {
        if (ghostBricks.Count > 0)
        {
            if(GameObjectPooler.Current.GetRemainingPoolSize(brickPrefab) <= ghostBricks.Count)
            {
                DecayBricks(ghostBricks.Count); // this is kind of wishy washy, because ghost bricks come from the same pool
            }

            if (swipeTutorialElement && tutorialManager.IsTop(swipeTutorialElement))
            {
                tutorialManager.PopCurrentMenu();
            }

            foreach (GameObject ghostBrick in ghostBricks)
            {
                var newBrick = GameObjectPooler.Current.GetObject(brickPrefab);
                newBrick.transform.position = ghostBrick.transform.position;
                newBrick.transform.localScale = ghostBrick.transform.localScale;
                newBrick.transform.rotation = ghostBrick.transform.rotation;
                instantiatedBricks.Enqueue(newBrick.GetComponent<BrickDecay>());

                //if(newBrick.transform.position.x > LevelGenerator.Current.Goal.transform.position.x)
                //{
                //    var conveyor = newBrick.GetComponent<ConveyorBelt>();
                //    conveyor.Direction *= -1.0f;
                //}

                GameObjectPooler.Current.PoolObject(ghostBrick);
            }
            ghostBricks.Clear();
        }
    }

    //    if(Input.GetTouch(0).phase == TouchPhase.Began
	void Update () {

        if (Time.timeScale == 1.0f)
        {
            UpdateTouch();
            UpdateMouse();
        }
	}

    public void Clear()
    {
        ghostBricks.Clear();
        instantiatedBricks.Clear();
    }

    private void DecayBricks(int count)
    {
        // if we have too many bricks
        int stepFunction = Mathf.Max(1, count / 48); // random number slightly  less than the default pool size
        for (int i = 0; i < count; ++i)
        {
            var brick = instantiatedBricks.Dequeue();

            if (i % stepFunction == 0)
            {
                brick.Decay(
                    (Vector2)brick.transform.position + Random.insideUnitCircle *
                    Random.Range(1.0f, brick.transform.localScale.x), Random.insideUnitCircle * Random.Range(1.0f, 100.0f),
                    float.MaxValue);
            }
            else
            {
                // try to conserve and just pool
                GameObjectPooler.Current.PoolObject(brick.gameObject);
            }
        }
    }

    public void DecayAll()
    {
        DecayBricks(instantiatedBricks.Count);
    }
}
 