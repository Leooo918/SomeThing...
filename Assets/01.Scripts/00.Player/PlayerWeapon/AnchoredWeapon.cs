using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchoredWeapon : MonoBehaviour
{
    private WeaponParents weaponParent = null;
    private Transform parent = null;
    private float angle = 0f;
    private Vector2 position = Vector2.zero;

    [SerializeField]private float clampValue = 1f;

    private void Awake()
    {
        weaponParent = GetComponentInParent<WeaponParents>();
        angle = transform.rotation.eulerAngles.z;
        position = transform.position;
    }

    private void Update()
    {
        float rot = transform.eulerAngles.z;
        float clampRot = Mathf.Clamp(rot, angle - clampValue, angle + clampValue);
        transform.eulerAngles = new Vector3(0, 0, clampRot);

        Vector2 pos = transform.position;
        Vector2 clampPos = new Vector2(Mathf.Clamp(pos.x, position.x-clampValue, position.x +clampValue), Mathf.Clamp(pos.y, position.y - clampValue, position.y + clampValue));
        transform.position = clampPos;
    }
}
