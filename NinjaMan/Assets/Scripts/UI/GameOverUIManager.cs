using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameOverUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text retryButton;
    [SerializeField] private TMP_Text mainMenuButtonText;

   

    public void RestartLevel()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
