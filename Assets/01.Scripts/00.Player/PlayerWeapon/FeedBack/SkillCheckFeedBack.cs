using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillCheckResult
{
    BigSuccess = 0,
    Success = 1,
    Fail = 2
}

public class SkillCheckFeedBack : MonoBehaviour
{
    private Transform skillCheck = null;
    private RectTransform successRange = null;
    private RectTransform bigSuccessRange = null;
    private RectTransform arrow = null;
    private float successRangeRot = 0;
    private float arrowRot = 0;
    public float skillCheckSpeed = 2;

    private bool end = false;

    private void Start()
    {
        Init(UIManager.instance.SkillCheck);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartSkillCheck();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print(EndSkillCheck().ToString());
        }
    }


    public void StartSkillCheck()
    {
        end = false;
        StartCoroutine("StartSkillCheckRoutine");
    }

    public SkillCheckResult EndSkillCheck()
    {
        end = true;
        arrowRot = arrow.rotation.eulerAngles.z;
        if (arrow.rotation.eulerAngles.z > 100)
        {
            arrowRot -= 360;
        }

        if (arrowRot - 45 >= successRangeRot - 8 && arrowRot - 45 <= successRangeRot + 8)
        {
            return SkillCheckResult.BigSuccess;
        }
        else if (arrowRot - 45 >= successRangeRot - 48 && arrowRot - 45 <= successRangeRot + 48)
        {
            return SkillCheckResult.Success;
        }
        else
        {
            return SkillCheckResult.Fail;
        }
    }

    IEnumerator StartSkillCheckRoutine()
    {
        skillCheck.gameObject.SetActive(true);
        successRangeRot = Random.Range(-90, 0);
        successRange.eulerAngles = new Vector3(0, 0, successRangeRot);
        arrow.eulerAngles = new Vector3(0, 0, 90);
        print(successRangeRot);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("ArrowRoutine");
    }

    IEnumerator ArrowRoutine()
    {
        while (true)
        {
            float rot = arrow.eulerAngles.z;

            if (end)
            {
                break;
            }

            rot -= skillCheckSpeed;
            if (rot <= -270f) rot = 90f;


            arrow.eulerAngles = new Vector3(0, 0, rot);

            yield return null;
        }
        skillCheck.gameObject.SetActive(false);
    }

    public void Init(Transform skillCheck)
    {
        this.skillCheck = skillCheck;
        successRange = skillCheck.Find("SuccessRange").GetComponent<RectTransform>();
        arrow = skillCheck.Find("Arrow").GetComponent<RectTransform>();
        bigSuccessRange = successRange.Find("BigSuccessRange").GetComponent<RectTransform>();
    }
}
