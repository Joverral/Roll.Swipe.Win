using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CheckSkippable : MonoBehaviour {

    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    Button[] disableIfNotSkippable;

    public void OnTransitionIn()
    {
        for(int i = 0; i < disableIfNotSkippable.Length; ++i)
        {
            disableIfNotSkippable[i].interactable = gameManager.CanSkip();
        }
    }
}
