using DG.Tweening;

public class WalkAnimation : CharacterAnimation
{
    private Sequence seq;

    public override void Animation()
    {
        isAnimating = true;

        seq = DOTween.Sequence();

        float originPos = rect.anchoredPosition.y;

        seq.Append(rect.DOAnchorPosY(originPos + 100, 0.5f).SetEase(Ease.Linear))
            .Append(rect.DOAnchorPosY(originPos, 0.1f).SetEase(Ease.Linear))
            .Append(rect.DOAnchorPosY(originPos + 100, 0.5f).SetEase(Ease.Linear))
            .Append(rect.DOAnchorPosY(originPos, 0.1f).SetEase(Ease.Linear))
            .Append(rect.DOAnchorPosY(originPos + 100, 0.5f).SetEase(Ease.Linear))
            .Append(rect.DOAnchorPosY(originPos, 0.1f).SetEase(Ease.Linear))
            .OnComplete(() => 
            {
                isAnimating = false;
            });
    }
}
