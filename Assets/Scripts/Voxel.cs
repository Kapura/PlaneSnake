using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class Voxel : MonoBehaviour
{

    public Point3 point;

    private VoxelState _state;
    public VoxelState State {
        get { return _state; }
        set {
            _state = value;
            if (_state.name != "Empty") {
                RendMat = _state.mat;
                rend.enabled = true;
                for (int i = 0; i < _numActiveReflections; i++) {
                    _reflections[i].ReflectionMaterial = _state.reflectionMat;
                    _reflections[i].ReflectionsEnabled = true;
                }
            } else {  // "Empty" keyword disables the renderers
                rend.enabled = false;
                for (int i = 0; i < _numActiveReflections; i++) {
                    _reflections[i].ReflectionsEnabled = false;
                }
            }
        }
    }

    // Extensions from the centre cube to side cubes on other sides
    uint _extMask = 0;
    public GameObject TopExtension, BottomExtension, NorthExtension, EastExtension, SouthExtension, WestExtension;
    public bool ExtendTop
    {
        get { return GetBit( _extMask, 0 ); }
        set
        {
            _extMask = SetBit( _extMask, value, 0 );
            TopExtension.SetActive( value );
        }
    }
    public bool ExtendBottom
    {
        get { return GetBit( _extMask, 1 ); }
        set
        {
            _extMask = SetBit( _extMask, value, 1 );
            BottomExtension.SetActive( value );
        }
    }
    public bool ExtendNorth
    {
        get { return GetBit( _extMask, 2 ); }
        set
        {
            _extMask = SetBit( _extMask, value, 2 );
            NorthExtension.SetActive( value );
        }
    }
    public bool ExtendEast
    {
        get { return GetBit( _extMask, 3 ); }
        set
        {
            _extMask = SetBit( _extMask, value, 3 );
            EastExtension.SetActive( value );
        }
    }
    public bool ExtendSouth
    {
        get { return GetBit( _extMask, 4 ); }
        set
        {
            _extMask = SetBit( _extMask, value, 4 );
            SouthExtension.SetActive( value );
        }
    }
    public bool ExtendWest
    {
        get { return GetBit( _extMask, 5 ); }
        set
        {
            _extMask = SetBit( _extMask, value, 5 );
            WestExtension.SetActive( value );
        }
    }

    private Renderer[] extensionRenderers;

    // Reflections get turned on when the voxel is being generated, then they are spawned in Start()
    uint _refMask = 0;
    public bool TopRef
    {
        get { return GetBit( _refMask, 0 ); }
        set { _refMask = SetBit( _refMask, value, 0 ); }
    }
    public bool NorthRef
    {
        get { return GetBit( _refMask, 1 ); }
        set { _refMask = SetBit( _refMask, value, 1 ); }
    }
    public bool EastRef
    {
        get { return GetBit( _refMask, 2 ); }
        set { _refMask = SetBit( _refMask, value, 2 ); }
    }
    public bool SouthRef
    {
        get { return GetBit( _refMask, 3 ); }
        set { _refMask = SetBit( _refMask, value, 3 ); }
    }
    public bool WestRef
    {
        get { return GetBit( _refMask, 4 ); }
        set { _refMask = SetBit( _refMask, value, 4 ); }
    }
    public bool BottomRef
    {
        get { return GetBit( _refMask, 5 ); }
        set { _refMask = SetBit( _refMask, value, 5 ); }
    }

    static uint SetBit( uint mask, bool value, int bitNum )
    {
        if (value)
        {
            return mask | (uint)(1 << bitNum);
        }
        else
        {
            return mask & ~(uint)(1 << bitNum);
        }
    }

    static bool GetBit( uint mask, int bitNum )
    {
        return ( mask & ( 1 << bitNum ) ) != 0;
    }

    public GameObject reflectionPrefab;

    BoxReflections[] _reflections;
    int _numActiveReflections = 0;

    public Renderer rend;
    public Color MaterialColor
    {
        get { return rend.material.color; }
        //set { rend.material.color = value; }
    }

    public Material RendMat
    {
        get { return rend.material; }
        set {
            rend.material = value;
            foreach ( var renderer in extensionRenderers )
            {
                renderer.material = value;
            }
        }
    }

    public Color baseColor;

    public LineRenderer line_h; // Horizontal line on the current 2d plane
    public LineRenderer line_v; // Vertical line on the current 2d plane

    void Awake() {
        Assert.IsNotNull( rend );
        extensionRenderers = new Renderer[6] {
            TopExtension.GetComponent<Renderer>(),
            BottomExtension.GetComponent<Renderer>(),
            NorthExtension.GetComponent<Renderer>(),
            EastExtension.GetComponent<Renderer>(),
            SouthExtension.GetComponent<Renderer>(),
            WestExtension.GetComponent<Renderer>()
        };
        // Turn off all extensions
        ExtendTop = false;
        ExtendBottom = false;
        ExtendNorth = false;
        ExtendEast = false;
        ExtendSouth = false;
        ExtendWest = false;

        DisableGuidelines();
    }

    void Start() {
        // Voxels stay at fixed places in the cube, so we need only generate the proper reflections once
        _reflections = new BoxReflections[6];
        GameObject newRef = null;
        _numActiveReflections = 0;
        if (TopRef) {
            newRef = Instantiate(reflectionPrefab) as GameObject;
            newRef.transform.parent = this.transform;
            newRef.transform.Rotate(0, 0, 180f);
            newRef.transform.localPosition = new Vector3(0, .51f, 0);
            newRef.transform.localScale = Vector3.one;
            _reflections[_numActiveReflections] = newRef.GetComponent<BoxReflections>();
            _reflections[_numActiveReflections].ReflectionsEnabled = false;
            _numActiveReflections += 1;
        }

        if (BottomRef) {
            newRef = Instantiate(reflectionPrefab) as GameObject;
            newRef.transform.parent = this.transform;
            newRef.transform.Rotate(0, 0, 0);
            newRef.transform.localPosition = new Vector3( 0, -0.51f, 0 );
            newRef.transform.localScale = Vector3.one;
            _reflections[_numActiveReflections] = newRef.GetComponent<BoxReflections>();
            _reflections[_numActiveReflections].ReflectionsEnabled = false;
            _numActiveReflections += 1;
        }

        if (NorthRef) {
            newRef = Instantiate(reflectionPrefab) as GameObject;
            newRef.transform.parent = this.transform;
            newRef.transform.Rotate(-90, 0, 0);
            newRef.transform.localPosition = new Vector3( 0, 0, 0.51f );
            newRef.transform.localScale = Vector3.one;
            _reflections[_numActiveReflections] = newRef.GetComponent<BoxReflections>();
            _reflections[_numActiveReflections].ReflectionsEnabled = false;
            _numActiveReflections += 1;
        }

        if (SouthRef) {
            newRef = Instantiate(reflectionPrefab) as GameObject;
            newRef.transform.parent = this.transform;
            newRef.transform.Rotate(90, 0, 0);
            newRef.transform.localPosition = new Vector3( 0, 0, -0.51f );
            newRef.transform.localScale = Vector3.one;
            _reflections[_numActiveReflections] = newRef.GetComponent<BoxReflections>();
            _reflections[_numActiveReflections].ReflectionsEnabled = false;
            _numActiveReflections += 1;
        }

        if (EastRef) {
            newRef = Instantiate(reflectionPrefab) as GameObject;
            newRef.transform.parent = this.transform;
            newRef.transform.Rotate(0, 0, 90);
            newRef.transform.localPosition = new Vector3( 0.51f, 0, 0 );
            newRef.transform.localScale = Vector3.one;
            _reflections[_numActiveReflections] = newRef.GetComponent<BoxReflections>();
            _reflections[_numActiveReflections].ReflectionsEnabled = false;
            _numActiveReflections += 1;
        }

        if (WestRef) {
            newRef = Instantiate(reflectionPrefab) as GameObject;
            newRef.transform.parent = this.transform;
            newRef.transform.Rotate(0, 0, -90);
            newRef.transform.localPosition = new Vector3( -0.51f, 0, 0 );
            newRef.transform.localScale = Vector3.one;
            _reflections[_numActiveReflections] = newRef.GetComponent<BoxReflections>();
            _reflections[_numActiveReflections].ReflectionsEnabled = false;
            _numActiveReflections += 1;
        }
    }

    public void EnableGuidelines(Vector3 top, Vector3 bottom, Vector3 left, Vector3 right) {
        line_v.SetPosition(0, top);
        line_v.SetPosition(1, bottom);
        line_h.SetPosition(0, left);
        line_h.SetPosition(1, right);
        line_h.gameObject.SetActive(true);
        line_v.gameObject.SetActive(true);
    }

    public void DisableGuidelines() {
        line_h.gameObject.SetActive(false);
        line_v.gameObject.SetActive(false);
    }
}
