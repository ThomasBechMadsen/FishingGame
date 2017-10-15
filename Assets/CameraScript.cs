using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public float dampTime;
    public Transform target;

    Camera camera;
    Vector3 velocity = Vector3.zero;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        if (target)
        {
            Vector3 point = camera.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }
    }

}
