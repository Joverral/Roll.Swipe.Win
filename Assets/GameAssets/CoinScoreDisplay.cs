using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class CoinScoreDisplay : MonoBehaviour {

    [SerializeField]
    AnimationCurve displayCurve;

 //   public Vector2 FinalOffset;
    public float ScreenTime = 1.0f;
    public float FinalHeightOffset = 10.0f;
    float startTime = 0.0f;
    RectTransform rectTransform;
    Vector2 finalPosition;
   // Vector3 orgPosition;

    public RectTransform endTransform;

	// Use this for initialization
	void Start () {

        
       
	}
	
	// Update is called once per frame
	void Update () {
        rectTransform = GetComponent<RectTransform>();
        startTime += Time.deltaTime;
        // rectTransform.position = rectTransform.position + Vector3.up * Time.deltaTime * FinalHeightOffset;

        rectTransform.position = Vector3.Lerp(rectTransform.position, endTransform.position, displayCurve.Evaluate(startTime / ScreenTime));
        if (startTime >= ScreenTime)
        {
            this.gameObject.SetActive(false); // or pool orself?
        }
	}

    void OnEnable()
    {
        
        rectTransform = GetComponent<RectTransform>();
        //orgPosition = rectTransform.position;
        startTime = 0.0f;
      //  finalPosition = new Vector2(rectTransform.position.x + FinalOffset.x, rectTransform.position.y + FinalOffset.y);
    }
}
