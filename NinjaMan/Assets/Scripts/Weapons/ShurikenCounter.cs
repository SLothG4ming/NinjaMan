using UnityEngine;
using TMPro;

public class ShurikenCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text shurikenText;
    private int _shurikenCounter = 0;

    public void SetShurikenCount(int count)
    {
        if (shurikenText != null) // Check if the shurikenText is assigned
        {
            _shurikenCounter = count;
            shurikenText.text = _shurikenCounter.ToString();
        }
        else
        {
            Debug.LogWarning("ShurikenText is not assigned in the Inspector.");
        }
    }
}
