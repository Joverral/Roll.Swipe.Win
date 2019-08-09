using UnityEngine;
using System.Collections;

public class PoolOnCollision : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObjectPooler.Current.PoolObject(gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
      //  GameObjectPooler.Current.PoolObject(gameObject);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        GameObjectPooler.Current.PoolObject(gameObject);
    }
}
