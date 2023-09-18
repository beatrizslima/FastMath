using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using EasyTransition;

public class MainMenu : Singleton<MainMenu>
{
    [SerializeField] private TMP_InputField InputFieldName;
    [SerializeField] private TransitionSettings transition;
    [SerializeField] private Button Play;
    [SerializeField] private Button Info;
    [SerializeField] private GameObject InfoBoard;
    [SerializeField] private Button CloseInfoBoard;
    void Start()
    {
        Debug.Log(PlayerPrefs.GetString("PlayerName"));
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            string playerName = PlayerPrefs.GetString("PlayerName");
            ShowPlayerName(playerName);
        }
        Play.onClick.AddListener(SaveInfos);
        Info.onClick.AddListener(OpenInfoBoard);
        CloseInfoBoard.onClick.AddListener(CloseBoard);
    }

    void ShowPlayerName(string name)
    {
        InputFieldName.text = name;
    }

    void SaveInfos()
    {
        WsSingleton.Instance.Connect(InputFieldName.text);
        SavePlayerNameAndGoToPlay(InputFieldName.text);
    }
    void SavePlayerNameAndGoToPlay(string name)
    {
        PlayerPrefs.SetString("PlayerName", name);

        TransitionManager.Instance().Transition("Splash", transition, 0);

        //SceneManager.LoadScene("Splash");
    }


    void OpenInfoBoard()
    {
        InfoBoard.SetActive(true);
    }
    void CloseBoard()
    {
        InfoBoard.SetActive(false);
    }

    void Update()
    {

    }
}