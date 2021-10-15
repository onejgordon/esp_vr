using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehavior : MonoBehaviour
{
    private string id;


    void Start()
    {

    }

    void Update()
    {

    }

    public string getId() {
        return this.id;
    }

    public void setup() {
        // this.id = wall.id;
    }

    public void positionAndOrient(Vector2 p1, Vector2 p2) {
        Vector2 delta = p1 - p2;
        float theta = -1 * Mathf.Atan2(delta.y, delta.x) * 360 / Mathf.PI / 2;
        Vector2 mean = (p1 + p2)/2.0f;
        gameObject.transform.SetPositionAndRotation(new Vector3(mean.x, gameObject.transform.localScale.y/2, mean.y), Quaternion.Euler(0, theta, 0));

    }

}
