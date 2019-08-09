using UnityEngine;
using System.Collections;

public class BombScript : MonoBehaviour {

    [SerializeField]
    GameObject explosionPrefab;

    [SerializeField]
    ParticleSystem[] effects;

    [SerializeField]
    float fuseTimer = 1.0f;

    bool hasLitFuse = false;

    WaitForSeconds waitForSeconds;

    void Start()
    {
        waitForSeconds = new WaitForSeconds(fuseTimer);
    }

    IEnumerator LightFuseCoroutine()
    {
        for(int i = 0; i < effects.Length; ++i)
        {
            effects[i].Play();
        }
        yield return waitForSeconds;
        Explode();
    }

    private void Explode()
    {
        var explosion = GameObjectPooler.Current.GetObject(explosionPrefab);
        explosion.transform.position = this.transform.position;
        Debug.Log("Bomb Going Kaboom!");
        explosion.GetComponent<BombExplosion>().Kaboom();

        for (int i = 0; i < effects.Length; ++i)
        {
            effects[i].Stop();
        }

        GameObjectPooler.Current.PoolObject(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!hasLitFuse)
        {
            hasLitFuse = true;
            StartCoroutine(LightFuseCoroutine());
        }
        
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!hasLitFuse)
        {
            hasLitFuse = true;
            StartCoroutine(LightFuseCoroutine());
        }
    }

    void OnEnable()
    {
        hasLitFuse = false;
        for (int i = 0; i < effects.Length; ++i)
        {
            effects[i].Stop();
        }
    }

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Instantiate(explosion, transform.position, transform.rotation);
        
    //    Destroy(gameObject);
    //}
}
