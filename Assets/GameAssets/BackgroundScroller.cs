using UnityEngine;
using System.Collections;

public class BackgroundScroller : MonoBehaviour {

    [SerializeField]
    float divisor = 2.0f;

    [SerializeField]
    float xOffset = 0.0f;

    // Update is called once per frame
    void LateUpdate () {

        transform.localPosition = new Vector3(xOffset - Camera.main.transform.position.x / divisor,
                                               transform.localPosition.y,
                                               transform.localPosition.z);
	}
}
