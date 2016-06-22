using UnityEngine;
using System.Collections;

public class BoxReflections : MonoBehaviour {

    private Renderer[] _rends;

    private Material _refMat = null;

    void Awake() {
        _rends = new Renderer[transform.childCount];
        int i = 0;
        foreach (Transform t in transform) {
            _rends[i] = t.GetComponent<Renderer>();
            if (_refMat == null) {
                _refMat = new Material(_rends[i].material);
            }
            _rends[i].material = _refMat;
        }
    }

    public void SetColor(Color newC) {
        _refMat.color = newC;
    }
}
