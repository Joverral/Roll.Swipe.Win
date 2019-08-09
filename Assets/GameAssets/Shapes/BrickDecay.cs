using UnityEngine;
using System.Collections;

public class BrickDecay : MonoBehaviour
{

    [SerializeField]
    float CollisionTimeout = 2.0f;
    float currentCollisionTime = 0.0f;

    [SerializeField]
    SpriteRenderer childSpriteRenderer;

    [SerializeField]
    Color startColor;
    
    [SerializeField]
    GameObject brickShrapnel;
    Color endColor;

    [SerializeField]
    AnimationCurve colorChangeCurve;

    [SerializeField]
    bool UseVelocityToDecayCollisions = false;

    void Start()
    {
        endColor = brickShrapnel.GetComponentInChildren<SpriteRenderer>().color;
    }

    void UpdateColor()
    {
        float tValue = colorChangeCurve.Evaluate(currentCollisionTime / CollisionTimeout);
        childSpriteRenderer.color = Color.Lerp(startColor, endColor, tValue);
    }

    public void Decay(Vector2 point, Vector2 velocity, float lifeTimeRemoval)
    {
        if (this.gameObject.activeSelf)
        {
            currentCollisionTime += lifeTimeRemoval;

            UpdateColor();

            if (currentCollisionTime > CollisionTimeout)
            {
                SpawnShrapnel(point, velocity);
            }
        }
    }

    private void SpawnShrapnel(Vector2 point, Vector2 velocity)
    {
        var newShrapnel = GameObjectPooler.Current.GetObject(brickShrapnel);
        newShrapnel.transform.position = transform.position;
        newShrapnel.transform.rotation = transform.rotation;
        newShrapnel.transform.localScale = transform.localScale;
        newShrapnel.GetComponent<Rigidbody2D>().AddForceAtPosition(velocity, point);

        // Reset it for future use
        currentCollisionTime = 0.0f;
        gameObject.SetActive(false);
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        Decay(coll.contacts[0].point, coll.relativeVelocity, Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if(!UseVelocityToDecayCollisions)
        {
            Decay(coll.contacts[0].point, coll.relativeVelocity, Time.deltaTime);
        }
        else
        {
            Decay(coll.contacts[0].point, coll.relativeVelocity, Time.deltaTime * coll.relativeVelocity.SqrMagnitude() / 1000.0f);
        }
    }

    void OnEnable()
    {
        currentCollisionTime = 0.0f;
        childSpriteRenderer.color = startColor;
    }
  
}
