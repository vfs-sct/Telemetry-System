using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Sign Up Fields")]
    public TMP_InputField signUpUsernameField;
    public TMP_InputField signUpEmailField;
    public TMP_InputField signUpPasswordField;

    [Header("Sign In Fields")]
    public TMP_InputField signInUsernameField;
    public TMP_InputField signInPasswordField;

    [Header("Auth Refernces")]
    public AuthService authService;

    [Header("Login Scene")]
    public string gameSceneName = "Game";

    public void SignUpButton()
    {
        string username = signUpUsernameField.text.Trim();
        string email = signUpEmailField.text.Trim();
        string password = signUpPasswordField.text.Trim();

        StartCoroutine(authService.SignUp(username, email, password, OnSignUpComleted));
    }

    private void OnSignUpComleted(bool success, string responseData)
    {
        if (success) 
        {
            Debug.Log("Sign up successful:" + responseData);
        }
        else
        {
            Debug.Log("Sign up failed:" + responseData);
        }
    }

    public void SignInButton()
    {
        string username = signInUsernameField.text.Trim();
        string password = signInPasswordField.text;

        StartCoroutine(authService.SignIn(username, password, OnSignInCompleted));
    }

    private void OnSignInCompleted(bool success, string responseData)
    {
        if (success)
        {
            AuthService.SigninResponse signResp = JsonUtility.FromJson<AuthService.SigninResponse>(responseData);


            if (!string.IsNullOrEmpty(signResp.token))
            {
                SessionManager.Instance.SetAuthToken(signResp.token);
                Debug.Log("Login Successful" + signResp.token);

                SceneManager.LoadScene(gameSceneName);
            }
            else
            {
                Debug.Log("No Token in response" + responseData);
            }
        }
        else
        {
            Debug.LogError("Login Failed" + responseData);
        }
    }
}