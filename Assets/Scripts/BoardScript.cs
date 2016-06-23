using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardScript : MonoBehaviour {

    public UIScript ui;
    public GameObject voxelPrefab;
    public Transform frameTransform;
    public Transform universe;

    public int cubeSize = 5;
    public int growthRate = 5;

    public float moveInterval;
    public float rotateTime;

    public int goalCount;

    public VoxelState[] stateList;
    private Dictionary<string, VoxelState> _stateDict;

    private Voxel[, ,] _voxelSpace;

    private HashSet<Voxel>[] _XVoxelPlanes;
    private HashSet<Voxel>[] _YVoxelPlanes;

    public Direction Orientation = Direction.South; // initially, board faces north, so you see the south side

    public bool isRotating {
        get;
        private set;
    }

    private float _nextMove = -1f;

    private SnakeScript _snake;
    private GameController _ctrl;
    private Vector3 _baseCamPosition;
    public bool running { get; private set; }

    private Point3 _goal;

    void Awake() {
        Random.seed = (int)System.DateTime.Now.ToBinary();
        _stateDict = new Dictionary<string, VoxelState>();
        foreach (VoxelState vs in stateList) {
            _stateDict[vs.name] = vs;
        }

        _snake = GetComponent<SnakeScript>();
        _ctrl = GetComponent<GameController>();

        isRotating = false;
        _baseCamPosition = Camera.main.transform.localPosition;
        running = false;
        ui.Score = 0;

    }

	// Use this for initialization
    public void Go() {
        InitVoxelSpace(new Point3(cubeSize, cubeSize, cubeSize));
        NewGoal();
        _snake.Go();
        Camera.main.transform.localPosition = GetTargetCamPosition();
        float camDist = _snake.head.transform.position.z - Camera.main.transform.position.z;

        var snakeMat = GetState("Snake").mat;
        _snake.baseSnakeMat = snakeMat;
        float maxAlphaDist = camDist - 0.5f;
        float minAlphaDist = maxAlphaDist - cubeSize;
        float minBlackDist = camDist + 0.5f;
        float maxBlackDist = minBlackDist + cubeSize;

        snakeMat.SetFloat( "_MinAlphaDist", minAlphaDist );
        snakeMat.SetFloat( "_MaxAlphaDist", maxAlphaDist );
        snakeMat.SetFloat( "_MinBlackDist", minBlackDist );
        snakeMat.SetFloat( "_MaxBlackDist", maxBlackDist );

        var goalMat = _stateDict["Goal"].mat;
        goalMat.SetFloat( "_MinAlphaDist", minAlphaDist );
        goalMat.SetFloat( "_MaxAlphaDist", maxAlphaDist );
        goalMat.SetFloat( "_MinBlackDist", minBlackDist );
        goalMat.SetFloat( "_MaxBlackDist", maxBlackDist );

        _nextMove = Time.time + moveInterval;
        running = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (running) {
            if (!isRotating && Time.time > _nextMove) {
                _nextMove += moveInterval;
                _snake.Move();
            }
            if (ui.SnakeOrientation != _snake.planarDirection) {
                ui.SnakeOrientation = _snake.planarDirection;
            }
        }
	}

    void InitVoxelSpace(Point3 size) {
        transform.localRotation = Quaternion.identity;
        Orientation = Direction.South;

        // Clear the old boxes
        foreach (Transform t in this.transform) {
            if (t != frameTransform) {
                Destroy(t.gameObject);
            }
        }

        int length = size.x;
        int width = size.z;
        int height = size.y;
        _voxelSpace = new Voxel[length, width, height];
        _XVoxelPlanes = new HashSet<Voxel>[length];
        for (int i = 0; i < _XVoxelPlanes.Length; i++) {
            if (_XVoxelPlanes[i] == null) {
                _XVoxelPlanes[i] = new HashSet<Voxel>();
            } else {
                _XVoxelPlanes[i].Clear();
            }
        }
        _YVoxelPlanes = new HashSet<Voxel>[width];
        for (int i = 0; i < _YVoxelPlanes.Length; i++) {
            if (_YVoxelPlanes[i] == null) {
                _YVoxelPlanes[i] = new HashSet<Voxel>();
            } else {
                _YVoxelPlanes[i].Clear();
            }
        }

        Vector3 origin = new Vector3(
            -((float)(length - 1) / 2f),
            -((float)(height - 1) / 2f),
            -((float)(width - 1) / 2f));

        for (int x = 0; x < length; x++) {
            for (int y = 0; y < width; y++) {
                for (int z = 0; z < height; z++) {
                    Vector3 thisPosition = origin;
                    thisPosition.x += x;
                    thisPosition.y += z;
                    thisPosition.z += y;

                    GameObject obj = Instantiate(voxelPrefab) as GameObject;
                    obj.transform.SetParent(this.transform);
                    obj.transform.localPosition = thisPosition;

                    obj.transform.localScale = Vector3.one;
                    Voxel vox = obj.GetComponent<Voxel>();
                    vox.State = GetState("Empty");
                    vox.point = new Point3(x, y, z);

                    // Turn on reflections for edge voxels
                    if (x == 0) {
                        vox.WestRef = true;
                    }
                    if (x == length - 1) {
                        vox.EastRef = true;
                    }
                    if (y == 0) {
                        vox.SouthRef = true;
                    }
                    if (y == width - 1) {
                        vox.NorthRef = true;
                    }
                    if (z == 0) {
                        vox.BottomRef = true;
                    }
                    if (z == height - 1) {
                        vox.TopRef = true;
                    }

                    _voxelSpace[x, y, z] = vox;
                }
            }
        }

        frameTransform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
    }

    void NewGoal() {
        if (goalCount <= 0) {
            _ctrl.LevelComplete();
            running = false;
            return;
        }

        Point3 oldGoal = _goal;
        bool hasFoundGoal = false;
        while (!hasFoundGoal) {
            _goal.x = Random.Range(0, cubeSize);
            _goal.y = Random.Range(0, cubeSize);
            _goal.z = Random.Range(0, cubeSize);

            if (_goal != oldGoal && GetVoxelAt(_goal).State.name == "Empty") {
                hasFoundGoal = true;
            }
        }

        SetVoxel(_goal, "Goal");
    }

    VoxelState GetState(string name) {
        return _stateDict[name];
    }

    public Voxel GetVoxelAt(Point3 point) {
        if (point.x < 0 ||
            point.y < 0 ||
            point.z < 0 ||
            point.x >= cubeSize ||
            point.y >= cubeSize ||
            point.z >= cubeSize)
        {
            return null;
        }

        return _voxelSpace[point.x, point.y, point.z];
    }

    public bool SetVoxel(Point3 point, string stateName) {
        VoxelState state = GetState(stateName);
        Voxel v = GetVoxelAt(point);

        if (v == null) {
            Debug.Log("Out of bounds: " + point.ToString() + " " + stateName);
            if (stateName == "Snake") {
                // ded
                _ctrl.GameOver();
                running = false;
            } else {
                Debug.LogError("Invalid voxelsetting!");
            }

            return false;
        }

        if (stateName == "Snake") {
            // check to see if there's anything in the snakes way
            if (v.State.name == "Goal") {
                // grow + seed new goal
                ui.Score += 1;
                _snake.snakeLength += growthRate;
                goalCount -= 1;
                _ctrl.PlayGrabTone();
                NewGoal();
            } else if (v.State.name == "Snake") {
                // collided with self :(
                _ctrl.GameOver();
                running = false;
                return false;
            }
        }

        v.State = state;
        if (stateName == "Empty") {
            _XVoxelPlanes[v.point.x].Remove(v);
            _YVoxelPlanes[v.point.y].Remove(v);
        } else {
            _XVoxelPlanes[v.point.x].Add(v);
            _YVoxelPlanes[v.point.y].Add(v);
        }
        return true;
    }

    public void RotateLeft() {
        switch (Orientation) {
            case Direction.North:
                Orientation = Direction.West;
                break;
            case Direction.West:
                Orientation = Direction.South;
                break;
            case Direction.South:
                Orientation = Direction.East;
                break;
            case Direction.East:
                Orientation = Direction.North;
                break;
        }
        StartCoroutine(RotateBoard(90f));
    }

    public void RotateRight() {
        switch (Orientation) {
            case Direction.North:
                Orientation = Direction.East;
                break;
            case Direction.West:
                Orientation = Direction.North;
                break;
            case Direction.South:
                Orientation = Direction.West;
                break;
            case Direction.East:
                Orientation = Direction.South;
                break;
        }
        StartCoroutine(RotateBoard(-90f));
    }

    IEnumerator RotateBoard(float angles) {
        isRotating = true;
        _snake.DisableGuidelines();
        _snake.ClearWireBoxes();
        _nextMove = Time.time + rotateTime;
        Vector3 initialCamPosition = Camera.main.transform.localPosition;
        Vector3 targetCamPosition = GetTargetCamPosition();
        // initialise voxel's lerpappearence process
        var initialRotation = transform.localRotation;

        transform.Rotate( Vector3.up, angles );
        var finalRotation = transform.localRotation;
        transform.localRotation = initialRotation;

        while (Time.time < _nextMove) {
            // rotate board
            float t = 1f - ((_nextMove - Time.time) / rotateTime);

            var rot = Quaternion.Lerp( initialRotation, finalRotation, t );

            transform.localRotation = rot;

            // adjust camera distance (constant planar distance)
            Camera.main.transform.localPosition = Vector3.Lerp(initialCamPosition, targetCamPosition, t);
            
            yield return null;
        }
        transform.localRotation = finalRotation;
        Camera.main.transform.localPosition = targetCamPosition;

        _nextMove = Time.time + moveInterval;
        _snake.ProbeGuidelines(_snake.position);
        isRotating = false;
    }

    Vector3 GetTargetCamPosition() {
        Vector3 newCamPos = _baseCamPosition;
        switch (Orientation) {
            case Direction.North:
                newCamPos.z += (cubeSize - 1 - _snake.position.y);
                break;
            case Direction.West:
                newCamPos.z += _snake.position.x;
                break;
            case Direction.South:
                newCamPos.z += _snake.position.y;
                break;
            case Direction.East:
                newCamPos.z += (cubeSize - 1 - _snake.position.x);
                break;
        }
        return newCamPos;
    }
}
