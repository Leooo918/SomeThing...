using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyWarning : MonoBehaviour
{
    private Transform fill;
    private Vector2 originPos;

    private Tween tween;

    private void Awake()
    {
        fill = transform.Find("Fill");
        originPos = new Vector2(0, fill.localPosition.y);
    }

    public void StartWarning(float time)
    {
        tween = fill.DOLocalMoveY(0, time)
             .OnComplete(() =>
             {
                 fill.localPosition = originPos;
                 gameObject.SetActive(false);
                 //�� ���� �� Destroy�ϴ°ɷ� �ص� �ǰ��
             })
             .SetAutoKill(true);
    }

    public void StopWarning()
    {
        tween.Kill();
        fill.localPosition = originPos;
        gameObject.SetActive(false);
    }
}
