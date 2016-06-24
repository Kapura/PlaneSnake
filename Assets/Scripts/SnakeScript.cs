using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnakeScript : MonoBehaviour {

    public Point3 position;
    private Point3 _lastPosition;

    private Direction _planarDirection;
    public Direction planarDirection {
        get { return _planarDirection; }
        set {
            if (position + GetMoveDirection(value) != _lastPosition) {
                _planarDirection = value;
            }
        }
    }

    public int snakeLength;

    private BoardScript _board;
    private GameController _ctrl;
    private bool _alive = true;
    private LinkedList<Point3> _snakeList;

    public Voxel head = null;

    private List<Voxel> _wiredBoxes;

    public Material baseSnakeMat;
    public Material snakeHeadMat;

    void Awake() {
        _board = GetComponent<BoardScript>();
        _ctrl = GetComponent<GameController>();
        _snakeList = new LinkedList<Point3>();
        _wiredBoxes = new List<Voxel>(3);
    }

	// Use this for initialization
    public void Go() {
        ClearWireBoxes();

        _snakeList.Clear();
        _alive = true;
        MakeSnake(position);
        _lastPosition = position;
	}

    bool MakeSnake(Point3 newSnakePos) {
        if (head != null)
            DisableGuidelines();

        if (_board.SetVoxel(newSnakePos, "Snake")) {

            if (head != null) {
                head.RendMat = baseSnakeMat;
            }
            head = _board.GetVoxelAt( newSnakePos );
            head.RendMat = snakeHeadMat;

            LinkedListNode<Point3> lln = new LinkedListNode<Point3>(newSnakePos);
            _snakeList.AddFirst(lln);
            if ( _snakeList.Count > 1 )
            {
                var neckPoint = lln.Next.Value;
                var neck = _board.GetVoxelAt( neckPoint );
                
                if ( neckPoint.x > newSnakePos.x )
                {
                    // East
                    head.ExtendEast = true;
                    neck.ExtendWest = true;
                }
                else if ( neckPoint.x < newSnakePos.x )
                {
                    // West
                    head.ExtendWest = true;
                    neck.ExtendEast = true;
                }
                else if ( neckPoint.y > newSnakePos.y )
                {
                    // North
                    head.ExtendNorth = true;
                    neck.ExtendSouth = true;
                }
                else if ( neckPoint.y < newSnakePos.y )
                {
                    // South
                    head.ExtendSouth = true;
                    neck.ExtendNorth = true;
                }
                else if ( neckPoint.z > newSnakePos.z )
                {
                    // Above
                    head.ExtendTop = true;
                    neck.ExtendBottom = true;
                }
                else if ( neckPoint.z < newSnakePos.z )
                {
                    // Below
                    head.ExtendBottom = true;
                    neck.ExtendTop = true;
                }

            }

            return true;
        }
        return false;
    }

    public void DisableGuidelines() {
        head.DisableGuidelines();
    }

    public void ClearWireBoxes() {
        foreach (Voxel v in _wiredBoxes) {
            if (v.State.name == "Cubed") {
                _board.SetVoxel(v.point, "Empty");
            }
        }
        _wiredBoxes.Clear();
    }

    public void ProbeGuidelines(Point3 fromPoint) {
        Voxel fromVox = _board.GetVoxelAt(fromPoint);

        ClearWireBoxes();

        if ( fromVox == null )
        {
            return;
        }

        // probe for guidelines
        Point3 nextPoint = fromPoint + new Point3(0, 0, 1);
        Voxel nextVox = _board.GetVoxelAt(nextPoint);
        Voxel lastVox = fromVox;
        while (nextVox != null && nextVox.State.name == "Empty") {
            lastVox = nextVox;
            nextPoint += new Point3(0, 0, 1);
            nextVox = _board.GetVoxelAt(nextPoint);
        }
        Vector3 top = lastVox != null ? lastVox.transform.position : Vector3.zero;
        if (fromVox != lastVox) {
            _wiredBoxes.Add(lastVox);
        }

        nextPoint = fromPoint + new Point3(0, 0, -1);
        nextVox = _board.GetVoxelAt(nextPoint);
        lastVox = fromVox;
        while (nextVox != null && nextVox.State.name == "Empty") {
            lastVox = nextVox;
            nextPoint += new Point3(0, 0, -1);
            nextVox = _board.GetVoxelAt(nextPoint);
        }
        Vector3 bottom = lastVox != null ? lastVox.transform.position : Vector3.zero;
        if (fromVox != lastVox) {
            _wiredBoxes.Add(lastVox);
        }

        Vector3 left = Vector3.zero;
        Vector3 right = Vector3.zero;
        if (_board.Orientation == Direction.South || _board.Orientation == Direction.North) {
            nextPoint = fromPoint + new Point3(-1, 0, 0);
            nextVox = _board.GetVoxelAt(nextPoint);
            lastVox = fromVox;
            while (nextVox != null && nextVox.State.name == "Empty") {
                lastVox = nextVox;
                nextPoint += new Point3(-1, 0, 0);
                nextVox = _board.GetVoxelAt(nextPoint);
            }
            left = lastVox.transform.position;
            if (fromVox != lastVox) {
                _wiredBoxes.Add(lastVox);
            }

            nextPoint = fromPoint + new Point3(1, 0, 0);
            nextVox = _board.GetVoxelAt(nextPoint);
            lastVox = fromVox;
            while (nextVox != null && nextVox.State.name == "Empty") {
                lastVox = nextVox;
                nextPoint += new Point3(1, 0, 0);
                nextVox = _board.GetVoxelAt(nextPoint);
            }
            right = lastVox.transform.position;
            if (fromVox != lastVox) {
                _wiredBoxes.Add(lastVox);
            }
        } else {
            nextPoint = fromPoint + new Point3(0, -1, 0);
            nextVox = _board.GetVoxelAt(nextPoint);
            lastVox = fromVox;
            while (nextVox != null && nextVox.State.name == "Empty") {
                lastVox = nextVox;
                nextPoint += new Point3(0, -1, 0);
                nextVox = _board.GetVoxelAt(nextPoint);
            }
            left = lastVox.transform.position;
            if (fromVox != lastVox) {
                _wiredBoxes.Add(lastVox);
            }

            nextPoint = fromPoint + new Point3(0, 1, 0);
            nextVox = _board.GetVoxelAt(nextPoint);
            lastVox = fromVox;
            while (nextVox != null && nextVox.State.name == "Empty") {
                lastVox = nextVox;
                nextPoint += new Point3(0, 1, 0);
                nextVox = _board.GetVoxelAt(nextPoint);
            }
            right = lastVox.transform.position;
            if (fromVox != lastVox) {
                _wiredBoxes.Add(lastVox);
            }
        }
        head.EnableGuidelines(top, bottom, left, right);
        foreach (Voxel v in _wiredBoxes) {
            _board.SetVoxel(v.point, "Cubed");
        }
    }

    public void Move() {
        if (!_alive)
            return;
        _lastPosition = position;
        position += GetMoveDirection(planarDirection);
        if (MakeSnake(position)) {
            //delete tail node
            if (_snakeList.Count > snakeLength) {
                LinkedListNode<Point3> lln = _snakeList.Last;
                _snakeList.RemoveLast();
                var vox = _board.GetVoxelAt( lln.Value );
                vox.ExtendTop = false;
                vox.ExtendBottom = false;
                vox.ExtendNorth = false;
                vox.ExtendEast = false;
                vox.ExtendSouth = false;
                vox.ExtendWest = false;
                _board.SetVoxel(lln.Value, "Empty");
            }

            ProbeGuidelines( position );
        }

    }

    public void OnSnakeDeath() {
        Debug.Log("ded");
        ClearWireBoxes();
        _alive = false;
    }

    Point3 GetMoveDirection(Direction fromDirection) {
        switch (fromDirection) {
            case Direction.North:
                return new Point3(0, 0, 1);
            case Direction.South:
                return new Point3(0, 0, -1);
            case Direction.West:
                switch (_board.Orientation) {
                    case Direction.North:
                        return new Point3(-1, 0, 0);
                    case Direction.West:
                        return new Point3(0, -1, 0);
                    case Direction.South:
                        return new Point3(1, 0, 0);
                    case Direction.East:
                        return new Point3(0, 1, 0);
                }
                break;
            case Direction.East:
                switch (_board.Orientation) {
                    case Direction.North:
                        return new Point3(1, 0, 0);
                    case Direction.West:
                        return new Point3(0, 1, 0);
                    case Direction.South:
                        return new Point3(-1, 0, 0);
                    case Direction.East:
                        return new Point3(0, -1, 0);
                }
                break;
        }

        Debug.LogError("Aint got no move direction??");
        return Point3.Zero;
    }
}
