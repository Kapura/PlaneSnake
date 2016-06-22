using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{

    BoardScript board;
    SnakeScript snake;

    public GameObject resetPanel;
    public HighScoresTable highScores;
    public Text yourScoreText;
    public GameObject menuScreen;

    public string soundPath;
    public string tone1, tone2, grabTone, turnTone, deathTone;
    public string music;
    Dictionary<string, int> soundIds = new Dictionary<string, int>();

    public Material[] backgrounds;
    public GameObject bgQuad;

    public int currentLevel = 0;

    public bool _toning1 = true;

    public InstaButton upButton, downButton, leftButton, rightButton;
    public InstaButton rotLeft, rotRight;

    bool CanRotate { get { return true; } }

    void Awake()
    {
        board = GetComponent<BoardScript>();
        snake = GetComponent<SnakeScript>();
        resetPanel.gameObject.SetActive( false );

        soundIds[tone1] = AudioCenter.loadSound( tone1 );
        soundIds[tone2] = AudioCenter.loadSound( tone2 );
        soundIds[grabTone] = AudioCenter.loadSound( grabTone );
        soundIds[turnTone] = AudioCenter.loadSound( turnTone );
        soundIds[deathTone] = AudioCenter.loadSound( deathTone );
        /*
        upButton.OnClick = GoUp;
        downButton.OnClick = GoDown;
        leftButton.OnClick = GoLeft;
        rightButton.OnClick = GoRight;
         */
        rotRight.OnClick = RotateLeft;
        rotLeft.OnClick = RotateRight;
    }

    public void StartChildMode()
    {
        StartNewGame( 5, 1f, 1 );
    }

    public void StartStrategicMode()
    {
        StartNewGame( 8, .5f, 3 );
    }

    public void StartArcadeMode()
    {
        StartNewGame( 10, .25f, 5 );
    }

    void StartNewGame( int size, float moveInterval, int growthRate, int goalCount = int.MaxValue )
    {
        board.ui.Score = 0;

        board.cubeSize = size;
        board.moveInterval = moveInterval;
        board.goalCount = goalCount;
        board.growthRate = growthRate;
        snake.position = Point3.Zero;
        snake.snakeLength = 2;
        snake.planarDirection = Direction.North;

        board.Go();
        menuScreen.SetActive( false );

    }

    public void PlayMoveTone()
    {
        if ( _toning1 )
        {
            AudioCenter.playSound( soundIds[tone1] );
        }
        else
        {
            AudioCenter.playSound( soundIds[tone2] );
        }
        _toning1 = !_toning1;
    }

    public void PlayGrabTone()
    {
        AudioCenter.playSound( soundIds[grabTone] );
    }

    // Update is called once per frame
    void Update()
    {
        if ( board.running )
        {
            if ( Input.GetAxisRaw( "Horizontal" ) == 1 )
            {
                GoRight();
            }
            if ( Input.GetAxisRaw( "Horizontal" ) == -1 )
            {
                GoLeft();
            }
            if ( Input.GetAxisRaw( "Vertical" ) == 1 )
            {
                GoUp();
            }
            if ( Input.GetAxisRaw( "Vertical" ) == -1 )
            {
                GoDown();
            }
            if ( Input.GetButtonDown( "RotateRight" ) )
            {
                RotateRight();
            }
            if ( Input.GetButtonDown( "RotateLeft" ) )
            {
                RotateLeft();
            }
        }
    }

    public void GoLeft()
    {
        snake.planarDirection = Direction.East;
    }

    public void GoRight()
    {
        snake.planarDirection = Direction.West;
    }

    public void GoUp()
    {
        snake.planarDirection = Direction.North;
    }

    public void GoDown()
    {
        snake.planarDirection = Direction.South;
    }

    public void RotateRight()
    {
        if ( !board.isRotating && CanRotate )
        {
            board.RotateRight();
            AudioCenter.playSound( soundIds[turnTone] );
        }
    }

    public void RotateLeft()
    {
        if ( !board.isRotating && CanRotate )
        {
            board.RotateLeft();
            AudioCenter.playSound( soundIds[turnTone] );
        }
    }

    public void StartLevel( int level )
    {
        board.cubeSize = 5 + ( level / 2 );
        board.goalCount = 5 + 5 * level;
        board.moveInterval = 0.8f - ( 0.1f * level );
        snake.position = Point3.Zero;
        snake.snakeLength = 2;
        snake.planarDirection = Direction.North;

        if ( level < backgrounds.Length )
        {
            bgQuad.GetComponent<Renderer>().material = backgrounds[level];
        }
        else
        {
            bgQuad.GetComponent<Renderer>().material = backgrounds[0];
        }
    }

    public void GameOver()
    {
        AudioCenter.playSound( soundIds[deathTone] );
        snake.OnSnakeDeath();
        resetPanel.SetActive( true );
        yourScoreText.text = "Score: " + board.ui.Score;
        highScores.UpdateTable();
    }

    public void LevelComplete()
    {
        Debug.Log( "Win!" );
        currentLevel += 1;
        StartCoroutine( WaitAndStartLevel() );
    }

    IEnumerator WaitAndStartLevel()
    {
        yield return new WaitForSeconds( 1f );
        StartLevel( currentLevel );
    }

    public void ResetGame()
    {
        menuScreen.SetActive( true );
        resetPanel.SetActive( false );
    }
}
