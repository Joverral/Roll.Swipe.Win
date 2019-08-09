using UnityEngine;
using System.Collections;

public class SkipEnticer : MonoBehaviour {

    [SerializeField]
    GameObject Player;

    [SerializeField]
    float MinDistance = 3f;
    // Use this for initialization
    WaitForSeconds waitForSeconds;

	void Start () {
        waitForSeconds = new WaitForSeconds(1.5f); 
        StartCoroutine(CheckForProgress());
	}

    public IEnumerator CheckForProgress()
    {
        while (this.enabled)
        {
            float playerX = Player.transform.position.x;

            yield return waitForSeconds;

            if (Time.timeScale == 1.0f && // game is running
                (Player.transform.position.x - playerX) < MinDistance)
            {
                this.GetComponent<Animation>().Play();
            }
        }
    }
}
