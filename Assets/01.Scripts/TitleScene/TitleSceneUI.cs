using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TitleSceneUI : MonoBehaviour
{
    private UIDocument UIDoc = null;
    private VisualElement root = null;

    private Button startBtn = null;
    private Button optionBtn = null;
    private Button exitBtn = null;

    private void Awake()
    {
        UIDoc = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        root = UIDoc.rootVisualElement;

        startBtn = root.Q<Button>("start-btn");
        optionBtn = root.Q<Button>("option-btn");
        exitBtn = root.Q<Button>("exit-btn");

        startBtn.RegisterCallback<ClickEvent>(OnStart);
        optionBtn.RegisterCallback<ClickEvent>(OnOpenOption);
        exitBtn.RegisterCallback<ClickEvent>(OnExit);
    }

    private void OnStart(ClickEvent evt)
    {
        SceneManager.LoadScene(1);
    }

    private void OnOpenOption(ClickEvent evt)
    {

    }

    private void OnExit(ClickEvent evt)
    {
        Application.Quit();
    }

}
