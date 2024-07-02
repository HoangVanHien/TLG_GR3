using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LibraryQuestionUI : MonoBehaviour
{

    public Button questionPick;
    public TMP_Text questionNameText;
    public Button delButton;

    public string libraryID;
    public int questionID;
    private string questionName;
    private bool isChange = false;
    private UnityAction<string> onClickLib;
    private UnityAction<string> onDelLib;
    private UnityAction<int> onClickQues;
    private UnityAction<int> onDelQues;
    private bool isLibrary = true;

    private void Start()
    {
        questionPick.onClick.AddListener(QuestionClick);
        delButton.onClick.AddListener(OnDelButtonClick);
    }

    private void Update()
    {
        if (isChange)
        {
            questionNameText.text = questionName;
            isChange = false;
        }
    }

    public void SetUpLibraryList(string libraryID, string libraryName, UnityAction<string> onClick, UnityAction<string> onDel, bool isShowDeleteButton)
    {
        this.questionName = libraryName;
        this.libraryID = libraryID;
        this.onClickLib = onClick;
        onDelLib = onDel;
        delButton.gameObject.SetActive(isShowDeleteButton);
        isLibrary = true;
        isChange = true;
    }

    public void SetUpLibraryQuestion(int questionID, string questionName, UnityAction<int> onClick, UnityAction<int> onDel)
    {
        this.questionName = questionName;
        this.questionID = questionID;
        this.onClickQues = onClick;
        onDelQues = onDel;
        isLibrary = false;
        isChange = true;
    }

    private void QuestionClick()
    {
        if (isLibrary) onClickLib(libraryID);
        else onClickQues(questionID);
    }

    private void OnDelButtonClick()
    {
        if (isLibrary) onDelLib(libraryID);
        else onDelQues(questionID);
    }
}
