using UnityEngine;
using System.Collections;

public class MoveUpDown : MonoBehaviour {
    [SerializeField]
    float Amplitude =  5.0f;

    [SerializeField]
    float Rate = 5.0f;

    RectTransform rectTrans;
    Vector3 orgPos;
    void Start()
    {
        rectTrans = this.GetComponent<RectTransform>();
        orgPos = rectTrans.localPosition;
    }
	
	// Update is called once per frame
	void Update () {
        rectTrans.localPosition = new Vector3(rectTrans.localPosition.x,
                                              orgPos.y + Mathf.Sin(Time.time* Rate) * Amplitude * rectTrans.localScale.y,
                                              rectTrans.localPosition.z);
    }
}
