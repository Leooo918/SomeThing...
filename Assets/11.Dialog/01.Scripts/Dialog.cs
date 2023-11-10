using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialog : MonoBehaviour
{
    private DialogSO currentDialog;         //���� ���̾�α�
    private ScriptSO currentScript;         //���� ��ũ��Ʈ
    private PlayerDialogStatus playerStatus;      //�÷��̾��� ����(���൵, 2ĳ���Ϳ� ���� ȣ����)
    private Transform canvas;

    private RectTransform dialogBackground;
    private TextMeshProUGUI dialogTxt;
    private TextMeshProUGUI peopleName;
    private Image peopleImage;
    private Image backgroundImage;
    private Transform bottomBar;
    private Button skipBtn;

    private string curReadingLine;

    private bool isReadingTxt;

    private void Awake()
    {
        playerStatus = GameObject.Find("ManageObject").GetComponent<PlayerDialogStatus>();
        canvas = GameObject.Find("Canvas").transform;
    }

    public void Update()
    {
        if (isReadingTxt == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SkipLine();
            }

            dialogTxt.SetText(curReadingLine);
        }
    }

    public void StartReadDialog()
    {
        currentScript = currentDialog.scripts[0];   //�����ϸ� ù��° ��ũ��Ʈ���� ����

        StartCoroutine("ReadingLine");
    }

    private void SkipLine()
    {
        StartCoroutine("SkipRoutine");
    }
    IEnumerator SkipRoutine()
    {
        curReadingLine = currentScript.talkingDetails;
        StopCoroutine("ReadingLine");
        yield return new WaitForSeconds(0.1f) ;
        StartCoroutine("WaitNextScript");
    }

    IEnumerator ReadingLine()
    {
        if (currentScript.scriptType == ScriptType.IMAGE)
        {
            bottomBar.gameObject.SetActive(false);
            backgroundImage.sprite = currentScript.background;      //��� ����ְ�
            if (currentScript.image != null)
            {
                GameObject image = Instantiate(currentScript.image, canvas.Find("DialogBackground"));
                image.transform.SetSiblingIndex(1);
                peopleImage = image.GetComponent<Image>();

                if (currentScript.anim != null)                         //�ִϸ��̼��� �ִٸ�
                {
                    CharacterAnimation anim = currentScript.anim;       //�����ض�
                    anim.Init(image.GetComponent<Image>());
                    anim.Animation();

                    if (anim.isAnimatingDuringReading == false)
                    {
                        yield return new WaitUntil(() => anim.isAnimating == false);
                    }
                }
            }

            yield return new WaitForSeconds(0.3f);
            StartCoroutine("WaitNextScript");
            yield break;
        }
        else
        {
            bottomBar.gameObject.SetActive(true);
        }


        isReadingTxt = true;                                    //���� ��ũ��Ʈ�� �а� ����

        peopleName.SetText(currentScript.talkingPeopleName);    //�а��ִ� ��� �����ְ�
        backgroundImage.sprite = currentScript.background;      //��� ����ְ�

        if (currentScript.image != null)
        {
            GameObject img = Instantiate(currentScript.image, canvas.Find("DialogBackground")); //ĳ���ʹ� ����� �ٸ��ϱ� ���������� �̾��ְ�
            img.transform.SetSiblingIndex(1);                       //��溸�� �տ� ��ȭâ���� �ڿ� �ְ�
            peopleImage = img.GetComponent<Image>();                //�̹��� �������ְ�

            if (currentScript.anim != null)                         //�ִϸ��̼��� �ִٸ�
            {
                CharacterAnimation anim = currentScript.anim;       //�����ض�
                anim.Init(img.GetComponent<Image>());
                anim.Animation();

                yield return new WaitUntil(() => anim.isAnimating == false);
            }
        }



        curReadingLine = "";                                    //��ȭ �ʱ�ȭ

        for (int i = 0; i < currentScript.talkingDetails.Length; i++)
        {
            curReadingLine += currentScript.talkingDetails[i];  //�ѱ��ھ� �а���
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine("WaitNextScript");
    }

    IEnumerator WaitNextScript()
    {
        isReadingTxt = false;

        switch (currentScript.scriptType)
        {
            case ScriptType.NORMAL:
                NormalScriptSO normal = currentScript as NormalScriptSO;
                currentScript = normal.nextSO;

                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                if (currentScript == null) break;

                backgroundImage.sprite = null;

                break;
            case ScriptType.OPTION:
                OptionSO option = currentScript as OptionSO;
                bool isOptionSelected = false;

                for (int i = 0; i < option.options.Count; i++)
                {
                    Button optionBtn = Instantiate(option.optionPf, dialogBackground.Find("Options")).GetComponent<Button>();
                    optionBtn.transform.Find("Text").GetComponent<TextMeshProUGUI>().SetText(option.options[i]);

                    int index = i;

                    optionBtn.onClick.AddListener(() =>
                    {
                        for (int i = 0; i < dialogBackground.Find("Options").childCount; i++)
                        {
                            Destroy(dialogBackground.Find("Options").GetChild(i).gameObject);
                        }
                        currentScript = option.nextScriptsByOption[index];
                        isOptionSelected = true;

                        backgroundImage.sprite = null;
                    });
                }

                yield return new WaitUntil(() => isOptionSelected == true);
                break;
            case ScriptType.BRANCH:
                BranchSO branch = currentScript as BranchSO;

                if (branch.condition.Judge())
                {
                    currentScript = branch.nextScriptOnTrue;
                }
                else
                {
                    currentScript = branch.nextScriptOnFalse;
                }
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                break;
            case ScriptType.IMAGE:
                ImageSO image = currentScript as ImageSO;

                currentScript = image.nextScript;
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                break;
        }

        if (peopleImage != null) Destroy(peopleImage.gameObject);
        if (currentScript == null)
        {
            dialogBackground.gameObject.SetActive(false);

            DialogGameManager.instance.EndDialog();
            yield break;
        }

        playerStatus.LikeablilityUp(currentScript.charactor, currentScript.changeLikeyValue);
        //ȣ������ �������� �� �׷� �� ������ �� �ƴ϶� �׳� �ƹ� ��ȭ�� ���� �� �� ��ȭ�� ������ ���� ��ŭ ������ ��

        StartCoroutine("ReadingLine");
    }

    public void Init(DialogSO dialog, RectTransform dialogBackground)
    {
        currentDialog = dialog;

        this.dialogBackground = dialogBackground;
        bottomBar = dialogBackground.Find("BottomBar");
        dialogTxt = dialogBackground.Find("BottomBar/Dialog").GetComponent<TextMeshProUGUI>();
        peopleName = dialogBackground.Find("BottomBar/NameTxt").GetComponent<TextMeshProUGUI>();
        peopleImage = dialogBackground.Find("").GetComponent<Image>();
        backgroundImage = dialogBackground.Find("Background").GetComponent<Image>();
        skipBtn = dialogBackground.Find("SkipBtn").GetComponent<Button>();

        dialogBackground.gameObject.SetActive(true);
    }
}
