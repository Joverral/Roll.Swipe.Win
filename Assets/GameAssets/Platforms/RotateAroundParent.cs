using UnityEngine;
using System.Collections;

public class RotateAroundParent : MonoBehaviour {

    [SerializeField]
    float rotationRate = 1.0f;

	void Update ()
    {
        transform.RotateAround(this.transform.parent.position, Vector3.forward, rotationRate * Time.deltaTime);
	}
}
