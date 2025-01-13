using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalmingWall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<MatchingObject>() != null)
        {
            collision.gameObject.GetComponent<Rigidbody>().velocity *= 0.1f;
            collision.gameObject.GetComponent<Rigidbody>().angularVelocity *= 0.1f;
        }
    }
}
