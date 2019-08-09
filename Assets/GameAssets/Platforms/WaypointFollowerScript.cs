using UnityEngine;
using System.Collections;

public class WaypointFollowerScript : MonoBehaviour {

    [SerializeField]
    WaypointScript waypointScript;
    [SerializeField]
    int CurrentWayPoint = 0;  // the current waypoint the object is trying to reach
    [SerializeField]
    float moveRate = 1.0f;


    Rigidbody2D followerRigidBody2D;

	// Use this for initialization
	void Start () {
        Debug.Assert(waypointScript != null, "Waypoint list not set!!", this);

        followerRigidBody2D = this.GetComponent<Rigidbody2D>();
        Debug.Assert(followerRigidBody2D, "WaypointFollower lacks rigidBody2D", this);
    }
	
	// Update is called once per frame
	void Update () {

        var waypointPos = waypointScript.WayPoints[CurrentWayPoint].position;
        Debug.Assert(followerRigidBody2D, "WaypointFollower lacks rigidBody2D", this);
        followerRigidBody2D.MovePosition(Vector2.MoveTowards(this.transform.position, waypointPos, moveRate * Time.deltaTime));
        if(this.transform.position == waypointPos)
        {
            if(++CurrentWayPoint == waypointScript.WayPoints.Length)
            {
                CurrentWayPoint = 0;
            }
        }

	}
}
