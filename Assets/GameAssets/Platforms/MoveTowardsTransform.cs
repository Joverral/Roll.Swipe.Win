using UnityEngine;
using System.Collections;

public class MoveTowardsTransform : MonoBehaviour {

    [SerializeField]
    Transform target;

    [SerializeField]
    float moveRate = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        var moveVec = (target.transform.position - transform.transform.position).normalized;

        this.transform.position += moveVec * moveRate * Time.fixedDeltaTime;
    }
}
