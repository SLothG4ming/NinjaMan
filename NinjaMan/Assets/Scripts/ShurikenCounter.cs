using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShurikenCounter : MonoBehaviour
{
    [SerializeField] private Image shurikenImage;
    [SerializeField] private TextMeshProUGUI shurikenText;
    private int shurikenCount = 0;

    public void SetShurikenCount(int count)
    {
        shurikenCount = count;
        shurikenText.text = count.ToString();
    }

    public void SetShurikenSprite(Sprite sprite)
    {
        shurikenImage.sprite = sprite;
    }
}
