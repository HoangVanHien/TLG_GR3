using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GamePlayQuestionUI : MonoBehaviour
{
    private GameManager gameManager;

    private LibraryQuestionModeEnum libraryQuestionModeEnum;
    private QuestionModeEnum questionModeEnum;

    public TMP_Text turnText;
    public TMP_Text timeText;
    public TMP_InputField timeInput;
    public TMP_Text scoreText;
    public TMP_InputField scoreInput;

    public TMP_InputField questionInput;
    public TMP_Text questionText;

    public GameObject typeInAnswerPanel;
    public TMP_InputField typeInAnswer;

    public List<TMP_InputField> multiplechoiceAnswersInput = new List<TMP_InputField>();
    public List<TMP_Text> multiplechoiceAnswersText = new List<TMP_Text>();
    public List<Button> multiplechoiceAnswerButtons = new List<Button>();
    public List<Toggle> isRightAnswerToggleList = new List<Toggle>();
    private List<bool> isRightAnswerList = new List<bool>();

    public TMP_Dropdown questionModeDropDown;
    private bool isChangingQuestionMode = false;

    public Button backButton;
    public TMP_Text backButtonText;
    public Button midButton;
    public TMP_Text midButtonText;
    public Button confirmButton;
    public TMP_Text confirmButtonText;

    public Color greenColor;
    public Color blueColor;

    private bool isNeedToSetUpQuestion = false;
    LibraryPacket libraryPacket;
    LibraryQuestion libraryQuestion;
    LibraryWord libraryWord;
    private int libraryQuestionID;
    private bool isMatch = false;

    private bool isMatchChange = false;
    public bool isAnswered = false;

    private bool isNeedToSetUpRank = false;
    private bool isEndGame = false;
    public GameObject rankObject;
    public GameObject questionObject;
    public Transform rankContainer;
    public RoomPlayerUI roomPlayerUIPrefab;
    public List<RoomPlayerUI> playerUIList = new List<RoomPlayerUI>();


    private void Start()
    {
        gameManager = GameManager.instance;
        foreach (Button answer in multiplechoiceAnswerButtons)
        {
            isRightAnswerList.Add(false);
        }
        backButton.onClick.AddListener(OnBackButtonClick);
        midButton.onClick.AddListener(OnMidButtonClick);
        confirmButton.onClick.AddListener(OnConfirmButtonClick);

        gameManager.data.OnLibraryQuestionIDChangeAddListener(() => { isNeedToSetUpQuestion = true; });
        gameManager.matchManager.OnShowRankAddListener(ShowRank);
        gameManager.matchManager.OnShowQuestion(ShowQuestion);
        gameManager.matchManager.OnMatchStructChangeAddListener(OnMatchStructChange);
        gameManager.matchManager.OnEndGame(OnEndGame);

        //List<string> questionModes = new List<string>();
        //for (int i = 0; i < (int)QuestionModeEnum.count; i++)
        //{
        //    questionModes.Add(((QuestionModeEnum)i).ToString());
        //}
        //questionModeDropDown.AddOptions(questionModes);
        questionModeDropDown.onValueChanged.AddListener(OnQuestionModeChange);

        //isNeedToSetUp = true;

        //int a = Random.Range(1, 200);
        //int b = Random.Range(1, 200);
        //int an = a + b;
        //List<string> rAns = new List<string>() { an.ToString() };
        //for (int j = 1; j < 4; j++)
        //{
        //    int plus = Random.Range(-20, 20);
        //    rAns.Add((an + (plus != 0 ? plus : 1)).ToString());
        //}
        //libraryQuestion = new LibraryQuestion((a + " + " + b).ToString(), QuestionModeEnum.multipleRightAnswer, rAns, new List<int> { 0 });
        //SetUpQuestionPlay();
    }

    private void Update()
    {
        if (!gameManager)
        {
            gameManager = GameManager.instance;
            if (!gameManager) return;
        }
        if (!gameManager.isDoneSetUp) return;
        if (isNeedToSetUpQuestion)
        {
            isNeedToSetUpQuestion = false;
            SetUpQuestion();
        }
        if (isNeedToSetUpRank)
        {
            isNeedToSetUpRank = false;
            SetUpRank(gameManager.matchManager.matchPlayers);
        }
        if (isMatch)
        {
            TopBarSetUp();
            if (isEndGame) backButton.gameObject.SetActive(true);
        }
        //if (!isAddListener) gameManager.matchManager.OnMatchStructChangeAddListener(SetUpMatch);
        //timeText.text = "Time\n" + GameManager.instance.matchManager.timRemain;
    }

    private void SetUpQuestion()
    {
        isMatch = gameManager.data.isLibraryInMatch;
        if (isMatch)
        {
            libraryQuestionID = gameManager.matchManager.matchStruct.curLibraryQuestion;
            libraryPacket = gameManager.matchManager.libraryPacket;
        }
        else
        {
            libraryQuestionID = gameManager.data.GetLibraryQuestionID();
            libraryPacket = gameManager.data.GetLibraryPacket();
        }
        //libraryQuestion = new LibraryQuestion("Upgrade", QuestionModeEnum.typeIn);
        //libraryPacket = new LibraryPacket("", libraryQuestions: new List<LibraryQuestion> { libraryQuestion });
        //isMatch = true;
        switch (libraryPacket.modeEnum)
        {
            case LibraryQuestionModeEnum.question:
                if (libraryPacket.libraryQuestions == null) libraryPacket.libraryQuestions = new List<LibraryQuestion>();
                if (libraryPacket.libraryQuestions.Count <= libraryQuestionID) libraryQuestion = new LibraryQuestion("");
                else libraryQuestion = libraryPacket.libraryQuestions[libraryQuestionID];
                break;
            case LibraryQuestionModeEnum.word:
                if (libraryPacket.libraryWords == null) libraryPacket.libraryWords = new List<LibraryWord>();
                if (libraryPacket.libraryWords.Count <= libraryQuestionID) libraryQuestion = new LibraryQuestion("");
                else libraryWord = libraryPacket.libraryWords[libraryQuestionID];
                break;
            default:
                break;
        }
        if (isMatch) SetUpQuestionMatch();
        else SetUpLibraryQuestion();
        midButton.gameObject.SetActive(false);//fix later
        gameManager.screenManager.BringUp(ScreenEnum.GamePlay);
    }

    private void SetUpLibraryQuestion()
    {
        if (!isChangingQuestionMode) questionModeEnum = libraryQuestion.questionMode;
        else isChangingQuestionMode = false;
        questionModeDropDown.SetValueWithoutNotify((int)questionModeEnum);
        backButton.gameObject.SetActive(true);
        midButton.gameObject.SetActive(false);
        confirmButtonText.text = "Save";
        confirmButton.gameObject.SetActive(true);

        timeText.text = "Time\n";
        timeInput.text = libraryQuestion.time.ToString();
        timeInput.gameObject.SetActive(true);
        scoreText.text = "Score\n";
        scoreInput.text = libraryQuestion.score.ToString();
        scoreInput.gameObject.SetActive(true);
        turnText.text = "";

        QuestionSetUp(true, libraryQuestion.question);

        switch (questionModeEnum)
        {
            case QuestionModeEnum.oneRightAnswer:
                TypeInAnswerSetUp(false);
                MultiplechoiceAnswersInputSetUp(true, libraryQuestion.answers);
                MultiplechoiceAnswerButtonsSetUp(true);
                IsRightAnswerToggleListSetUp(true, libraryQuestion.isRightAnswerIndexList);
                break;
            case QuestionModeEnum.multipleRightAnswer:
                TypeInAnswerSetUp(false);
                MultiplechoiceAnswersInputSetUp(true, libraryQuestion.answers);
                MultiplechoiceAnswerButtonsSetUp(true);
                IsRightAnswerToggleListSetUp(true, libraryQuestion.isRightAnswerIndexList);
                break;
            case QuestionModeEnum.typeIn:
                TypeInAnswerSetUp(true,libraryQuestion.answers);
                MultiplechoiceAnswersInputSetUp(false);
                MultiplechoiceAnswerButtonsSetUp(false);
                IsRightAnswerToggleListSetUp(false);
                break;
            default:
                break;
        }

        questionObject.SetActive(true);
        rankObject.SetActive(false);
    }

    public void SetUpQuestionMatch()
    {
        isEndGame = false;
        questionModeEnum = libraryQuestion.questionMode;
        questionModeDropDown.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        midButton.gameObject.SetActive(false);
        confirmButtonText.text = "Confirm";
        timeInput.gameObject.SetActive(false);
        scoreInput.gameObject.SetActive(false);
        QuestionSetUp(false, libraryQuestion.question);
        switch (questionModeEnum)
        {
            case QuestionModeEnum.oneRightAnswer:
                TypeInAnswerSetUp(false);
                MultiplechoiceAnswersInputSetUp(false);
                MultiplechoiceAnswerButtonsSetUp(true, libraryQuestion.answers);
                IsRightAnswerToggleListSetUp(false);
                confirmButton.gameObject.SetActive(false);
                break;
            case QuestionModeEnum.multipleRightAnswer:
                TypeInAnswerSetUp(false);
                MultiplechoiceAnswersInputSetUp(false);
                MultiplechoiceAnswerButtonsSetUp(true, libraryQuestion.answers);
                IsRightAnswerToggleListSetUp(true);
                confirmButton.gameObject.SetActive(true);
                break;
            case QuestionModeEnum.typeIn:
                TypeInAnswerSetUp(true);
                MultiplechoiceAnswersInputSetUp(false);
                MultiplechoiceAnswerButtonsSetUp(false);
                IsRightAnswerToggleListSetUp(false);
                confirmButton.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void QuestionSetUp(bool isQuestionInputActive, string content)
    {
        questionInput.text = questionText.text = content;
        questionInput.gameObject.SetActive(isQuestionInputActive);
        questionText.gameObject.SetActive(!isQuestionInputActive);
    }
    private void MultiplechoiceAnswerButtonsSetUp(bool isActive, List<string> content = null)
    {
        for (int i = 0; i < multiplechoiceAnswerButtons.Count; i++)
        {
            multiplechoiceAnswerButtons[i].gameObject.SetActive(isActive);
            multiplechoiceAnswersText[i].text = content == null ? "" : content.Count <= i ? "" : content[i];
            multiplechoiceAnswerButtons[i].image.color = blueColor; 
        }
    }
    private void MultiplechoiceAnswersInputSetUp(bool isActive, List<string> content = null)
    {
        for (int i = 0; i < multiplechoiceAnswersInput.Count; i++)
        {
            multiplechoiceAnswersInput[i].gameObject.SetActive(isActive);
            multiplechoiceAnswersInput[i].text = content == null ? "" : content.Count <= i ? "" : content[i];
        }
    }
    private void IsRightAnswerToggleListSetUp(bool isActive, List<int> isOn = null)
    {
        for (int i = 0; i < isRightAnswerToggleList.Count; i++)
        {
            isRightAnswerToggleList[i].gameObject.SetActive(isActive);
            isRightAnswerToggleList[i].isOn = isOn == null ? false : isOn.Contains(i);
        }
    }
    private void TypeInAnswerSetUp(bool isActive, List<string> content = null)
    {
        typeInAnswerPanel.gameObject.SetActive(isActive);
        typeInAnswer.text = content == null ? "" : content.Count <= 0 ? "" : content[0];
    }

    private void OnQuestionModeChange(int value)
    {
        questionModeEnum = (QuestionModeEnum)value;
        isChangingQuestionMode = true;
        isNeedToSetUpQuestion = true;
    }

    private void TopBarSetUp()
    {
        timeText.text = "Time\n" + ((int)gameManager.matchManager.timRemain).ToString();
        scoreText.text = "Score\n" + gameManager.matchManager.score;
        turnText.text = "Turn\n" + (gameManager.matchManager.matchStruct.curLibraryQuestion + 1);
    }

    public void OnMultiplechoiceAnswerButtonClick(int index)
    {
        if (isAnswered) return;
        if (questionModeEnum == QuestionModeEnum.oneRightAnswer)
        {
            List<int> rightAnswer = gameManager.matchManager.rightAnswer;
            if (rightAnswer != null && rightAnswer.Count > 0)
            {
                multiplechoiceAnswerButtons[rightAnswer[0]].image.color = greenColor;
                isAnswered = true;
                gameManager.matchManager.AnswerQuestion(index == rightAnswer[0]);
            }
        }
        else if(questionModeEnum == QuestionModeEnum.multipleRightAnswer)
        {
            if (index < 0 || index >= 4) return;
            isRightAnswerToggleList[index].isOn = !isRightAnswerToggleList[index].isOn;
        }
    }

    private void OnBackButtonClick()
    {
        BringDownThis();
        if (isMatch)
        {
            gameManager.matchManager.ExitMatch();
        }
    }

    private void OnMidButtonClick()
    {

    }

    private void OnConfirmButtonClick()
    {
        if (isMatch)
        {
            AnswerWithConfirm();
        }
        else
        {
            SaveLibraryQuestion();
        }
    }

    private void AnswerWithConfirm()
    {
        if (isAnswered) return;
        if (questionModeEnum == QuestionModeEnum.typeIn)
        {
            isAnswered = true;
            string answer = gameManager.matchManager.rightTypeIn;
            if (typeInAnswer.text == answer)
            {
                gameManager.matchManager.AnswerQuestion(true);
            }
            else
            {
                questionText.text = questionText.text + "\n Right answer: " + answer;
                gameManager.matchManager.AnswerQuestion(false);
            }
        }
        else if (questionModeEnum == QuestionModeEnum.multipleRightAnswer)
        {
            List<int> answers = gameManager.matchManager.rightAnswer;
            if (answers == null)
            {
                Debug.Log("No answer");
                return;
            }
            bool isRight = true;
            for (int i = 0; i < 4; i++)
            {
                if (i < answers.Count) multiplechoiceAnswerButtons[answers[i]].image.color = greenColor;
                if (isRightAnswerToggleList[i].isOn && !answers.Contains(i))
                {
                    Debug.Log("wrong answer 1: " + i);
                    isRight = false;
                }
                else if (!isRightAnswerToggleList[i].isOn && answers.Contains(i))
                {
                    Debug.Log("wrong answer 2: " + i);
                    isRight = false;
                }
            }
            gameManager.matchManager.AnswerQuestion(isRight);
        }
    }

    private void SaveLibraryQuestion()
    {
        if(questionInput.text == null || questionInput.text == "")
        {
            Debug.Log("null question save");
            return;
        }
        libraryQuestion.question = questionInput.text;
        libraryQuestion.answers = new List<string>();
        libraryQuestion.questionMode = (QuestionModeEnum)questionModeDropDown.value;
        switch (libraryQuestion.questionMode)
        {
            case QuestionModeEnum.oneRightAnswer:
                foreach (TMP_InputField ans in multiplechoiceAnswersInput)
                {
                    if (ans.text == null || ans.text == "")
                    {
                        Debug.Log("null answer save");
                        return;
                    } 
                    libraryQuestion.answers.Add(ans.text);
                    
                }
                libraryQuestion.isRightAnswerIndexList = new List<int>();
                for (int i = 0; i < isRightAnswerToggleList.Count; i++)
                {
                    Debug.Log(i + " : " + isRightAnswerToggleList[i].isOn);
                    if (isRightAnswerToggleList[i].isOn)
                    {
                        libraryQuestion.isRightAnswerIndexList.Add(i);
                        break;
                    }
                }
                if (libraryQuestion.isRightAnswerIndexList.Count <= 0)
                {
                    Debug.Log("null pick answer save");
                    return;
                }
                break;
            case QuestionModeEnum.multipleRightAnswer:
                foreach (TMP_InputField ans in multiplechoiceAnswersInput)
                {
                    if (ans.text == null || ans.text == "")
                    {
                        Debug.Log("null answer save");
                        return;
                    }
                    libraryQuestion.answers.Add(ans.text);

                }
                libraryQuestion.isRightAnswerIndexList = new List<int>();
                for (int i = 0; i < isRightAnswerToggleList.Count; i++)
                {
                    if (isRightAnswerToggleList[i].isOn) libraryQuestion.isRightAnswerIndexList.Add(i);
                }
                if (libraryQuestion.isRightAnswerIndexList.Count <= 0)
                {
                    Debug.Log("no pick answer save");
                    return;
                }
                break;
            case QuestionModeEnum.typeIn:
                if(typeInAnswer.text == null || typeInAnswer.text == "")
                {
                    Debug.Log("null answer save");
                    return;
                }
                libraryQuestion.answers.Add(typeInAnswer.text);
                break;
            default:
                break;
        }

        libraryQuestion.time = timeInput.text == null || timeInput.text == "" ? 10 : float.Parse(timeInput.text);
        libraryQuestion.score = scoreInput.text == null || scoreInput.text == "" ? 10 : int.Parse(scoreInput.text);

        if (libraryPacket.libraryQuestions.Count <= libraryQuestionID) libraryPacket.libraryQuestions.Add(libraryQuestion);
        else libraryPacket.libraryQuestions[libraryQuestionID] = libraryQuestion;

        gameManager.data.UpdateLibrary(libraryPacket);
        BringDownThis();
        gameManager.screenManager.Announce("Library question saved.");
    }

    private void BringDownThis()
    {
        gameManager.screenManager.BringDown(ScreenEnum.GamePlay);
    }

    public void ShowRank()
    {
        isNeedToSetUpRank = true;
        rankObject.SetActive(true);
        questionObject.SetActive(false);
        isAnswered = false;
    }

    public void SetUpRank(List<MatchPlayer> matchPlayers)
    {
        questionModeDropDown.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        midButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        int i = 0;
        foreach (MatchPlayer player in matchPlayers)
        {
            Debug.Log("Set up " + player.name + " on " + i);
            if (playerUIList.Count <= i)
            {
                //add new
                RoomPlayerUI newSlot = Instantiate(roomPlayerUIPrefab, rankContainer);
                playerUIList.Add(newSlot);
            }
            playerUIList[i].SetupRankPlayer(player);
            i++;
        }
        for (; i  < playerUIList.Count; i++)
        {
            playerUIList[i].Reset();
        }
    }

    public void ShowQuestion()
    {
        questionObject.SetActive(true);
        rankObject.SetActive(false);
    }

    public void OnMatchStructChange()
    {
        isNeedToSetUpQuestion = true;
    }

    public void OnEndGame()
    {
        backButton.gameObject.SetActive(true);
    }
}
