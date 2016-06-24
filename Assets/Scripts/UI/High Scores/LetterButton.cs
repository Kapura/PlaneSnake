using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Collections;

public class LetterButton : MonoBehaviour
{
    public Text characterText;

    string Character { get { return characterText.text; } }

    public event Action<string> OnLetterPressed;

    void Awake()
    {
        Assert.IsNotNull( characterText );
    }

    public void SendLetter()
    {
        OnLetterPressed( Character );
    }
}
