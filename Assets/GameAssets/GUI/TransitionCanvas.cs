using UnityEngine;
using System.Collections;

public interface ITransitionCanvas
{
    void TransitionIn();
    void TransitionOut();
}


public class TransitionCanvas : MonoBehaviour, ITransitionCanvas
{

    [SerializeField]
    Vector2 offScreenOffset = new Vector2(0, -1000);

    [SerializeField]
    AnimationCurve offsetCurve;

    [SerializeField]
    float transitionTime = 1.0f;

    Coroutine outRoutine;
    Coroutine inRoutine;

    RectTransform trans2d;

    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    IEnumerator TransitionInCoroutine(float currentTransitionTime)
    {
        trans2d = this.GetComponent<RectTransform>();
        while (currentTransitionTime > 0.0f)
        {
            trans2d.anchoredPosition = Vector2.Lerp(Vector2.zero, offScreenOffset, offsetCurve.Evaluate(currentTransitionTime / transitionTime));
            currentTransitionTime -= Time.unscaledDeltaTime;
            yield return waitForEndOfFrame;

        }

        trans2d.anchoredPosition = Vector2.zero;
        inRoutine = null;
    }

    IEnumerator TransitionOutCoroutine(float currentTransitionTime)
    {
        trans2d = this.GetComponent<RectTransform>();
        while (currentTransitionTime > 0.0f)
        {
            trans2d.anchoredPosition = Vector2.LerpUnclamped(offScreenOffset, Vector2.zero, offsetCurve.Evaluate(currentTransitionTime / transitionTime));
            currentTransitionTime -= Time.unscaledDeltaTime;
            yield return waitForEndOfFrame;

        }

        trans2d.anchoredPosition = offScreenOffset;
        outRoutine = null;
    }

    public void TransitionIn()
    {
       // Debug.Log("Transitioning In: " + this.name);

        if (inRoutine != null)
        {
            Debug.Log("Trying to double transition in, ignoring second attempt");
            return;
            //StopCoroutine(inRoutine);
        }

        if (outRoutine != null)
        {
            StopCoroutine(outRoutine);
            outRoutine = null;
        }

        gameObject.SendMessage("OnTransitionIn", SendMessageOptions.DontRequireReceiver);
      

        inRoutine = StartCoroutine(TransitionInCoroutine(transitionTime));
    }

    public void TransitionOut()
    {
        //Debug.Log("Transitioning Out: " + this.name);

        if (outRoutine != null)
        {
            Debug.Log("Trying to double transition OUT, ignoring second attempt");
            return;
            //StopCoroutine(outRoutine);
        }

        if (inRoutine != null)
        {
            StopCoroutine(inRoutine);
            inRoutine = null;
        }

        outRoutine = StartCoroutine(TransitionOutCoroutine(transitionTime));
    }

    // Use this for initialization
    void Start()
    {
        trans2d = this.GetComponent<RectTransform>();
    }

    //// Update is called once per frame
    //void Update () {

    //}
}
