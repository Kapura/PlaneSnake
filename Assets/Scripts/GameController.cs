using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{

    BoardScript board;
    SnakeScript snake;

    public AudioSource audio;
    public GameObject resetPanel;
    public HighScoresTable highScores;
    public Text yourScoreText;
    public GameObject menuScreen;
    public GameObject creditScreen;

    public Dragger difficultySelector;

    public AudioClip grabTone, turnTone, deathTone;
    Dictionary<string, int> soundIds = new Dictionary<string, int>();

    public Material[] backgrounds;
    public GameObject bgQuad;

    public int currentLevel = 0;

    public bool _toning1 = true;

    public int modeIndex = 1;
    Action[] modeArray;

    public InstaButton upButton, downButton, leftButton, rightButton;
    public InstaButton rotLeft, rotRight;

    bool CanRotate { get { return true; } }

    void Awake()
    {
        InitModeArray();
        SetDifficulty( 1 );
        board = GetComponent<BoardScript>();
        snake = GetComponent<SnakeScript>();
        resetPanel.gameObject.SetActive( false );
        HideCredits();

        /*
        upButton.OnClick = GoUp;
        downButton.OnClick = GoDown;
        leftButton.OnClick = GoLeft;
        rightButton.OnClick = GoRight;
         */
        rotRight.OnClick = RotateLeft;
        rotLeft.OnClick = RotateRight;

        difficultySelector.OnPositionChanged += SetDifficulty;
    }

    public void SetDifficulty( int i )
    {
        modeIndex = i;
    }

    public void StartChildMode()
    {
        StartNewGame( 5, 1f, 1 );
    }

    public void StartStrategicMode()
    {
        StartNewGame( 7, .5f, 3 );
    }

    public void StartArcadeMode()
    {
        StartNewGame( 10, .25f, 5 );
    }

    void InitModeArray()
    {
        modeArray = new Action[3];
        modeArray[0] = StartChildMode;
        modeArray[1] = StartStrategicMode;
        modeArray[2] = StartArcadeMode;
    }

    public void StartDynamicMode()
    {
        Debug.Log( modeIndex );
        modeArray[modeIndex]();
    }

    void StartNewGame( int size, float moveInterval, int growthRate, int goalCount = int.MaxValue )
    {
        board.score = 0;

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

    public void PlayGrabTone()
    {
        audio.PlayOneShot( grabTone );
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
            audio.PlayOneShot( turnTone );
        }
    }

    public void RotateLeft()
    {
        if ( !board.isRotating && CanRotate )
        {
            board.RotateLeft();
            audio.PlayOneShot( turnTone );
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
        audio.PlayOneShot( deathTone );
        snake.OnSnakeDeath();
        resetPanel.SetActive( true );
        yourScoreText.text = "Score: " + board.score;
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

    public void ShowCredits()
    {
        creditScreen.gameObject.SetActive( true );
    }

    public void HideCredits()
    {
        creditScreen.gameObject.SetActive( false );
    }
}
