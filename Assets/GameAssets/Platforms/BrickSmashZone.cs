using UnityEngine;
using System.Collections;

public class BrickSmashZone : MonoBehaviour {

	public void OnTriggerEnter2D(Collider2D other)
    {
        var decayingBrick = other.transform.GetComponent<BrickDecay>();
        if (decayingBrick)
        {
            var vecToOther = other.transform.position - this.transform.position;
            var myCircleCollider = this.GetComponent<CircleCollider2D>();

            var hit = Physics2D.CircleCast(this.transform.position, myCircleCollider.radius, vecToOther);
            if(hit)
            {
                decayingBrick.Decay(hit.point, vecToOther * 100.0f, float.MaxValue);
            }
            //decayingBrick.Decay(collision.contacts[0].point, collision.relativeVelocity, float.MaxValue);
            
        }
    }
}
