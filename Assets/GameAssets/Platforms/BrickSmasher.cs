using UnityEngine;
using System.Collections;

public class BrickSmasher : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.enabled)
        {
            Debug.Log("whatddya know, this worked!");
            var decayingBrick = collision.transform.GetComponent<BrickDecay>();
            if (decayingBrick)
            {
                decayingBrick.Decay(collision.contacts[0].point, collision.relativeVelocity, float.MaxValue);

            }
        }
    }
}
