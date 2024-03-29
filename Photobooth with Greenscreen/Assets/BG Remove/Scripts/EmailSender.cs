﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DoozyUI;
using Klak.Video;
public class EmailSender : MonoBehaviour {

    System.Text.RegularExpressions.Regex mailValidator = new System.Text.RegularExpressions.Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$");

    public InputField emailInputfield;
    public UIButton sendEmailButton;
    public Text emailInfoText;
    public Text emailResultText;
    public UIElement[] elementsToShowAndHide;
    ProcAmp pa;
    KeyboardInput m_keyboardinput;
    bool isInDebugMode;

    string emailAddress;

    private void Start()
    {
        pa = GetComponent<ProcAmp>();
        isInDebugMode = pa.isInDebugMode;
        m_keyboardinput = GetComponent<KeyboardInput>();
    }
    public void SendEmail(string filepath, string filename)
    {
        
        if (ValidateEmailAddress())
        {
            emailInfoText.text = "";
          
            sendEmailButton.DisableButton();
            StartCoroutine(StartSendEmail(filepath, filename));

            
        }
        else
        {
            emailInfoText.text = "Invalid Email Address";
        }
    }

    string PHP_url;
    string status;
    IEnumerator StartSendEmail(string filepath, string filename)
    {
        ShowEndingPage();
        //emailResultText.text = "Email is sending...";
        WWWForm form = new WWWForm();
        form.AddField("function", "SendEmail");
        form.AddField("Receiver", emailAddress);
        form.AddField("FilePath", filepath);
        form.AddField("FileName", filename);
        

        PHP_url = pa.PHP_url;

        if (emailAddress == null)
        {
            Debug.Log("email is empty");
        }
       
        
        using (var w = new WWW(PHP_url, form))
        {
            yield return w;
            if (w.error != null)
            {
                status = "Fail";
                if (isInDebugMode) Debug.Log(DateTime.Now.ToString("HH:mm:ss ") + "php email status: " + status + "   info: " +  w.error);
                //emailResultText.text = "Fail to send the email. SOS.";
            }
            else
            {
                status = "Good";
                if (isInDebugMode) Debug.Log(DateTime.Now.ToString("HH:mm:ss ") + "php email status: " + status + "  send to: " + emailAddress);
                
                //emailResultText.text = "Email is sent successfully.";
                emailInputfield.text = "";
                StartCoroutine(ClearLocalFolder());
               

            }
        }
    }
    IEnumerator ClearLocalFolder()
    {
        WWWForm form = new WWWForm();
        form.AddField("function", "ClearLocalFolder");

        PHP_url = pa.PHP_url;

        using (var w = new WWW(PHP_url, form))
        {
            yield return w;
            if (w.error != null)
            {
                status = "Fail";
                if (isInDebugMode) Debug.Log(DateTime.Now.ToString("HH:mm:ss ") + "php clear local folder status: " + status + "   info: " + w.error);
            
            }
            else
            {
                status = "Good";
                if (isInDebugMode) Debug.Log(DateTime.Now.ToString("HH:mm:ss ") + "php clear local folder statuss: " + status);


            }
        }
    }


    bool ValidateEmailAddress()
    {
        if(isInDebugMode) Debug.Log("validate: " + emailInputfield.text);

        emailAddress = emailInputfield.text;
        if (!mailValidator.IsMatch(emailAddress))
        {
            return false;
        }
        else
        {
            return true;
        }
            
    }

    void ShowEndingPage()
    {
        elementsToShowAndHide[0].Hide(false);
        elementsToShowAndHide[2].Hide(false);
        elementsToShowAndHide[1].Show(false);
        m_keyboardinput.ResetContent();
    }

     
   
}
