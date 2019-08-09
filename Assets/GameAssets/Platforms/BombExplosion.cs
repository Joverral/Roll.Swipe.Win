using UnityEngine;
using System.Collections;

// TODO:  Need to split up the bombexplosion physical effect from the graphical effect.

public class BombExplosion : MonoBehaviour {

    [SerializeField]
    float radius = 4.0f;

    [SerializeField]
    float force = 10.0f;

    [SerializeField]
    LayerMask layerMask;

    [SerializeField]
    float brickDecayMultiplier = 10000.0f;

    [SerializeField]
    AnimationCurve dropOffCurve;

    // If unity is singlethreaded, can i just make this static?
    //static Collider2D[] results = new Collider2D[16];

    static RaycastHit2D[] rayResults = new RaycastHit2D[16];

    //void OnEnable()
    //{
    //    int numHits = Physics2D.CircleCastNonAlloc(this.transform.position, radius,Vector2.zero, rayResults, layerMask);
    //    for (int i = 0; i < numHits; ++i)
    //    {
    //        if (rayResults[i].rigidbody)
    //        {
    //            AddExplosionForce2D(rayResults[i].rigidbody);
    //        }
    //        else
    //        {
    //            var decayingBrick = rayResults[i].transform.GetComponent<BrickDecay>();
    //            if(decayingBrick)
    //            {
    //                decayingBrick.Decay(rayResults[i].point, ExplosionForce(rayResults[i].point), Time.deltaTime * brickDecayMultiplier);
    //            }
    //        }
    //    }

    //    var particleSystem = GetComponent<ParticleSystem>();

    //    if(particleSystem != null)
    //    {
    //        particleSystem.Play();

    //    }

    //    GameObjectPooler.Current.PoolObject(this.gameObject, particleSystem.duration);
    //}

    public void OnDisable()
    {
        var particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Stop(true);
        }
    }

    public void Kaboom()
    {
        int numHits = Physics2D.CircleCastNonAlloc(this.transform.position, radius, Vector2.zero, rayResults, layerMask);
        for (int i = 0; i < numHits; ++i)
        {
            if (rayResults[i].rigidbody)
            {
                AddExplosionForce2D(rayResults[i].rigidbody);
            }
            else
            {
                var decayingBrick = rayResults[i].transform.GetComponent<BrickDecay>();
                if (decayingBrick)
                {
                    decayingBrick.Decay(rayResults[i].point, ExplosionForce(rayResults[i].point), Time.deltaTime * brickDecayMultiplier);
                }
            }
        }

        var particleSystem = GetComponent<ParticleSystem>();

        if (particleSystem != null)
        {
            particleSystem.Play(true);
        }

        GameObjectPooler.Current.PoolObject(this.gameObject, particleSystem.duration);
    }

    Vector2 ExplosionForce(Vector3 otherPos)
    {
        var dir = (otherPos - transform.position);
        float falloff = 1 - dropOffCurve.Evaluate(dir.magnitude / radius);

        return dir.normalized * force * falloff;
    }

    void AddExplosionForce2D(Rigidbody2D body)
    {
        body.AddForce(ExplosionForce(body.transform.position));
    }
}
