using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject records;

    public AudioSource clickSound;
    public AudioSource backgroundAudio;

    public GameObject muteButton;

    public Sprite muteSound;
    public Sprite unMuteSound;

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

    public void ClickSound()
    {
        clickSound.enabled = true;
        clickSound.Play();
    }

    public void MuteAudio()
    {
            if (backgroundAudio.isPlaying)
            {
                backgroundAudio.Pause();
                muteButton.GetComponent<Image>().sprite = unMuteSound;
            }
            else if (!backgroundAudio.isPlaying)
            {
                backgroundAudio.Play();
                muteButton.GetComponent<Image>().sprite = muteSound;
            }        
    }
}