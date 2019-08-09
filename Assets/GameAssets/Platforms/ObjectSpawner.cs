using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{

    [SerializeField]
    GameObject ObjectToSpawnPrefab;

    [SerializeField]
    float MinTimeBetweenSpawns = 3.0f;

    [SerializeField]
    float MaxTimeBetweenSpawns = 5.0f;

    Coroutine spawnCoroutine;

    [SerializeField]
    bool AddVelocityOnSpawn = false;

    [SerializeField]
    Vector2 minVelocity;

    [SerializeField]
    Vector2 maxVelocity;

    [SerializeField]
    BoxCollider2D spawnZone;

    [SerializeField]
    bool RandomizeScale  = false;

    [SerializeField]
    float minScale;

    [SerializeField]
    float maxScale;


    IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MinTimeBetweenSpawns, MaxTimeBetweenSpawns));
            var spawnObj = GameObjectPooler.Current.GetObject(ObjectToSpawnPrefab);

            if (spawnZone != null)
            {
                spawnObj.transform.position =  new Vector3(
                                                    spawnZone.transform.position.x + 
                                                        Random.Range(-spawnZone.size.x / 2.0f, spawnZone.size.x / 2.0f),
                                                    spawnZone.transform.position.y + 
                                                        Random.Range(-spawnZone.size.y / 2.0f, spawnZone.size.y / 2.0f),
                                                    0);
            }
            else
            {
                spawnObj.transform.position = this.transform.position;
            }
            
            if (RandomizeScale)
            {
                float newScale = Random.Range(minScale, maxScale);
                spawnObj.transform.localScale = new Vector3(newScale, newScale, newScale);

                var trail = spawnObj.GetComponent<TrailRenderer>();
                if (trail != null)
                {
                    var circleCollider = spawnObj.GetComponent<CircleCollider2D>(); //terrible hackery

                    trail.startWidth = newScale * circleCollider.radius * 5;
                }
            }

            if (AddVelocityOnSpawn)
            {
                var body = spawnObj.GetComponent<Rigidbody2D>();
                Vector2 forceToAdd = Vector2.Lerp(minVelocity, maxVelocity, Random.value);
                body.AddForce(forceToAdd * body.mass, ForceMode2D.Impulse);
            }

            
            
            // TODO play noise 
        }
    }

    void OnEnable()
    {
        if(spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }
}