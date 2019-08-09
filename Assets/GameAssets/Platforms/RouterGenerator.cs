using UnityEngine;
using System.Collections;

public class RouterGenerator : MonoBehaviour {

    BrickDecay[] BrickChildren;

    [SerializeField]
    int RouteSize = 2;

    // Use this for initialization
    void Start () {
        BrickChildren = GetComponentsInChildren<BrickDecay>();
	}
	
    void GenerateRouter()
    {
        transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
            
        if( BrickChildren != null)
        {
            for (int i = 0; i < BrickChildren.Length; ++i)
            {
                BrickChildren[i].gameObject.SetActive(true);
            }

            int routeStartIdx = Random.Range(0, BrickChildren.Length - RouteSize);
            int routeEndIdx = routeStartIdx + RouteSize;
            for (int i = routeStartIdx; i <= routeEndIdx; ++i)
            {
                //Debug.Log("Disabling Route Block " + i.ToString());
                BrickChildren[i].gameObject.SetActive(false);
            }
        }
    }


    IEnumerator DelayedGenerateCoroutine()
    {
        yield return null;

        GenerateRouter();
    }

    void OnEnable()
    {
        //GenerateRouter();
        StartCoroutine(DelayedGenerateCoroutine());
    }
}
