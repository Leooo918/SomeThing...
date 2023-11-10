using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialog : MonoBehaviour
{
    private DialogSO currentDialog;         //현재 다이어로그
    private ScriptSO currentScript;         //현재 스크립트
    private PlayerDialogStatus playerStatus;      //플레이어의 상태(진행도, 2캐릭터에 대한 호감도)
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
        currentScript = currentDialog.scripts[0];   //시작하면 첫번째 스크립트부터 실행

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
            backgroundImage.sprite = currentScript.background;      //배경 깔아주고
            if (currentScript.image != null)
            {
                GameObject image = Instantiate(currentScript.image, canvas.Find("DialogBackground"));
                image.transform.SetSiblingIndex(1);
                peopleImage = image.GetComponent<Image>();

                if (currentScript.anim != null)                         //애니메이션이 있다면
                {
                    CharacterAnimation anim = currentScript.anim;       //실행해라
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


        isReadingTxt = true;                                    //현재 스크립트를 읽고 있음

        peopleName.SetText(currentScript.talkingPeopleName);    //읽고있는 사람 적어주고
        backgroundImage.sprite = currentScript.background;      //배경 깔아주고

        if (currentScript.image != null)
        {
            GameObject img = Instantiate(currentScript.image, canvas.Find("DialogBackground")); //캐릭터는 사이즈가 다르니까 프리펩으로 뽑아주고
            img.transform.SetSiblingIndex(1);                       //배경보단 앞에 대화창보단 뒤에 있게
            peopleImage = img.GetComponent<Image>();                //이미지 지정해주고

            if (currentScript.anim != null)                         //애니메이션이 있다면
            {
                CharacterAnimation anim = currentScript.anim;       //실행해라
                anim.Init(img.GetComponent<Image>());
                anim.Animation();

                yield return new WaitUntil(() => anim.isAnimating == false);
            }
        }



        curReadingLine = "";                                    //대화 초기화

        for (int i = 0; i < currentScript.talkingDetails.Length; i++)
        {
            curReadingLine += currentScript.talkingDetails[i];  //한글자씩 읽게해
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
        //호감도는 선택했을 때 그럴 때 오르는 게 아니라 그냥 아무 대화에 들어갔을 때 그 대화에 지정해 놓은 만큼 오르게 함

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
