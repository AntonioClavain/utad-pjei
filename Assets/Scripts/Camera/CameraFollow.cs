using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetTransform;

    public float cameraMovementSpeed;

    public Vector3 cameraFollowOffset;

    public enum CameraFollowType { X, Y, XY};

    public CameraFollowType cameraFollowType;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = targetTransform.position + cameraFollowOffset;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movementVector = targetTransform.position + cameraFollowOffset - transform.position;

        if(cameraFollowType == CameraFollowType.X)
        {
            movementVector = Vector3.Project(movementVector, Vector3.right);
        }
        else if(cameraFollowType == CameraFollowType.Y)
        {
            movementVector = Vector3.Project(movementVector, Vector3.up);
        }

        if(movementVector.sqrMagnitude > 1)
        {
            movementVector.Normalize();
        }

        transform.position += movementVector * Time.deltaTime * cameraMovementSpeed;
    }
}
