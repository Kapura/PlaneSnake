using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScorePanel : MonoBehaviour {

    public Text nameText;
    public Text scoreText;

    public void Initialise( string name, int score )
    {
        nameText.text = name;
        scoreText.text = score.ToString();
    }
}
