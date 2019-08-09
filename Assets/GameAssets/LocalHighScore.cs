using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LocalHighScore : MonoBehaviour {

    // Laziness, These don't really need to be here:
    [SerializeField]
    Text nameHeaderText;

    [SerializeField]
    Text levelHeaderText;

    [SerializeField]
    Text scoreHeaderText;

    [SerializeField]
    RectTransform contentContainer;

    public struct HighScoreEntry
    {
        public string Name;
        public int Level;
        public int Score;
    }

    [SerializeField]
    int NumHighScores = 10;

    [SerializeField]
    int Spacing = 5;

    List<HighScoreEntry> HighScoreList = new List<HighScoreEntry>();

    List<Text> nameTexts = new List<Text>();
    List<Text> levelTexts = new List<Text>();
    List<Text> scoreTexts = new List<Text>();

    int playerPos = -1;

    // Use this for initialization
    void Start()
    {
        LoadHighScores();
    }

    public void DisplayScores(int preSelectedPosition)
    {
        var nameHeaderRect = nameHeaderText.GetComponent<RectTransform>().rect;
        var levelHeaderRect = levelHeaderText.GetComponent<RectTransform>().rect;
        var scoreHeaderRect = scoreHeaderText.GetComponent<RectTransform>().rect;

        var nameHeaderAnchorPos = nameHeaderText.GetComponent<RectTransform>().anchoredPosition;
        if (nameTexts.Count == 0)
        {
            for (int i = 0; i < NumHighScores; ++i)
            {
                nameTexts.Add(GameObject.Instantiate<Text>(nameHeaderText));
                nameTexts[i].GetComponent<RectTransform>().SetParent(contentContainer, false); // oddly not inherited from instance

                levelTexts.Add(GameObject.Instantiate<Text>(levelHeaderText));
                levelTexts[i].GetComponent<RectTransform>().SetParent(contentContainer, false); // oddly not inherited from instance

                scoreTexts.Add(GameObject.Instantiate<Text>(scoreHeaderText));
                scoreTexts[i].GetComponent<RectTransform>().SetParent(contentContainer, false); // oddly not inherited from instance


                nameTexts[i].GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(nameHeaderAnchorPos.x, nameHeaderRect.y - (nameHeaderRect.height + Spacing) * (i + 1));

                levelTexts[i].GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(levelHeaderRect.x, levelHeaderRect.y - (levelHeaderRect.height + Spacing) * (i + 1));

                scoreTexts[i].GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(scoreHeaderRect.x - 10.0f, scoreHeaderRect.y - (scoreHeaderRect.height + Spacing) * (i + 1));
            }
        }

        for (int i = 0; i < HighScoreList.Count; ++i)
        {
            nameTexts[i].text = HighScoreList[i].Name;
            levelTexts[i].text = HighScoreList[i].Level.ToString();  
            scoreTexts[i].text = HighScoreList[i].Score.ToString();
        }

        if (playerPos != -1)
        {
            nameTexts[playerPos].color = Color.white;
            levelTexts[playerPos].color = Color.white;
            scoreTexts[playerPos].color = Color.white;

            EventSystem.current.SetSelectedGameObject(nameTexts[playerPos].gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(nameTexts[0].gameObject);
        }
    }

    public int AddScore(string newName, int newLevel, int newScore)
    {
        if(playerPos != -1)
        {
            nameTexts[playerPos].color = Color.green;
            levelTexts[playerPos].color = Color.green;
            scoreTexts[playerPos].color = Color.green;
        }

        playerPos = -1;

        // meh, just walk through the scores linearly, we're not dealing with large data
        for(int i = 0; i < HighScoreList.Count; ++i)
        {
            if(HighScoreList[i].Score < newScore)
            {
                HighScoreList.Insert(i, new HighScoreEntry() { Score = newScore, Level = newLevel, Name = newName });
                HighScoreList.RemoveAt(HighScoreList.Count - 1); // remove the last entry
                playerPos = i;
                break;
            }
        }

      

        SaveToPlayerPrefs();

        return playerPos;
    }

    private void SaveToPlayerPrefs()
    {
        for (int i = 0; i < NumHighScores; ++i)
        {
            string iStr = i.ToString();
            PlayerPrefs.SetInt("Level" + iStr, HighScoreList[i].Level);
            PlayerPrefs.SetInt("Score" + iStr, HighScoreList[i].Score);
            PlayerPrefs.SetString("Name" + iStr, HighScoreList[i].Name);
        }

        PlayerPrefs.Save();
    }

    private void LoadHighScores()
    {
        // Going to assume the high scores are saved sorted.
        HighScoreList.Clear();
        for (int i = 0; i < NumHighScores; ++i)
        {
            string iStr = i.ToString();
            HighScoreList.Add( new HighScoreEntry() {
                                                      Level = PlayerPrefs.GetInt("Level" + iStr, 0),
                                                      Score = PlayerPrefs.GetInt("Score" + iStr, 0),
                                                      Name = PlayerPrefs.GetString("Name" + iStr, "Nobody")
                                                    });
        }
    }
}
