using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private List<Button> alternatives;

    private Question question;
    private bool answered;
    void Start()
    {
  
    }

    public void SetQuestion(Question question){
        // gets the randomized question and its alternatives and sets the text
        this.question = question;
        questionText.text = question.questionInfo;
        List<string> answerList = ShuffleList.ShuffleListItems<string>(question.alternatives);   

        for (int i = 0; i < alternatives.Count; i++)
        {
            alternatives[i].GetComponentInChildren<TextMeshProUGUI>().text = answerList[i];
            alternatives[i].name = answerList[i];
        }

        answered = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
