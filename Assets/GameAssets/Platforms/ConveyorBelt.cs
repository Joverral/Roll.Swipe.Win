using UnityEngine;
using System.Collections;

public class ConveyorBelt : MonoBehaviour {

   [SerializeField]
   float Speed = 2.0f;
   [SerializeField]
   Vector2 Direction = new Vector2(1, 0);


    	// Use this for initialization
	void Start () {

        Direction = this.transform.TransformDirection(Direction);
	}

    void Update()
    {
        var transDirection = this.transform.TransformDirection(Direction);

        // if the transformed direction is not facing the goal, negate it.

        var goalDirection = LevelGenerator.Current.Goal.transform.position - gameObject.transform.position;
        var dir = Vector3.Dot(transDirection, goalDirection) > 0.0f ? 1.0f : -1.0f;
        Debug.DrawLine(this.transform.position, (this.transform.position + new Vector3(transDirection.x, transDirection.y) * Speed * dir));
    }

    // TODO, not a fan of how these work this method blows, look into on stay b
    //void FixedUpdate()
    //{
        //Direction = this.transform.TransformDirection(new Vector2(1, 0));

        //rgBody2D.position -= Direction * Speed * Time.deltaTime;
        //rgBody2D.MovePosition(rgBody2D.position + Direction * Speed * Time.deltaTime);

        //Debug.DrawLine(this.transform.position, (this.transform.position + new Vector3(Direction.x, Direction.y) * Speed));
    //}

    void OnCollide(Collision2D coll, ForceMode2D force)
    {
        if (coll.collider.attachedRigidbody != null)
        {
            var dir = 1.0f;
            if ( (transform.position.x + this.transform.localScale.x) > LevelGenerator.Current.Goal.transform.position.x)
            {
                dir = -1.0f;
            }

            coll.collider.attachedRigidbody.AddForceAtPosition(Direction * Speed * Time.deltaTime * dir, coll.contacts[0].point, force);
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        OnCollide(coll, ForceMode2D.Impulse);
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        OnCollide(coll, ForceMode2D.Impulse);
    }
}
