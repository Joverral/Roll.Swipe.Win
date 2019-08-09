using UnityEngine;
using System.Collections;

public class ProceduralSpawnBubble : MonoBehaviour {

    [SerializeField]
    float TimeUntilSpawn = 0.5f;

    float timer = 0.0f;
    GameObject ObjectToSpawnPrefab;

    public void StartSpawn(GameObject objectToSpawnPrefab)
    {
        ObjectToSpawnPrefab = objectToSpawnPrefab;
        timer = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;
        if(timer > TimeUntilSpawn)
        {
            var newObstacle = GameObjectPooler.Current.GetObject(ObjectToSpawnPrefab, false);
            newObstacle.transform.position = this.transform.position;
            SetActiveRecursively(newObstacle, true);
            GameObjectPooler.Current.PoolObject(this.gameObject);
        }
	}

    // TODO stick this in a generic helper somewhere
    private void SetActiveRecursively(GameObject targetObj, bool active)
    {
        targetObj.SetActive(active);
        for (int i = 0; i < targetObj.transform.childCount; ++i)
        {
            var child = targetObj.transform.GetChild(i);
            SetActiveRecursively(child.gameObject, active);
        }
    }
}
