using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class GameWonUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text retryButton;
    [SerializeField] private TMP_Text mainMenuButtonText;
    [SerializeField] private TMP_Text pointCounterText;
    [SerializeField] private TMP_Text killCounterText;
    //Secrets not used for now

    // Set the point count and kill count
    public void SetStatistics(int points, int kills)
    {
        pointCounterText.text = points.ToString();
        killCounterText.text = kills.ToString();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
