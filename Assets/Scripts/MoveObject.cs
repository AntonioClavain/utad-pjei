using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Vector3 movementOffset;

    public AnimationCurve movementClip;

    public bool isLoop;

    public bool isActive;

    private float _elapsedTime;

    private Vector3 _originalPosition;

    private Vector3 _destinationPosition;

    private Vector3 _previousPosition;

    private Collider _collider;

    private Collider[] _colliderResults;
    
    // Start is called before the first frame update
    void Start()
    {
        _originalPosition = transform.position;

        _destinationPosition = _originalPosition + movementOffset;

        _collider = GetComponent<Collider>();

        _colliderResults = new Collider[5];
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            _elapsedTime += Time.deltaTime;
            if(_elapsedTime > movementClip.keys[movementClip.length - 1].time && isLoop)
            {
                _elapsedTime -= movementClip.keys[movementClip.length - 1].time;
            }
            _elapsedTime = Mathf.Clamp(_elapsedTime, 0, movementClip.keys[movementClip.length - 1].time);
            transform.position = Vector3.Lerp(_originalPosition, _destinationPosition, movementClip.Evaluate(_elapsedTime));

            if(Physics.OverlapBoxNonAlloc(transform.position, transform.localScale + Vector3.up * 0.3f, _colliderResults) > 0)
            {
                foreach(Collider col in _colliderResults)
                {
                    if (col != null && col.CompareTag("Player") )
                    {
                        CharacterController charController = col.GetComponent<CharacterController>();
                        if(Vector3.Dot(Vector3.up, col.transform.position - (Vector3.up * charController.height / 2) - transform.position) > 0)
                        {
                            col.transform.position += transform.position - _previousPosition;
                        }
                    }
                }
            }

            _previousPosition = transform.position;
        }
    }
}
