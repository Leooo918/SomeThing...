using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance = null;

    private CinemachineVirtualCamera vCam = null;
    private CinemachineBasicMultiChannelPerlin bperlin = null;
    private CinemachineConfiner2D confiner = null;

    private Volume globalVolume = null;
    private VolumeProfile voluemeProfile = null;
    private Vignette vignette = null;

    private bool isHurt = false;
    private bool isIntencityDown = false;
    private float vignetteIntencity = 0f;

    private void Update()
    {

        if (isHurt == true)
        {
            MediateVignetteIntencity();

            vignette.intensity.value = vignetteIntencity;

        }
    }

    private void MediateVignetteIntencity()
    {
        if (isIntencityDown == true)
        {
            vignetteIntencity -= Time.deltaTime * 0.6f;
        }
        else
        {
            vignetteIntencity += Time.deltaTime * 0.4f;
        }

        if (vignetteIntencity >= 0.6f) isIntencityDown = true;
        else if (vignetteIntencity <= 0.25f) isIntencityDown = false;
    }

    public void ScreenHurt(bool isHurt)
    {
        this.isHurt = isHurt;
    }

    public void SetCamBound(PolygonCollider2D coll)
    {
        confiner.m_BoundingShape2D = coll;
    }

    public void ShakeCam(float amplitude, float frequency, float time)
    {
        StartCoroutine(CamShakeRoutine(amplitude, frequency, time));
    }

    IEnumerator CamShakeRoutine(float amplitude, float frequency, float time)
    {
        bperlin.m_AmplitudeGain = amplitude;
        bperlin.m_FrequencyGain = frequency;
        yield return new WaitForSeconds(time);
        bperlin.m_AmplitudeGain = 0;
        bperlin.m_FrequencyGain = 0;
    }


    public void Init()
    {
        vCam = transform.Find("PlayerCam").GetComponent<CinemachineVirtualCamera>();

        bperlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        confiner = vCam.GetComponent<CinemachineConfiner2D>();

        globalVolume = transform.Find("Global Volume").GetComponent<Volume>();
        voluemeProfile = globalVolume.profile;

        voluemeProfile.TryGet<Vignette>(out vignette);
    }
}
