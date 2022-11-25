using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        var step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, endPosition, step);

        if(Vector3.Distance(transform.position, endPosition) < 0.001f)
        {
            endPosition *= -1.0f;
        }
    }
}
