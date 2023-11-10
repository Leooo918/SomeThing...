using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLantern : MonoBehaviour
{
    private Light2D lantern = null;
    private PlayerStatus status = null;

    private float durabilityDownCool = 5f;
    private float durabilityDownCoolDown = 0f;


    private void Awake()
    {
        lantern = GetComponent<Light2D>();
        status = GetComponentInParent<PlayerStatus>();
    }

    private void Update()
    {
        if (status.LanternSlot.assignedItem != null)
        {
            lantern.intensity = 1.5f;
            Lantern l = status.LanternSlot.assignedItem as Lantern;

            durabilityDownCoolDown -= Time.deltaTime;
            if(durabilityDownCoolDown <= 0)
            {
                durabilityDownCoolDown = durabilityDownCool;
                l.DurabilityDown(0.1f);
            }
        }
        else
        {
            lantern.intensity = 0.2f;
        }
    }

}
