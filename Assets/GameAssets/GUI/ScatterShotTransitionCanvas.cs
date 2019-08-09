using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ScatterShotTransitionCanvas : MonoBehaviour, ITransitionCanvas
{
    //List<GameObject> mychildren;
    [SerializeField]
    AnimationCurve offsetCurve;

    [SerializeField]
    float transitionTime = 1.0f;

    // todo should probably be screen coords
    [SerializeField]
    float minDistance = 500.0f;

    [SerializeField]
    float maxDistance = 1500.0f;

    public bool UseUnscaledTime = true;

    public float TransitionTime { get { return transitionTime; }}

    class ChildData
    {
        public Vector2 onScreenPosition;
        public Vector2 offScreenPosition;
        public RectTransform childRect;
    }

    List<ChildData> childData;

    Coroutine outRoutine;
    Coroutine inRoutine;

    public void TransitionIn()
    {
        if (inRoutine != null)
        {
            Debug.Log("Trying to double transition in, ignoring second attempt: " + this.gameObject.name);
            return;
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
        if (outRoutine != null)
        {
            Debug.Log("Trying to double transition OUT, ignoring second attempt: " + this.gameObject.name);
            return;
        }

        if (inRoutine != null)
        {
            StopCoroutine(inRoutine);
            inRoutine = null;
        }

        outRoutine = StartCoroutine(TransitionOutCoroutine(transitionTime));
    }
    
    void LazyInit()
    {
        childData = new List<ChildData>(transform.childCount);

        for (int i = 0; i < this.transform.childCount; ++i)
        {
            var rect = transform.GetChild(i).GetComponent<RectTransform>();
            if (rect == null)
            {
                continue;
            }

            ChildData child = new ChildData();
            child.childRect = rect;
            child.onScreenPosition = rect.anchoredPosition;

            childData.Add(child);
        }
    }


    IEnumerator TransitionInCoroutine(float currentTransitionTime)
    {
        if(childData == null)
        {
            LazyInit();
        }

        // assign random locations to all the children to transition into
        for (int i = 0; i < childData.Count; ++i)
        {
            float randomAngle = Random.Range(0, Mathf.PI * 2.0f);
            float x = Mathf.Cos(randomAngle);
            float y = Mathf.Sin(randomAngle);

            childData[i].offScreenPosition = new Vector2(x, y) * Random.Range(minDistance, maxDistance);
        }

        while (currentTransitionTime != 0.0f)
        {
            yield return null;
            currentTransitionTime -= UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            currentTransitionTime = Mathf.Max(currentTransitionTime, 0.0f);
            for (int i = 0; i < childData.Count; ++i)
            {
                childData[i].childRect.anchoredPosition = Vector2.Lerp(childData[i].onScreenPosition, childData[i].offScreenPosition, offsetCurve.Evaluate(currentTransitionTime / transitionTime));
            }
        }

        inRoutine = null;
    }

    IEnumerator TransitionOutCoroutine(float currentTransitionTime)
    {
        if (childData == null)
        {
            LazyInit();
        }

        // assign random locations to all the children to transition out to
        for (int i = 0; i < childData.Count; ++i)
        {
            float randomAngle = Random.Range(0, Mathf.PI * 2.0f);
            float x = Mathf.Cos(randomAngle);
            float y = Mathf.Sin(randomAngle);

            childData[i].offScreenPosition = new Vector2(x, y) * Random.Range(minDistance, maxDistance);
        }

        while (currentTransitionTime != 0.0f)
        {
            yield return null;
            currentTransitionTime -= UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            currentTransitionTime = Mathf.Max(currentTransitionTime, 0.0f);
            for (int i = 0; i < childData.Count; ++i)
            {
                childData[i].childRect.anchoredPosition = Vector2.Lerp(childData[i].offScreenPosition, childData[i].onScreenPosition, offsetCurve.Evaluate(currentTransitionTime / transitionTime));
            }
        }

        outRoutine = null;
    }

}
