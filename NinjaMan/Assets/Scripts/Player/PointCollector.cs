using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PointCollector : MonoBehaviour
{
    [SerializeField] private int _points = 0;
    [SerializeField] private TMP_Text _pointText; // Reference to the TMP Text component

    private void Start()
    {
        // Make sure pointText is assigned in the Inspector
        if (_pointText == null)
        {
            Debug.LogError("PointText is not assigned in the Inspector.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Points"))
        {
            Destroy(collision.gameObject);
            _points++;
           

            // Check if the player has collected 50 points
            if (_points >= 50)
            {
                SceneManager.LoadScene("GameWon");
            }

            // Update the UI with the new point count directly
            if (_pointText != null)
            {
                _pointText.text = _points.ToString();
            }
        }
    }
}
