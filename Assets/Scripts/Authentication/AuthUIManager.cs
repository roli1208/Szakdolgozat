using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AuthUIManager : MonoBehaviour
{
   public static AuthUIManager instance;

    [SerializeField]
    private GameObject checkingForAccountUI;
    [SerializeField]
    private GameObject loginUI;
    [SerializeField]
    private GameObject registerUI;
    [SerializeField]
    private GameObject verifyEmailUI;
    [SerializeField]
    private TMP_Text verifyEmailText;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void clearUI()
    {
        FirebaseManager.instance.ClearOutputs();
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        //verifyEmailUI.SetActive(false);
       // checkingForAccountUI.SetActive(false);
    }
    public void LoginScreen()
    {
        clearUI();
        loginUI.SetActive(true);
    }
    public void RegisterScreen()
    {
        clearUI();
        registerUI.SetActive(true);
    }
    public void AwaitVerification(bool _emailSent, string _email, string _output)
    {
        clearUI();
        verifyEmailUI.SetActive(true);
        if (_emailSent)
        {
            verifyEmailText.text = $"Verification email sent to: {_email}";
        }
    }
}
