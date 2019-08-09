using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

    [SerializeField]
    GameObject explosionPrefab;

    bool hasExploded;

    private void Explode(Vector2 explosionPos)
    {
        var explosion = GameObjectPooler.Current.GetObject(explosionPrefab);
        explosion.transform.position = explosionPos;
        //Debug.Log("Bullet Going Kaboom!");
        explosion.GetComponent<BombExplosion>().Kaboom();

        hasExploded = true;
        GameObjectPooler.Current.PoolObject(gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!hasExploded)
        {
            Explode(collision.contacts[0].point);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasExploded)
        {
            Explode(collision.contacts[0].point);
        }
    }

    void OnEnable()
    {
        hasExploded = false;
    }
}
