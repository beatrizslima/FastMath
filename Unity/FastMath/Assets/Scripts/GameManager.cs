using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uIManager;

    [SerializeField] private List<Question> questions;
    private Question randomized;

    void Start()
    {
        //RandomQuestion();
    }

    // void RandomQuestion(){
    //     int random = Random.Range(0, questions.Count);
    //     randomized = questions[random];
    //     uIManager.SetQuestion(randomized);
    // }

    // public bool SelectedAnswer(string answered){
    //     bool isCorrect = false;
    // }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
    public class Question{
        public string questionInfo;
        public List<string> alternatives;
        public string correctAnswer;
    }
