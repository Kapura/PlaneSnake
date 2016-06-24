using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIScript : MonoBehaviour {

    public Image directionArrow;

    public Text scoreText;

    public GameController ctrl;

    private int _score;
    int Score {
        get { return _score; }
        set {
            _score = value;
            //scoreText.text = _score.ToString();
        }
    }

    private Direction _dir;
    public Direction SnakeOrientation {
        get { return _dir; }
        set {
            _dir = value;
            /*
            switch (_dir) {
                case Direction.North:
                    directionArrow.gameObject.transform.rotation = rotations[0];
                    break;
                case Direction.West:
                    directionArrow.gameObject.transform.rotation = rotations[1];
                    break;
                case Direction.South:
                    directionArrow.gameObject.transform.rotation = rotations[2];
                    break;
                case Direction.East:
                    directionArrow.gameObject.transform.rotation = rotations[3];
                    break;
            }
             */
        }
    }

    private Quaternion[] rotations;

	// Use this for initialization
	void Start () {
        rotations = new Quaternion[4];
        for (int i = 0; i < rotations.Length; i++) {
            rotations[i] = transform.rotation;
            transform.Rotate(0f, 0f, -90f);
        }
	}
}
