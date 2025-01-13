using UnityEngine;
using UnityEngine.UI;

public abstract class UIFlyingIcon : MonoBehaviour
{
    internal RectTransform rectTransform;
    public Image Icon;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
    }

    public void AssignSprite(Sprite sprite)
    {
        if (sprite == null) return;
        Icon.sprite = sprite;
        if (sprite.rect.size.x > sprite.rect.size.y)
        {
            Icon.rectTransform.sizeDelta = new Vector2(Icon.rectTransform.sizeDelta.x,
                                                  Icon.rectTransform.sizeDelta.y * sprite.rect.size.y / sprite.rect.size.x);
        }
        else
        {
            Icon.rectTransform.sizeDelta = new Vector2(Icon.rectTransform.sizeDelta.x * sprite.rect.size.x / sprite.rect.size.y,
                                              Icon.rectTransform.sizeDelta.y);
        }
    }

    virtual public void ResetAndReturn()
    {
        float maxDimension = Mathf.Max(Icon.rectTransform.sizeDelta.x, Icon.rectTransform.sizeDelta.y);
        Icon.rectTransform.sizeDelta = new Vector3(maxDimension, maxDimension, 1);
        rectTransform.anchoredPosition = Vector2.zero;
        Icon.rectTransform.anchoredPosition = Vector2.zero;
        Icon.rectTransform.localScale = Vector3.one;

        ObjectPooler.Return(gameObject);
    }
}
