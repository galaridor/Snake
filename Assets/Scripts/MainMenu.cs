using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject records;

    [SerializeField]
    private int lastScore;

    [SerializeField]
    private int bestScore;

    [SerializeField]
    private int gameCounter;

    private void Awake()
    {
        bestScore = PlayerPrefs.GetInt("BestScore");
        lastScore = PlayerPrefs.GetInt("LastScore");
        gameCounter = PlayerPrefs.GetInt("gameCounter");

        SetRecords();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void SetRecords()
    {
        if (gameCounter > 0)
        {
            if(lastScore> bestScore)
            {
                bestScore = lastScore;
                PlayerPrefs.SetInt("BestScore",bestScore);
                PlayerPrefs.Save();
            }
            records.GetComponent<TextMeshProUGUI>().text = bestScore.ToString();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}