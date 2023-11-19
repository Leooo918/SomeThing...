using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ShelterPortalObject : ShelterObject
{
    private Transform dungeonCheckUI;
    private Image fadeImg;
    
    protected override void Awake()
    {
        base.Awake();

        dungeonCheckUI = GameObject.Find("CheckGoInPortal").transform;
        fadeImg = GameObject.Find("FadeOut").GetComponent<Image>();

        dungeonCheckUI.Find("Yes").GetComponent<Button>().onClick.AddListener(() =>
        {
            fadeImg.gameObject.SetActive(true);
            fadeImg.DOFade(1, 1f).OnComplete(() => SceneManager.LoadScene(2));
        });
        dungeonCheckUI.Find("No").GetComponent<Button>().onClick.AddListener(() =>
        {
            dungeonCheckUI.DOScale(0, 0.5f);
        });

        dungeonCheckUI.localScale = Vector3.zero;
    }

    public override void UseObject()
    {
        dungeonCheckUI.DOScale(1, 0.5f);
    }
}
