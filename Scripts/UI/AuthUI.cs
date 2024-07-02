using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthUI : MonoBehaviour
{
    public TMP_InputField signInMail;
    public TMP_InputField signInPass;

    public TMP_InputField signUpMail;
    public TMP_InputField signUpPass;
    public TMP_InputField signUpPlayerName;


    public void SignUp()
    {
        GameManager.instance.screenManager.Loading();
        GameManager.instance.firebaseAuth.SignUp(signUpMail.text, signUpPass.text, signUpPlayerName.text);
    }

    public void SignIn()
    {
        GameManager.instance.screenManager.Loading();
        GameManager.instance.firebaseAuth.SignIn(signInMail.text, signInPass.text);
    }
}
