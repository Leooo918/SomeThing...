using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class ShelterObject : MonoBehaviour
{
    ShelterPlayer.onMoveComplete onComplete;
    protected Renderer render = null;
    protected ShelterPlayer player = null;
    protected ShelterObejctInteract shelter = null;
    public bool isSelected = false;
    private Transform targetPos;
    public GameObject interactUI { get; private set; }

    protected virtual void Awake()
    {
        shelter = GetComponentInChildren<ShelterObejctInteract>();
        render = transform.Find("Sprite").GetComponent<Renderer>();
        targetPos = transform.Find("PlayerMovePos");
        interactUI = transform.Find("PressFImage").gameObject;
        interactUI.SetActive(false);
    }

    private void Start()
    {
        player = ShelterSceneManager.instance.player;
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButton(0) && isSelected == true)
        {
            onComplete = () => {
                interactUI.SetActive(true);
                shelter.isInteracting = true;
            };
            player.Move(targetPos.position, onComplete);
        }

        OnPointerExit();
    }

    public abstract void UseObject();
    public virtual void OnPointerExit()
    {
        render.material.SetFloat("_isDrawOutLine", 0);
        isSelected = false;
    }

    public virtual void OnPointerEnter()
    {
        render.material.SetFloat("_isDrawOutLine", 1);
        isSelected = true;
    }
}
