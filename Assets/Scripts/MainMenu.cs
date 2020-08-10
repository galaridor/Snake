using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject records;

    public AudioSource clickSound;
    public AudioSource backgroundAudio;

    public GameObject muteButton;

    public Sprite muteSound;
    public Sprite unMuteSound;

    [SerializeField]
    private int gameCounter;

    private void Awake()
    {
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
            Scores.allScores.Sort();
            Scores.allScores.Reverse();

            int n = 0;

            if(Scores.allScores.Count < 10)
            {
                n = Scores.allScores.Count;
            }
            else if (Scores.allScores.Count >= 10)
            {
                n = 10;
            }

            for (int i = 0; i < n; i++)
            {
                records.GetComponent<TextMeshProUGUI>().text = records.GetComponent<TextMeshProUGUI>().text + " " + Scores.allScores[i].ToString();
            }             
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