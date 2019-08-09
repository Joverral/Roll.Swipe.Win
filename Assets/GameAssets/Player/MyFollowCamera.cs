using UnityEngine;
using System.Collections;

public class MyFollowCamera : MonoBehaviour {

    public Transform target;
    public Transform secondaryTarget;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    
    public float targetXOffset = 10.0f;
    public float targetYOffset = 6.0f;

    public float targetMaxXVelocity = 50.0f;
    public AnimationCurve targetXVelocityCurve;

    public float maxDistance = 10.0f;
    public float minDistance = 1.0f;
     
    void Update()
    {
       // Vector3 targetPosition = target.TransformPoint(new Vector3(0, -6, -10));
        //transform.position = Vector3.SmoothDamp((transform.position, targetPosition, ref velocity, smoothTime);
        //transform.position = targetPosition;
        var xVel = target.GetComponent<Rigidbody2D>().velocity.x;
        float dir = xVel >= 0.0f ? 1.0f : -1.0f;

        var xT = Mathf.Abs(xVel / targetMaxXVelocity);
        var curveT = targetXVelocityCurve.Evaluate(xT);
        float xOffset = curveT * targetXOffset;

        //Debug.Log(string.Format("xVel = {0}, xOffset = {1}, xT={2}, curveT={3}", xVel, xOffset, xT, xOffset));  


        var destPosition = new Vector3(target.position.x + xOffset * dir, targetYOffset, -1);

        if((target.position.x + xOffset )> secondaryTarget.position.x)
        {
            destPosition = new Vector3(secondaryTarget.position.x, targetYOffset, -1);
        }

        var mag = (transform.position - destPosition).sqrMagnitude;

        //transform.position = Vector3.MoveTowards(transform.position, destPosition, Time.deltaTime * mag * mag);
        //transform.position = Vector3.Lerp(transform.position, destPosition, smoothTime * Time.deltaTime);
        
        //transform.position = Vector3.MoveTowards(transform.position, destPosition, Time.deltaTime * mag * maxDistance);
        //mag = Mathf.Min(minDistance, mag);
        transform.position = Vector3.SmoothDamp(transform.position, destPosition, ref velocity, smoothTime );
        //transform.position = destPosition;

        //transform.position = new Vector3(transform.position.x > secondaryTarget.position.x ? secondaryTarget.position.x : transform.position.x
        //    , transform.position.y, transform.position.z);
    }
}
