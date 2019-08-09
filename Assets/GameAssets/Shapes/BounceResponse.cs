using UnityEngine;
using System.Collections;

public class BounceResponse : MonoBehaviour {

    [SerializeField]
    float bounceTime = 1.0f;
    float timer;

    [SerializeField]
    AnimationCurve scalarCurve;

    [SerializeField]
    AudioClip responseClip;
    AudioSource audioSource;
    

    Vector3 orgScale;

    Coroutine bounceResponseRoutine;

    void Start()
    {
        orgScale = transform.localScale;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    IEnumerator BounceResponseCoroutine()
    {
        while (timer < bounceTime)
        {
            float scalarCurveAmt = scalarCurve.Evaluate(timer / bounceTime);
            gameObject.transform.localScale = orgScale * scalarCurveAmt;
            timer += Time.deltaTime;

            yield return null;
        }
    }

    void OnBounce()
    {
        if (bounceResponseRoutine != null)
        {
            StopCoroutine(bounceResponseRoutine);
        }

        timer = 0.0f;

        // todo, randomize the pitch slightly
        audioSource.PlayOneShot(responseClip);
        bounceResponseRoutine = StartCoroutine(BounceResponseCoroutine());
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        OnBounce();
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        OnBounce();
    }
}
