using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterAnimation : MonoBehaviour
{
    protected Image image;
    protected RectTransform rect;

    public bool isAnimatingDuringReading = true;
    public bool isAnimating { get; protected set; }

    private void OnEnable()
    {
        isAnimating = false;
    }

    public abstract void Animation();

    public virtual void Init(Image image)
    {
        this.image = image;
        rect = image.GetComponent<RectTransform>();
    }
}
