using UnityEngine;
using System.Collections;

public class VortexSuction : MonoBehaviour {

    [SerializeField]
    float suctionStrength = 10.0f;

    [SerializeField]
    float minSize = 0.1f;

    [SerializeField]
    AnimationCurve scalarCurve;

    [SerializeField]
    AnimationCurve suctionCurve;

    CircleCollider2D circleCollider2d;

	// Use this for initialization
	void Start () {
        circleCollider2d = this.GetComponent<CircleCollider2D>();
	}

    void ScaleOtherObject(Collider2D other)
    {
        Vector2 vecToVortex = (this.transform.position - other.transform.position);
        float otherRadius = other.GetComponent<CircleCollider2D>().radius * other.transform.localScale.x;

        float distanceT = vecToVortex.magnitude / (circleCollider2d.radius + otherRadius);

        
      
        float scaleAmt = scalarCurve.Evaluate(1.0f - distanceT);
        scaleAmt = Mathf.Max(scaleAmt, minSize);

        other.transform.localScale = new Vector3(scaleAmt, scaleAmt, scaleAmt);
        //Debug.Log("Scale Amt: " + scaleAmt + " Name: " + other.name);


        var otherRB = other.GetComponent<Rigidbody2D>();
        scaleAmt = suctionCurve.Evaluate(1.0f - distanceT);
        //Debug.Log("Suction Amt: " + suctionStrength * scaleAmt + " Name: " + other.name);
        otherRB.AddForce(vecToVortex * suctionStrength * scaleAmt, ForceMode2D.Force);
        otherRB.AddForce(new Vector2(-vecToVortex.y, vecToVortex.x) * 0.25f * suctionStrength * scaleAmt);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ScaleOtherObject(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        ScaleOtherObject(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        other.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //Debug.Log("EXIT: Scale Amt: " + 1.0f + " Name: " + other.name);
    }
}
