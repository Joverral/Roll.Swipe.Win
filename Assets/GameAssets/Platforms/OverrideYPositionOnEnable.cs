using UnityEngine;
using System.Collections;

public class OverrideYPositionOnEnable : MonoBehaviour {

    [SerializeField]
    float yPosition;

    void OnEnable()
    {
        this.transform.position = new Vector3(this.transform.position.x, yPosition, this.transform.position.z);
    }
}
