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
    [SerializeField] private Image FillLoading;
    // Start is called before the first frame update
    void Start()
    {
        isTransitioning = false;
    }

    public void LoadGame()
    {
        TransitionManager.Instance().Transition("Game", transition, 0);
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
                LoadGame();
                isTransitioning = true;
            }
        }
    }
}
