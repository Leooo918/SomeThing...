using UnityEngine;

public enum Sound
{
    ButtonClilckSound = 0,
    ErrorSound = 1,
    CursurMoveSound = 2
}


public class UISoundManager : MonoBehaviour
{
    private AudioSource audioSource = null;
    public static UISoundManager instance = null;

    public AudioClip[] uiSounds;

    private void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayOneShot(Sound.ErrorSound);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            PlayOneShot(Sound.ButtonClilckSound);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayOneShot(Sound.CursurMoveSound);
        }
    }

    public void PlayOneShot(Sound soundType)
    {
        switch (soundType)
        {
            case Sound.ButtonClilckSound:
                audioSource.pitch = 0.85f;
                break;
            case Sound.ErrorSound:
                audioSource.pitch = 0.98f;
                break;
            case Sound.CursurMoveSound:
                audioSource.pitch = 0.85f;
                break;
        }
        audioSource.PlayOneShot(uiSounds[(int)soundType]);
    }

    public void Play(Sound soundType)
    {
        switch (soundType)
        {
            case Sound.ButtonClilckSound:
                audioSource.pitch = 0.85f;
                break;
            case Sound.ErrorSound:
                audioSource.pitch = 0.98f;
                break;
            case Sound.CursurMoveSound:
                audioSource.pitch = 0.85f;
                break;
        }
        audioSource.clip = uiSounds[(int)soundType];

        audioSource.Play();
    }

}
