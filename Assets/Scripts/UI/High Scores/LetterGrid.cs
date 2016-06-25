using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;

public class LetterGrid : MonoBehaviour
{
    public Text nameText;
    public HighScoresTable table;

    public string namePrefix = "Enter your name:";
    string _name = "";
    string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            var output = namePrefix;
            for ( int i = 0; i < 3; i++ )
            {
                if ( i >= _name.Length )
                {
                    output += "_";
                }
                else
                {
                    output += _name[i];
                }
            }
            nameText.text = output;
        }
    }

    int nameIdx = 0;

    const int maxChars = 3;

	// Use this for initialization
	void Start () {
        foreach ( Transform t in transform )
        {
            LetterButton b = t.GetComponent<LetterButton>();
            b.OnLetterPressed += RecieveLetter;
        }
	}

    void RecieveLetter( string c )
    {
        if ( nameIdx < maxChars )
        {
            nameIdx++;
            Name += c;
        }
        // Do nothing if at maxChars
    }

    public void Backspace()
    {
        nameIdx--;
        if ( nameIdx < 0 )
        {
            nameIdx = 0;
        }
        if ( nameIdx > 0 )
        {
            Name = Name.Substring( 0, nameIdx );
        }
        else
        {
            Name = "";
        }
    }

    public void Send()
    {
        if ( nameIdx > 0 )
        {
            table.SetNewName( Name );
        }
    }
}
