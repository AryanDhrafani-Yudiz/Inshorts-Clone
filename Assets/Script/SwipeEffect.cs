using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeEffect : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 _initialPosition;
    private Vector3 _previousCardinitialPosition;

    private float _distanceMoved;
    private float _previousCardDistanceMoved;

    public bool isPreviousCardMovable = true;
    public SwipeEffect previousCard;
    private readonly float timeForCoroutine = 0.3f;

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.delta.y > 0)
        {
            if (previousCard.transform.localPosition.y < _previousCardinitialPosition.y && isPreviousCardMovable)
            {
                previousCard.transform.localPosition = new Vector2(previousCard.transform.localPosition.x, previousCard.transform.localPosition.y + eventData.delta.y * 1.5f);

            }
            else
            {
                transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + eventData.delta.y * 1.5f);
            }
        }
        else
        {
            if (transform.localPosition.y > _initialPosition.y)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + eventData.delta.y * 1.5f);
            }
            else
            {
                transform.localPosition = _initialPosition;
                if (isPreviousCardMovable)
                {
                    previousCard.transform.localPosition = new Vector2(previousCard.transform.localPosition.x, Mathf.Clamp(previousCard.transform.localPosition.y + eventData.delta.y * 1.5f, _initialPosition.y, _previousCardinitialPosition.y));

                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _initialPosition = transform.localPosition;
        _previousCardinitialPosition = previousCard.transform.localPosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        _distanceMoved = Mathf.Abs(transform.localPosition.y - _initialPosition.y);
        _previousCardDistanceMoved = Mathf.Abs(previousCard.transform.localPosition.y - _previousCardinitialPosition.y);

        if (_distanceMoved < 0.2 * Screen.height)
        {
            StartCoroutine(MovedCard(_initialPosition.y, false));
        }
        else
        {
            StartCoroutine(MovedCard(transform.localPosition.y + Screen.height, true));
        }

        if (_previousCardDistanceMoved < 0.2 * Screen.height)
        {
            StartCoroutine(MovedPreviousCard(_previousCardinitialPosition.y, false));
        }
        else
        {
            StartCoroutine(MovedPreviousCard(_initialPosition.y, true));
        }
    }

    private IEnumerator MovedCard(float targetPosiition, bool triggerEvent)
    {
        float time = 0;

        while (time < timeForCoroutine)
        {
            time += Time.deltaTime;

            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.SmoothStep(transform.localPosition.y, targetPosiition, time), 0);

            yield return null;
        }
        transform.localPosition = new Vector3(transform.localPosition.x, targetPosiition, 0);
        if (triggerEvent) CardManager.TriggerNextPage();
    }
    private IEnumerator MovedPreviousCard(float targetPosiition, bool triggerEvent)
    {
        float time = 0;

        while (time < timeForCoroutine)
        {
            time += Time.deltaTime;

            previousCard.transform.localPosition = new Vector3(previousCard.transform.localPosition.x, Mathf.SmoothStep(previousCard.transform.localPosition.y, targetPosiition, time), 0);

            yield return null;
        }
        previousCard.transform.localPosition = new Vector3(previousCard.transform.localPosition.x, targetPosiition, 0);
        if (triggerEvent) CardManager.TriggerPreviousPage();
    }
}