using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SwipeArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

    public GameController ctrl;

    public LineRenderer liney;

	// Use this for initialization
	void Awake ()
    {
        liney.enabled = false;
	}

    public void OnDrag( PointerEventData eventData )
    {
        liney.enabled = true;
        var startPoint = new Vector3( eventData.pressPosition.x, eventData.pressPosition.y, Camera.main.nearClipPlane );
        liney.SetPosition( 0, Camera.main.ScreenToWorldPoint( startPoint ) );

        var endPoint = new Vector3( eventData.position.x, eventData.position.y, Camera.main.nearClipPlane );
        liney.SetPosition( 1, Camera.main.ScreenToWorldPoint( endPoint ) );

        var xDelta = eventData.position.x - eventData.pressPosition.x;
        var yDelta = eventData.position.y - eventData.pressPosition.y;

        if ( Mathf.Abs( xDelta ) > Mathf.Abs( yDelta ) )
        {
            // Horizontal movement
            if ( xDelta > 0 )
            {
                ctrl.GoRight();
            }
            else
            {
                ctrl.GoLeft();
            }
        }
        else
        {
            if ( yDelta > 0 )
            {
                ctrl.GoUp();
            }
            else
            {
                ctrl.GoDown();
            }
        }
    }

    public void OnPointerUp( PointerEventData eventData )
    {
        liney.enabled = false;
    }

    public void OnPointerDown( PointerEventData eventData )
    {
        if ( eventData.pointerId > 0 )
        {
            var firstTouch = Input.touches[0];
            var touchDelta = firstTouch.position - eventData.position;
            if ( touchDelta.x > 0 )
            {
                ctrl.RotateRight();
            }
            else
            {
                ctrl.RotateLeft();
            }
        }
    }
}
