using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    [SerializeField] float _doorSpeed = 2f;
    [SerializeField] Transform _doorTransform;
    [SerializeField] Collider2D _doorCollider;
    [SerializeField] Transform _doorInteractionTransform;
    [SerializeField] Transform _upperDoor;
    [SerializeField] Transform _lowerDoor;
    [SerializeField] Vector3 _openPosition;
    [SerializeField] float _closeDelay = 3f;

    bool _isDoorOpen = false;
    bool _playerPassedThrough = false;
    private bool _isMoving = false;

    private void Start()
    {
        _openPosition = _doorTransform.position + _openPosition; // Calculate the open position based on the initial position
    }

    public void StartInteraction()
    {
        if (!_isMoving)
        {
            if (!_isDoorOpen)
            {
                _isDoorOpen = true;
                _doorCollider.enabled = false;
                StartCoroutine(OpenDoor());
            }
            else if (_playerPassedThrough)
            {
                _playerPassedThrough = false;
                StartCoroutine(CloseDoor());
            }
        }
    }

    IEnumerator OpenDoor()
    {
        Vector3 targetPosition = _openPosition;
        float step = _doorSpeed * Time.fixedDeltaTime;

        while (Vector3.Distance(_doorTransform.position, targetPosition) > 0.01f)
        {
            _doorTransform.position = Vector3.MoveTowards(_doorTransform.position, targetPosition, step);

            // Move the upper and lower door positions along with the main door
            Vector3 doorOffset = (_doorTransform.position - targetPosition) / 2f;
            _upperDoor.position = _upperDoor.position + doorOffset;
            _lowerDoor.position = _lowerDoor.position + doorOffset;

            yield return new WaitForFixedUpdate();
        }

        Debug.Log("Door opened");

        _playerPassedThrough = false;
        StartCoroutine(CloseDoorAfterDelay());
    }

    IEnumerator CloseDoor()
    {
        Vector3 targetPosition = _doorTransform.position;
        float step = _doorSpeed * Time.fixedDeltaTime;

        while (Vector3.Distance(_doorTransform.position, targetPosition) > 0.01f)
        {
            _doorTransform.position = Vector3.MoveTowards(_doorTransform.position, targetPosition, step);

            // Move the upper and lower door positions along with the main door
            Vector3 doorOffset = (_doorTransform.position - targetPosition) / 2f;
            _upperDoor.position = _upperDoor.position - doorOffset;
            _lowerDoor.position = _lowerDoor.position - doorOffset;

            yield return new WaitForFixedUpdate();
        }

        _isDoorOpen = false;
        _doorCollider.enabled = true;
        Debug.Log("Door closed");
    }

    IEnumerator CloseDoorAfterDelay()
    {
        yield return new WaitForSeconds(_closeDelay);

        Debug.Log("Starting close delay...");
        StartCoroutine(CloseDoor());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_playerPassedThrough)
        {
            Debug.Log("Player detected in interaction zone.");
            _playerPassedThrough = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited interaction zone.");
            _playerPassedThrough = false;
        }
    }

    public void StopInteraction()
    {
        if (_isDoorOpen && !_isMoving)
        {
            _isMoving = true;
        }
    }
}
