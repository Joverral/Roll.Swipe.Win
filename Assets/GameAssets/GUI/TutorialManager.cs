using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class TutorialManager : MonoBehaviour {

    [SerializeField]
    TransitionCanvas[] TutorialElements;

    Stack<TransitionCanvas> panelStack = new Stack<TransitionCanvas>();

    public void PushPanel(TransitionCanvas newPanel)
    {
        if (panelStack.Count > 0 && panelStack.Peek() == newPanel)
        {
            PopCurrentMenu();
        }
        else
        {
            // slight kludge, since we never make them inactive
            newPanel.gameObject.SetActive(true);

            if (panelStack.Count > 0)
            {
                panelStack.Peek().TransitionOut();
            }

            newPanel.TransitionIn();
            panelStack.Push(newPanel);
        }
    }

    void Start()
    {
        for (int i = TutorialElements.Length - 1; i >= 0; --i)
        {
            panelStack.Push(TutorialElements[i]);
        }
    }

    public void EnableTutorialManager()
    {
        if (panelStack.Count > 0)
        {
            panelStack.Peek().gameObject.SetActive(true);
            panelStack.Peek().TransitionIn();
        }
    }

    public void DisableTutorialManager()
    {
        if (panelStack.Count > 0)
        {
            panelStack.Peek().gameObject.SetActive(false);
            //panelStack.Peek().TransitionOut();
        }
    }

    public void PopCurrentMenu()
    {
        panelStack.Pop().TransitionOut();

        if (panelStack.Count > 0)
        {
            panelStack.Peek().gameObject.SetActive(true);
            panelStack.Peek().TransitionIn();
        }
    }

    public void PopAll()
    {
        while (panelStack.Count > 0)
        {
            panelStack.Pop().TransitionOut();
        }
    }

    public bool IsTop(TransitionCanvas canvas)
    {
        return panelStack.Count > 0 && panelStack.Peek() == canvas;
    }

    public bool IsEmpty()
    {
        return panelStack.Count > 0;
    }
}
