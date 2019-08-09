using UnityEngine;
using System.Collections;

public class RevertLocalPositionOnDisable : MonoBehaviour {

    Quaternion orgLocalRotation;
    Vector3 orgLocalPos;
     
    void OnStart()
    {
        orgLocalRotation = this.transform.localRotation;
        orgLocalPos = this.transform.localPosition;
    }

	void OnDisable()
    {
        this.transform.localRotation = orgLocalRotation;
        this.transform.localPosition = orgLocalPos;
    }
}
