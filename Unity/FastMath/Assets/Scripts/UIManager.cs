using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SimpleJSON;
using System.Collections.Concurrent;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
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
    string roundId;

    private Question question;
    private bool answered;

    ConcurrentQueue<MatchData> queuedLogs = new ConcurrentQueue<MatchData>();

    void Start()
    {
        SetPlayersName();
        WsSingleton.Instance.OnMessageReceived += HandleWebSocketMessage;
        
        button1.onClick.AddListener(()=>HandleAlternativeClick(button1.GetComponentInChildren<TextMeshProUGUI>().text));
        button2.onClick.AddListener(()=>HandleAlternativeClick(button2.GetComponentInChildren<TextMeshProUGUI>().text));
        button3.onClick.AddListener(()=>HandleAlternativeClick(button3.GetComponentInChildren<TextMeshProUGUI>().text));
    }

    struct MatchData {
        public string opponentName; 
        public string questionOne; 
        public string questionTwo; 
        public string alternative1; 
        public string alternative2; 
        public string alternative3; 
        public string gameEnded; 
        public string winner;
    }

    private void HandleWebSocketMessage(string data)
    {
        if (data == "Match made")
        {
            Debug.Log("Starting match");
            return;
        }
        var message = JSONNode.Parse(data);
        Debug.Log(message["Type"]);

        switch (message["Type"].Value)
        {
            case "starting":

                try
                {                        
                    roundId = message["Message"]["RoundData"]["RoundId"].Value;

                    var name = new MatchData { 
                        opponentName = message["Message"]["OpponentName"].Value, 
                        questionOne = message["Message"]["RoundData"]["Value1"].Value, 
                        questionTwo = message["Message"]["RoundData"]["Value2"].Value, 
                        alternative1 = message["Message"]["RoundData"]["Alternatives"][0].Value, 
                        alternative2 = message["Message"]["RoundData"]["Alternatives"][1].Value,
                        alternative3 = message["Message"]["RoundData"]["Alternatives"][2].Value, 
                        gameEnded = message["Message"]["RoundData"]["GameEnded"].Value,
                        winner = message["Message"]["RoundData"]["Winner"].Value
                    };
                    queuedLogs.Enqueue(name);
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                    throw;
                }
            break;
            default:
                Debug.Log(message["Type"].Value);
                Debug.Log(message["Message"].Value);
                break;

        }
    }

    void SetPlayersName()
    {
        string name = PlayerPrefs.GetString("PlayerName");
        Player1Name.SetText(name);
    }

    // public void SetQuestion(Question question){
    //     // gets the randomized question and its alternatives and sets the text
    //     this.question = question;
    //     questionText.text = question.questionInfo;
    //     List<string> answerList = ShuffleList.ShuffleListItems<string>(question.alternatives);   

    //     for (int i = 0; i < alternatives.Count; i++)
    //     {
    //         alternatives[i].GetComponentInChildren<TextMeshProUGUI>().text = answerList[i];
    //         alternatives[i].name = answerList[i];
    //     }

    //     answered = false;
    // }
    // Update is called once per frame
    void Update()
    {
        MatchData data;
        while (queuedLogs.TryDequeue(out data))
        {
            try
            {
                Player2Name.SetText(data.opponentName);
                questionText1.SetText(data.questionOne);
                questionText2.SetText(data.questionTwo);
                alternative1.SetText(data.alternative1);
                alternative2.SetText(data.alternative2);
                alternative3.SetText(data.alternative3);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }
        }
    }

    private void OnDestroy()
    {
        WsSingleton.Instance.OnMessageReceived -= HandleWebSocketMessage;
    }

    void HandleAlternativeClick(string value){
        string message = @$"{{""Type"":""guessed"",""Message"":{{""RoundId"":""{roundId}"",""Guess"":{value}}}}}";
        WsSingleton.Instance.Send(message);
    }
}
