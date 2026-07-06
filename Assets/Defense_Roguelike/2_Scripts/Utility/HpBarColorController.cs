using UnityEngine;
using UnityEngine.UI;

public class HpBarColorController : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Image image;

    public void UpdateHealthBarColor(float hpRatio)
    {
        Color newColor;

        if (hpRatio <= 0.2f) newColor = new Color(0.8f, 0, 0);
        else if (hpRatio <= 0.4f) newColor = new Color(0.8f, 0.2f, 0f);
        else if (hpRatio <= 0.6f) newColor = new Color(0.8f, 0.6f, 0f);
        else if (hpRatio <= 0.8f) newColor = new Color(0.7f, 0.8f, 0f);
        else newColor = new Color(0, 0.8f, 0.1f);

        if (sprite != null) sprite.color = newColor;
        if (image != null) image.color = newColor;
    }
}
