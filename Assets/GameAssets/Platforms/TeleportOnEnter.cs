using UnityEngine;
using System.Collections;

public class TeleportOnEnter : MonoBehaviour {

    [SerializeField]
    Transform target;
	

    void OnTriggerEnter2D(Collider2D other)
    {
        other.transform.position = target.position;
    }
}
