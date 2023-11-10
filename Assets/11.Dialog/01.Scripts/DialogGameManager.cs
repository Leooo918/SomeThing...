using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogGameManager : MonoBehaviour
{
    public static DialogGameManager instance = null;
    private PlayerDialogStatus playerStatsus = null;
    [SerializeField] private Dialog dialog = null;
    private RectTransform dialogBackground = null;
    [SerializeField] private List<DialogSO> dialogSO = new List<DialogSO>();
    private int readingDialogNum = 0;

    public PlayerDialogStatus Status => playerStatsus;


    private void Awake()
    {
        instance = this;
        playerStatsus = GetComponent<PlayerDialogStatus>();
        dialogBackground = GameObject.Find("DialogBackground").GetComponent<RectTransform>();
        playerStatsus.AutoLoad();

        Button skipBtn = dialogBackground.Find("SkipBtn").GetComponent<Button>();
        skipBtn.onClick.AddListener(EndDialog);
    }

    private void OnEnable()
    {
        dialogBackground.gameObject.SetActive(false);

        if(playerStatsus.curProgress == 0)
        {
            StartDialog(0);
        }
    }


    public void StartDialog(int dialogNum)
    {
        readingDialogNum = playerStatsus.curProgress;
        dialogBackground.gameObject.SetActive(true);
        dialog.Init(dialogSO[dialogNum], dialogBackground);
        dialog.StartReadDialog();
    }

    public void EndDialog()
    {
        if (readingDialogNum == playerStatsus.curProgress)
        {
            playerStatsus.curProgress++;
        }

        playerStatsus.AutoSave();
        dialogBackground.gameObject.SetActive(false);
    }
}
