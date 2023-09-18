using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class MatchData : MonoBehaviour
{
    public string roundId { get; set; }
    public string opponentName { get; set; }
    public string questionOne { get; set; }
    public string questionTwo { get; set; }
    public string alternative1 { get; set; }
    public string alternative2 { get; set; }
    public string alternative3 { get; set; }
    public string gameEnded { get; set; }
    public string winner { get; set; }
    public string roundState { get; set; }
    public string points { get; set; }
    public ConcurrentQueue<string> queuedLogs { get; set; }

    private static MatchData instance;
    public static MatchData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MatchData>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("MatchData");
                    instance = obj.AddComponent<MatchData>();
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (queuedLogs == null)
        {
            queuedLogs = new ConcurrentQueue<string>();
        }
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        WsSingleton.Instance.OnMessageReceived += HandleWebSocketMessage;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleWebSocketMessage(string data)
    {
        if (data == "Match made")
        {
            UnityEngine.Debug.Log("Starting match");
            return;
        }
        var message = JSONNode.Parse(data);


        switch (message["Type"].Value)
        {
            case "beggining":
                try
                {
                    opponentName = message["Message"]["OpponentName"].Value;
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e.Message);
                    throw;
                }
                break;
            case "starting":

                try
                {
                    roundId = message["Message"]["RoundData"]["RoundId"].Value;

                    questionTwo = message["Message"]["RoundData"]["Value2"].Value;
                    questionOne = message["Message"]["RoundData"]["Value1"].Value;
                    alternative1 = message["Message"]["RoundData"]["Alternatives"][0].Value;
                    alternative2 = message["Message"]["RoundData"]["Alternatives"][1].Value;
                    alternative3 = message["Message"]["RoundData"]["Alternatives"][2].Value;
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e.Message);
                    throw;
                }
                break;
            case "guessed":
                try
                {
                    roundId = message["Message"]["RoundId"].Value;
                    roundState = message["Message"]["Answer"].Value;
                    points = message["Message"]["Points"].Value;
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e.Message);
                    throw;
                }
                break;
            case "end":
                try
                {
                    points = message["Message"]["YourPoints"].Value;
                    gameEnded = message["Message"]["GameEnded"].Value;
                    winner = message["Message"]["Winner"].Value;
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e.Message);
                    throw;
                }
                break;

        }
        queuedLogs.Enqueue("matchData");
    }
}
