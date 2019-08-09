using UnityEngine;
using System.Collections;

public class LifeSpanPooler : MonoBehaviour {


    [SerializeField]
    float lifeSpan = 3.0f;
    WaitForSeconds waitForLifeSpan;
    Coroutine lifeSpanCoroutine;

    void Start()
    {
       // waitForSeconds = new WaitForSeconds(Lifespan);
    }

    IEnumerator LifeSpanCoroutine()
    {
        yield return waitForLifeSpan;

        GameObjectPooler.Current.PoolObject(this.gameObject);
    }

    void OnEnable()
    {
        if(waitForLifeSpan == null)
        {
            waitForLifeSpan = new WaitForSeconds(lifeSpan);
        }

        if (lifeSpanCoroutine == null)
        {
            lifeSpanCoroutine = StartCoroutine(LifeSpanCoroutine());
        }
    }

    void StopLifeClock()
    {
        if (lifeSpanCoroutine != null)
        {
            StopCoroutine(lifeSpanCoroutine);
            lifeSpanCoroutine = null;
        }
    }

    void OnDisable()
    {
        StopLifeClock();
    }
}
