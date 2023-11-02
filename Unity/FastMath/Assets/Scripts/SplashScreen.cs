using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Text.RegularExpressions;
using EasyTransition;


public class SplashScreen : MonoBehaviour
{
    [SerializeField] private TransitionSettings transition;
    float time, second;
    bool isLoading = true;
    bool isTransitioning = false;
    [SerializeField] private Button Home;
    [SerializeField] private Button Yes;
    [SerializeField] private Button No;
    [SerializeField] private GameObject PopUpGoHome;
    [SerializeField] private Image FillLoading;
    [SerializeField] private Button Config;
    [SerializeField] private Button CloseConfigScreen;
    [SerializeField] private GameObject ConfigScreen;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.ClockTicking();
        isTransitioning = false;
        Home.onClick.AddListener(PopUpGoToHome);
        Yes.onClick.AddListener(GoHome);
        No.onClick.AddListener(ClosePopUp);
        Config.onClick.AddListener(OpenConfig);
        CloseConfigScreen.onClick.AddListener(CloseConfig);
    }

     void PopUpGoToHome(){
        SoundManager.Instance.Click();
        PopUpGoHome.SetActive(true);
    }

    void GoHome(){
        SoundManager.Instance.Click();
        TransitionManager.Instance().Transition("Home", transition, 0);
        WsSingleton.Instance.Close();
    }

    void ClosePopUp(){
        SoundManager.Instance.Click();
        PopUpGoHome.SetActive(false);
    }

    public void LoadGame()
    {
        SoundManager.Instance.GameFound();
        TransitionManager.Instance().Transition("Game", transition, 0);
    }
    void OpenConfig()
    {
        SoundManager.Instance.Click();
        ConfigScreen.SetActive(true);
    }
    void CloseConfig()
    {
        SoundManager.Instance.Click();
        ConfigScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isLoading)
        {
            time += Time.deltaTime;
            FillLoading.fillAmount = (float)Math.Abs(Math.Cos(time));
            isLoading = string.IsNullOrEmpty(MatchData.Instance.roundId);
        }
        else
        {
            if (!isTransitioning)
            {
                SoundManager.Instance.PauseClockTicking();
                SoundManager.Instance.GameFound();
                LoadGame();
                isTransitioning = true;
            }
        }
    }
}
