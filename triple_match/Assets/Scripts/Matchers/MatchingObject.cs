using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MatchingObject : MonoBehaviour, IPointerDownHandler
{
    public MatchingType type;
    private bool isClicked;
    public Action<MatchingObject> clickCallback;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isClicked)
        {
            isClicked = true;
            clickCallback(this);
        }
    }

    public void Unclick() { isClicked = false; }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<MatchingObject>() != null) {
            if (gameObject.GetComponent<Rigidbody>() is Rigidbody rb)
            {
                rb.angularVelocity = Vector3.zero;
                rb.velocity = Vector3.zero;
                Debug.Log("calmed down");
            }
        }
    }

}
