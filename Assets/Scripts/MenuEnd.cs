using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    public GameObject gameOverUI;

    private bool isGameOver = false;
    private float catchTimer = 0.0f; // Timer to track how long players are in the same position
    private float catchDuration = 3.0f; // Time in seconds required to catch the player

    void Update()
    {
        if (!isGameOver)
        {
            // Check if the enemy and player have the same position
            if (Vector3.Distance(player.transform.position, enemy.transform.position) < 0.1f)
            {
                catchTimer += Time.deltaTime;

                if (catchTimer >= catchDuration)
                {
                    GameOver();
                }
            }
            else
            {
                catchTimer = 0.0f; // Reset the timer when they are not in the same position
            }
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        player.SetActive(false);
        gameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

