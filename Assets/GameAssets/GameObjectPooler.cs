//This is a heavy modification of the original code found here: http://forum.unity3d.com/threads/simple-reusable-object-pool-help-limit-your-instantiations.76851/

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameObjectPooler : MonoBehaviour
{
    // TODO:  I could probably re-work this to get rid of the singleton
    public static GameObjectPooler Current;			//A public static reference to itself (make's it visible to other objects without a reference)

    public GameObject[] prefabs;				//Collection of prefabs to be pooled

    HashSet<GameObject> prefabSet = new HashSet<GameObject>(); // internally we use this, makes it easier to programmatically add other prefabs to the pooler if we want

    Dictionary<int, Stack<GameObject>> pooledObjects;  // key = prefab instance id, value = list of pooled objects
    Dictionary<GameObject, GameObject> instancedPoolObjects; // Key = instance, Value = prefab

    public int[] amountToBuffer;				//The amount to pool of each object. This is optional
    public int defaultBufferAmount = 10;		//Default pooled amount if no amount abaove is supplied

    GameObject containerObject;					//A parent object for pooled objects to be nested under. Keeps the hierarchy clean

    void Awake()
    {
        //Ensure that there is only one object pool
        if (Current == null)
        {
            Current = this;
        }
        else
        {
            Debug.LogError("Attempting to spawn a second GameObjectPooler!");
            Destroy(gameObject);
        }

        //Create new container
        containerObject = new GameObject("ObjectPool");
        //Create new list for objects
        pooledObjects = new Dictionary<int, Stack<GameObject>>();
        instancedPoolObjects = new Dictionary<GameObject, GameObject>();

        int index = 0;
        
        foreach (GameObject objectPrefab in prefabs)
        {
            int bufferAmount;
            if (index < amountToBuffer.Length)
                bufferAmount = amountToBuffer[index];
            else
                bufferAmount = defaultBufferAmount;

            AddPrefab(objectPrefab, bufferAmount);

            // Go to the next prefab in the collection
            index++;
        }
    }

    public void AddPrefab(GameObject prefab)
    {
        AddPrefab(prefab, defaultBufferAmount);
    }
    public void AddPrefab(GameObject prefab, int startPoolSize)
    {
        if(!prefabSet.Add(prefab))
        {
            Debug.LogError("Trying to add prefab that already exists in the GameObjectPooler");
        }

        
        pooledObjects.Add(prefab.GetInstanceID(), new Stack<GameObject>());
        for (int i = 0; i < startPoolSize; i++)
        {
            var go = (GameObject)Instantiate(prefab);
            CleanPoolObject(go);
            pooledObjects[prefab.GetInstanceID()].Push(go);
        }
    }

    public GameObject GetObject(GameObject objectType, bool setActive = true)
    {
        int instanceID = objectType.GetInstanceID();
        Debug.Assert(pooledObjects.ContainsKey(instanceID),
                     "objecttype: " + objectType.ToString() + " not found in pool, did you add it to the GameObjectPooler?",
                     objectType);

        GameObject returnObj = null;

        if(pooledObjects[instanceID].Count == 0) // I am just always allow growing, TODO: Grow by more than 1 at a time.
        {

            GameObject obj = Instantiate(objectType) as GameObject;
            CleanPoolObject(obj);
            pooledObjects[instanceID].Push(obj);
            returnObj = obj;

            Debug.LogWarning("Pool might be too small for: " + objectType.name);
        }

        //If there are any left in the pool...
        if (pooledObjects[instanceID].Count > 0)
        {
            GameObject pooledObject = pooledObjects[instanceID].Pop();
            //Debug.Log(objectType.name + " PoolRemaining: " + pooledObjects[instanceID].Count);
            pooledObject.transform.parent = null;
            returnObj = pooledObject;
        }
       

        if(!instancedPoolObjects.ContainsKey(returnObj))
        {
            instancedPoolObjects.Add(returnObj, objectType);
        }

        // returnObj.SetActive(true);
        if (setActive)
        {
            SetActiveRecursively(returnObj, true);
        }
        return returnObj;
    }

    public int GetRemainingPoolSize(GameObject objectType)
    {
        return pooledObjects[objectType.GetInstanceID()].Count;
    }

    IEnumerator DelayPoolObjectRoutine(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        PoolObject(obj);
    }

    public void PoolObject(GameObject obj, float delay)
    {
        StartCoroutine(DelayPoolObjectRoutine(obj, delay));
    }

    public void PoolObject(GameObject obj)
    {
        CleanPoolObject(obj);

        Debug.Assert(instancedPoolObjects.ContainsKey(obj), "Object not found in pool: " + obj.name, obj);
        pooledObjects[ instancedPoolObjects[obj].GetInstanceID() ].Push(obj);

        instancedPoolObjects.Remove(obj); // this causes a bit of churn, but is handy for keeping track of all instances over pooled
    }

    // Be very careful with this.  If anyone is holding any references to a pooled object, things get wonky
    public void PoolAllObjects()
    {
        // We don't need to run any of the delay pool coroutines anymore, as we're pooling everything now
        StopAllCoroutines();

        foreach(var key in instancedPoolObjects.Keys)
        {
            CleanPoolObject(key);
            pooledObjects[ instancedPoolObjects[key].GetInstanceID() ].Push(key);
        }

        instancedPoolObjects.Clear();
    }

    private void CleanPoolObject(GameObject obj)
    {
        //obj.SendMessage("OnClean"); // OnDisable gets called intrinsically
        obj.SetActive(false);
      //  SetActiveRecursively(obj, false);
        obj.transform.parent = containerObject.transform;
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