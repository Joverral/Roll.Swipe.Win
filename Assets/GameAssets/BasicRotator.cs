using UnityEngine;
using System.Collections;

public class BasicRotator : MonoBehaviour {

    [SerializeField]
    float rotationSpeed = 1.0f;


	// Update is called once per frame
	void Update () {
        this.gameObject.transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
	}
}
