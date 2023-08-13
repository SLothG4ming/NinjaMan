using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    [SerializeField] float _doorSpeed = 2f;
    [SerializeField] Transform _doorTransform;
    [SerializeField] Collider2D _doorCollider;
    [SerializeField] Transform _doorInteractionTransform;
    [SerializeField] SpriteRenderer _upperDoorSpriteRenderer;
    [SerializeField] SpriteRenderer _lowerDoorSpriteRenderer;
    [SerializeField] float _closeDelay = 3f;

    bool _isDoorOpen = false;
    bool _playerPassedThrough = false;
    private bool _isMoving = false;

    private Vector3 _originalUpperSpritePosition;
    private Vector3 _originalLowerSpritePosition;
    private Vector3 _openUpperSpritePosition;
    private Vector3 _openLowerSpritePosition;

    private void Start()
    {
        _originalUpperSpritePosition = _upperDoorSpriteRenderer.transform.localPosition;
        _originalLowerSpritePosition = _lowerDoorSpriteRenderer.transform.localPosition;
        _openUpperSpritePosition = _originalUpperSpritePosition + new Vector3(0.3f, 0f, 0f);
        _openLowerSpritePosition = _originalLowerSpritePosition + new Vector3(0.3f, 0f, 0f);
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
        Vector3 targetUpperPosition = _openUpperSpritePosition;
        Vector3 targetLowerPosition = _openLowerSpritePosition;
        float timing = _doorSpeed * Time.fixedDeltaTime;

        while (Vector3.Distance(_upperDoorSpriteRenderer.transform.localPosition, targetUpperPosition) > 0.01f)
        {
            Vector3 upperSpritePosition = _upperDoorSpriteRenderer.transform.localPosition;
            upperSpritePosition = Vector3.MoveTowards(upperSpritePosition, targetUpperPosition, timing);
            _upperDoorSpriteRenderer.transform.localPosition = upperSpritePosition;

            Vector3 lowerSpritePosition = _lowerDoorSpriteRenderer.transform.localPosition;
            lowerSpritePosition = Vector3.MoveTowards(lowerSpritePosition, targetLowerPosition, timing);
            _lowerDoorSpriteRenderer.transform.localPosition = lowerSpritePosition;

            yield return new WaitForFixedUpdate();
        }

        Debug.Log("Door opened");

        _playerPassedThrough = false;
        StartCoroutine(CloseDoorAfterDelay());
    }

    IEnumerator CloseDoor()
    {
        Vector3 targetUpperPosition = _originalUpperSpritePosition;
        Vector3 targetLowerPosition = _originalLowerSpritePosition;
        float timing = _doorSpeed * Time.fixedDeltaTime;

        while (Vector3.Distance(_upperDoorSpriteRenderer.transform.localPosition, targetUpperPosition) > 0.01f)
        {
            Vector3 upperSpritePosition = _upperDoorSpriteRenderer.transform.localPosition;
            upperSpritePosition = Vector3.MoveTowards(upperSpritePosition, targetUpperPosition, timing);
            _upperDoorSpriteRenderer.transform.localPosition = upperSpritePosition;

            Vector3 lowerSpritePosition = _lowerDoorSpriteRenderer.transform.localPosition;
            lowerSpritePosition = Vector3.MoveTowards(lowerSpritePosition, targetLowerPosition, timing);
            _lowerDoorSpriteRenderer.transform.localPosition = lowerSpritePosition;

            yield return new WaitForFixedUpdate();
        }

        Debug.Log("Door closed");

        _isDoorOpen = false;
        _doorCollider.enabled = true;
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
