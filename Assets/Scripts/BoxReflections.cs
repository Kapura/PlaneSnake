using UnityEngine;
using System.Collections;

public class BoxReflections : MonoBehaviour {

    public Renderer[] rends;

    public Material ReflectionMaterial
    {
        set
        {
            if ( value != null )
            {
                foreach ( Renderer r in rends )
                {
                    r.material = value;
                }
            }
        }
    }

    bool _refEnabled;
    public bool ReflectionsEnabled
    {
        get { return _refEnabled; }
        set
        {
            _refEnabled = value;
            foreach ( Renderer r in rends )
            {
                r.enabled = _refEnabled;
            }
        }
    }
}
