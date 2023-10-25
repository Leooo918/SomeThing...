using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelect : MonoBehaviour
{
    private Camera mainCam = null;
    [SerializeField] private float clampingValue = 15f;
    [SerializeField] private int slotNum = 0;
    [SerializeField] private float weaponMaxSwitchingTime = 3f;
    private float selectedRot = 0f;

    private bool isSelectEnd = false;

    private WeaponSelector weaponSelector = null;

    private void Awake()
    {
        mainCam = Camera.main;
        weaponSelector = GetComponentInParent<WeaponSelector>();
    }

    private void OnEnable()
    {
        isSelectEnd = false;
        StopCoroutine("WeaponSelectingTimeLimit");
        StartCoroutine("WeaponSelectingTimeLimit");
    }

    private void Update()
    {
        if (isSelectEnd == true) return;

        Vector3 v = mainCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float targetAngle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;

        if (Mathf.Abs(targetAngle % 90) < clampingValue || Mathf.Abs(targetAngle % 90) > 90 - clampingValue)
        {
            float rot = Mathf.RoundToInt(targetAngle / 90) * 90;
            targetAngle = rot;
            selectedRot = rot;
        }

        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.realtimeSinceStartup * 10));

        if (Input.GetMouseButtonUp(0))
        {
            isSelectEnd = true;
            StartCoroutine("WeaopnSelectRoutine");
        }
    }

    IEnumerator WeaopnSelectRoutine()
    {
        Time.timeScale = 1f;
        while (Mathf.Abs(Mathf.Abs(transform.eulerAngles.z) - Mathf.Abs(selectedRot)) > 0.1f && Mathf.Abs(Mathf.Abs(transform.eulerAngles.z) - Mathf.Abs(selectedRot)) < 179.9f)
        {
            print(".");
            print(Mathf.Abs(Mathf.Abs(transform.eulerAngles.z) - Mathf.Abs(selectedRot)));
            transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, selectedRot, Time.deltaTime * 50));
            yield return null;
        }

        int rot = Mathf.RoundToInt(transform.eulerAngles.z);

        if (rot == 0)
        {
            slotNum = 2;
        }
        else if (rot == 90)
        {
            slotNum = 1;
        }
        else if (rot == 180 || rot == -180)
        {
            slotNum = 0;
        }


        weaponSelector.WeaponSelectEnd(slotNum);
    }

    IEnumerator WeaponSelectingTimeLimit()
    {
        print("¾ßÈ£");
        yield return new WaitForSecondsRealtime(weaponMaxSwitchingTime);
        Time.timeScale = 1f;
        weaponSelector.WeaponSelectEnd(slotNum);
    }
}
