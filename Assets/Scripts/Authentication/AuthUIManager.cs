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
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        FirebaseManager.instance.ClearOutputs();
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
}
