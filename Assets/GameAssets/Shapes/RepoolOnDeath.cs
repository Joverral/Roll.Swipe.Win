using UnityEngine;
using System.Collections;

public class RepoolOnDeath : MonoBehaviour {
    public void OnEnterDeathZone()
    {
        Debug.Log("Reclaim me!!!");
        GameObjectPooler.Current.PoolObject(gameObject);
    }
}
