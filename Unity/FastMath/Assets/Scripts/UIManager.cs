using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SimpleJSON;
using System.Collections.Concurrent;
using EasyTransition;

public class UIManager : MonoBehaviour
{
    private bool monitorActive = false;
    private int currPosition = 0;
    [SerializeField] private TextMeshProUGUI questionText1;
    [SerializeField] private TextMeshProUGUI questionText2;
    [SerializeField] private TextMeshProUGUI alternative1;
    [SerializeField] private TextMeshProUGUI alternative2;
    [SerializeField] private TextMeshProUGUI alternative3;
    [SerializeField] private TextMeshProUGUI Player1Name;
    [SerializeField] private TextMeshProUGUI Player2Name;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private GameObject RightAnswerFb;
    [SerializeField] private GameObject WrongAnswerFb;
    [SerializeField] private GameObject LateAnswerFb;
    [SerializeField] private GameObject WinnerFb;
    [SerializeField] private GameObject DefeatFb;
    [SerializeField] private GameObject TugOfWar;


    string roundId;
    float seconds = 1.5f;
    float finalMatchSeconds = 3f;
    [SerializeField] private TransitionSettings transition;

    void Start()
    {
        SetPlayersName();
        //SetMatch();

        CheckQueue();

        button1.onClick.AddListener(() => HandleAlternativeClick(button1.GetComponentInChildren<TextMeshProUGUI>().text));
        button2.onClick.AddListener(() => HandleAlternativeClick(button2.GetComponentInChildren<TextMeshProUGUI>().text));
        button3.onClick.AddListener(() => HandleAlternativeClick(button3.GetComponentInChildren<TextMeshProUGUI>().text));
    }

    IEnumerator ShowRigthFB(float seconds)
    {
        RightAnswerFb.SetActive(true);

        yield return new WaitForSeconds(seconds);

        RightAnswerFb.SetActive(false);
    }
    IEnumerator ShowWrongFB(float seconds)
    {
        WrongAnswerFb.SetActive(true);

        yield return new WaitForSeconds(seconds);

        WrongAnswerFb.SetActive(false);
    }
    IEnumerator ShowLateFB(float seconds)
    {
        LateAnswerFb.SetActive(true);

        yield return new WaitForSeconds(seconds);

        LateAnswerFb.SetActive(false);
    }

    IEnumerator WinnerFB(float seconds)
    {
        WinnerFb.SetActive(true);

        yield return new WaitForSeconds(seconds);

        WinnerFb.SetActive(false);

        TransitionManager.Instance().Transition("Home", transition, 0);

    }
    IEnumerator DefeatFB(float seconds)
    {
        DefeatFb.SetActive(true);

        yield return new WaitForSeconds(seconds);

        DefeatFb.SetActive(false);

        TransitionManager.Instance().Transition("Home", transition, 0);

    }

    void SetPlayersName()
    {
        string name = PlayerPrefs.GetString("PlayerName");
        Player1Name.SetText(name);
    }

    void SetMatch()
    {
    }

    void Update()
    {
        if (!monitorActive)
        {
            monitorActive = tryGetMatchData();
        }
        CheckQueue();
    }

    void CheckQueue()
    {
        string queueData;

        while (MatchData.Instance.queuedLogs.TryDequeue(out queueData) && !string.IsNullOrEmpty(queueData))
        {
            tryGetMatchData();
        }
    }
    

    bool tryGetMatchData()
    {
        try
        {
            roundId = MatchData.Instance.roundId;
            Player2Name.SetText(MatchData.Instance.opponentName);
            questionText1.SetText(MatchData.Instance.questionOne);
            questionText2.SetText(MatchData.Instance.questionTwo);
            alternative1.SetText(MatchData.Instance.alternative1);
            alternative2.SetText(MatchData.Instance.alternative2);
            alternative3.SetText(MatchData.Instance.alternative3);

            if (MatchData.Instance.roundState == "right")
            {
                StartCoroutine(ShowRigthFB(seconds));
            }
            else if (MatchData.Instance.roundState == "wrong")
            {
                StartCoroutine(ShowWrongFB(seconds));
            }
            else if (MatchData.Instance.roundState == "late")
            {
                StartCoroutine(ShowLateFB(seconds));
            }
            MatchData.Instance.roundState = null;

            if (MatchData.Instance.gameEnded == "True")
            {
                if (MatchData.Instance.winner == "True")
                {
                    StartCoroutine(WinnerFB(finalMatchSeconds));
                }
                else
                {
                    StartCoroutine(DefeatFB(finalMatchSeconds));
                }

                MatchData.Instance.gameEnded = null;
            }
            if (MatchData.Instance.points != null && int.Parse(MatchData.Instance.points)!=currPosition)
            {
                var posX = int.Parse(MatchData.Instance.points);
                Vector3 pos = new Vector3((float)posX * 0.5f, TugOfWar.transform.position.y,TugOfWar.transform.position.z);
                currPosition = posX;
                TugOfWar.transform.position = pos;
            }

            if (!string.IsNullOrEmpty(roundId))
            {
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
        return false;
    }

    private void OnDestroy()
    {
        Destroy(MatchData.Instance);
    }

    void HandleAlternativeClick(string value)
    {
        string message = @$"{{""Type"":""guessed"",""Message"":{{""RoundId"":""{roundId}"",""Guess"":{value}}}}}";
        WsSingleton.Instance.Send(message);
    }
}
