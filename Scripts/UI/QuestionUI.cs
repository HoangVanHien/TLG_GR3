using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestionUI : MonoBehaviour
{
    public TMP_Text question;
    public List<TMP_Text> answers;

    private int rightAnswer;
    
    public void SetUp(LibraryQuestion question)
    {
        this.question.text = question.question;
        for (int i = 0; i < answers.Count; i++)
        {
            answers[i].text = question.answers[i];
        }
        rightAnswer = 0;
    }

    public void Answer(int answer)
    {
        //GameManager.instance.matchManager.isAnswerYet = true;
        //GameManager.instance.matchManager.isAnswerRight = (answer == rightAnswer);
    }
}
