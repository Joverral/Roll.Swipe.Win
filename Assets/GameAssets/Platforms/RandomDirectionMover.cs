using UnityEngine;
using System.Collections;

public class RandomDirectionMover : MonoBehaviour {

    [SerializeField]
    float rotateRate = 1.0f;

    [SerializeField]
    float moveRate = 5.0f;

    [SerializeField]
    float moveDistance = 15.0f;


    float timer;
    Vector2 targetPos;
    Vector3 targetDir;
    float targetAngleAmt;

    WaitForSeconds waitForSeconds = new WaitForSeconds(1.0f);

    Coroutine currentCoroutine;


    IEnumerator RotateTowardsRoutine()
    {
        var moverBody = GetComponent<Rigidbody2D>();
        while (Mathf.DeltaAngle(moverBody.rotation, targetAngleAmt) != 0.0f)
        {
            moverBody.MoveRotation(Mathf.MoveTowardsAngle(moverBody.rotation, targetAngleAmt, Time.deltaTime * rotateRate));
            yield return null;
        }

        yield return waitForSeconds;
       
        targetPos = transform.position + transform.up * moveDistance;
        currentCoroutine = StartCoroutine(MoveTowardsRoutine());
    }

    IEnumerator MoveTowardsRoutine()
    {
        var moverBody = GetComponent<Rigidbody2D>();
        while (
            targetPos.x != this.transform.position.x ||
            targetPos.y != this.transform.position.y
                )
        {
            moverBody.MovePosition(Vector2.MoveTowards(transform.position, targetPos, moveRate * Time.deltaTime));
            yield return null;
        }

        yield return waitForSeconds;

        PickDestination();
    }

    void OnDisable()
    {
        if(currentCoroutine != null)
            StopCoroutine(currentCoroutine);
    }

    IEnumerator PickDestinationRoutine()
    {
        while(LevelGenerator.Current == null)
        {
            yield return waitForSeconds;
        }

        PickDestination();
    }

    void PickDestination()
    {
        Debug.Assert(LevelGenerator.Current, "LevelGenerator.Current is null!", this);
        targetPos = LevelGenerator.Current.GetRandomPoint();
        var thisPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 targetDir = targetPos - thisPos;
        targetPos = thisPos + targetDir.normalized * this.moveDistance;
        targetAngleAmt = Vector2.Angle(this.transform.up, targetDir);

        var moverBody = GetComponent<Rigidbody2D>();
        targetAngleAmt = moverBody.rotation + targetAngleAmt;
        currentCoroutine = StartCoroutine(RotateTowardsRoutine());
    }

    void OnEnable()
    {
        currentCoroutine = StartCoroutine(PickDestinationRoutine());
    }
}
