using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoadScript : MonoBehaviour {

    [SerializeField]
    string LevelName;


    public void LoadLevel()
    {
        SceneManager.LoadScene(LevelName, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
