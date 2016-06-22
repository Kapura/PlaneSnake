using UnityEngine;
using System.Collections;

public class HighScoresTable : MonoBehaviour {

    public GameObject panelPrefab;
    public int numPanels;
    public BoardScript board;

    public string playerName = "Darnabelle";

    const string HIGH_SCORE_NAME_PREFIX = "hs_name_";
    const string HIGH_SCORE_SCORE_PREFIX = "hs_score_";

    HighScorePanel[] panels;

	// Use this for initialization
	void Awake () {
        panels = new HighScorePanel[numPanels];
        for ( int i = 0; i < panels.Length; i++ )
        {
            var newObject = Instantiate( panelPrefab ) as GameObject;
            newObject.transform.SetParent( this.transform, false );
            panels[i] = newObject.GetComponent<HighScorePanel>();
        }
	}

    public void UpdateTable()
    {
        int[] scores;
        string[] names;
        GetHighScores( panels.Length, out names, out scores );
        int score = board.ui.Score;
        if ( score > scores[numPanels - 1] )
        {
            // Insering new score
            int index = 0;
            while ( scores[index] >= score )
            {
                index++;
            }
            SetHighScore( numPanels, index, playerName, score );
            GetHighScores( numPanels, out names, out scores );
        }

        for ( int i = 0; i < panels.Length; i++ )
        {
            panels[i].Initialise( ( i + 1 ).ToString() + ": " + names[i], scores[i] );
        }
    }

    public static void SetHighScore(int numScores, int index, string name, int score)
    {
        string lastName = name;
        int lastScore = score;
        for ( int i = index; i < numScores; i++ )
        {
            var nameStr = HIGH_SCORE_NAME_PREFIX + i.ToString();
            var tempName = PlayerPrefs.GetString( nameStr );
            PlayerPrefs.SetString( nameStr, lastName );
            lastName = tempName;

            var scoreStr = HIGH_SCORE_SCORE_PREFIX + i.ToString();
            var tempScore = PlayerPrefs.GetInt( scoreStr );
            PlayerPrefs.SetInt( scoreStr, lastScore );
            lastScore = tempScore;
        }
    }

    public static void GetHighScores( int numScores, out string[] names, out int[] scores )
    {
        names = new string[numScores];
        scores = new int[numScores];
        for ( int i = 0; i < numScores; i++ )
        {
            var nameStr = HIGH_SCORE_NAME_PREFIX + i.ToString();
            var scoreStr = HIGH_SCORE_SCORE_PREFIX + i.ToString();
            if ( !PlayerPrefs.HasKey(nameStr) )
            {
                PlayerPrefs.SetString( nameStr, "sssss" );
            }
            if ( !PlayerPrefs.HasKey( scoreStr ) )
            {
                PlayerPrefs.SetInt( scoreStr, 5 );
            }
            names[i] = PlayerPrefs.GetString( nameStr );
            scores[i] = PlayerPrefs.GetInt( scoreStr );
        }
    }
}
