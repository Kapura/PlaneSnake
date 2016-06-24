using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class Dragger : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {

    public RectTransform dragBackground;

    public RectTransform[] dragPositions;

    public RectTransform leftEdge;
    public RectTransform rightEdle;

    public float smoothness;
    bool dragging = false;
    bool started = false;
    Vector3 targetPosition;

    public event Action<int> OnPositionChanged;
	
	// Update is called once per frame
	void Update () {

        if ( started && !dragging )
        {
            dragBackground.position = Vector3.Lerp( dragBackground.position, targetPosition, smoothness * Time.deltaTime );
        }
	}

    public void OnBeginDrag( PointerEventData eventData )
    {
        started = true;
        dragging = true;
    }

    public void OnEndDrag( PointerEventData eventData = null )
    {
        if ( !dragging )
            return;

        dragging = false;
        float smallestDist = float.MaxValue;
        int targetIndex = 0;
        for ( int i = 0; i < dragPositions.Length; i++ )
        {
            var dp = dragPositions[i];
            var sqrDist = Vector3.SqrMagnitude( dragBackground.position - dp.position );
            if ( sqrDist < smallestDist )
            {
                smallestDist = sqrDist;
                targetPosition = dp.position;
                targetIndex = i;
            }
        }
        OnPositionChanged( targetIndex );
    }

    public void OnDrag( PointerEventData eventData )
    {
        if ( !dragging )
            return;

        var position = dragBackground.position;
        position.x = eventData.position.x;
        dragBackground.position = position;
        if ( position.x > rightEdle.position.x || position.x < leftEdge.position.x )
        {
            OnEndDrag();
        }
    }
}
