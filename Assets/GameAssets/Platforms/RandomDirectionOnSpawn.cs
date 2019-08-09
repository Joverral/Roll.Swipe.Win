using UnityEngine;
using System.Collections;

public class RandomDirectionOnSpawn : MonoBehaviour {

	public void OnEnable()
    {
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 359.0f));
    }
}
