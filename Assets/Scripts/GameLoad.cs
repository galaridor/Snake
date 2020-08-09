using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoad : MonoBehaviour
{
    private void Start()
    {
        Invoke("LoadGame", 4);
    }

    private void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}