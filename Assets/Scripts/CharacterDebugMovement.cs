using Ludiq.OdinSerializer.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A debugging class to move a debug character, the final game will use Unity's input system to handle character movement.
/// </summary>
public class CharacterDebugMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;

    [Header("Capture related")]
    [SerializeField]
    private RopeDispenser _captureRopeDispenser = null;

    [SerializeField]
    private NotePrefabDispenser _capturePrefabDispenser = null;

    [SerializeField]
    private List<CutableTrailRenderer> _captureTrailRenderers = new List<CutableTrailRenderer>();

    [Header("Attract related")]
    [SerializeField]
    private RopeDispenser _attractRopeDispenser = null;

    [SerializeField]
    private NotePrefabDispenser _attractPrefabDispenser = null;

    [SerializeField]
    private ParticleSystem _attractParticleSystem = null;

    bool _isCapturing = false;
    bool _isAttracting = false;
    bool _isMoving = false;

    Vector2 _direction;

    // Start is called before the first frame update
    void Start()
    {
        _captureRopeDispenser.ClosedContour.AddListener(OnCaptureClosedContour);
    }

    private void Update()
    {
        if(_isMoving)
            transform.Translate(new Vector3(_direction.x, 0f, _direction.y) * _speed * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _direction = context.ReadValue<Vector2>();

        _isMoving = _direction.sqrMagnitude > 0.1f;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        SetIsCapturing(context.ReadValueAsButton());
    }

    protected virtual void OnCaptureClosedContour(float time)
    {
        _capturePrefabDispenser.FadeAllNotes();
        _captureTrailRenderers.ForEach(cutableTrailRenderer => cutableTrailRenderer.Shorten(0f, endEmitting: false));
    }

    protected virtual void SetIsCapturing(bool value)
    {
        if (_isCapturing == value)
            return;

        _isCapturing = value;
        if(_isCapturing)
        {
            _captureTrailRenderers.ForEach(el => el.TrailRenderer.emitting = true);
            _capturePrefabDispenser.enabled = true;
            _captureRopeDispenser.Activated = true;
        }
        else
        {
            _captureTrailRenderers.ForEach(el => el.Shorten(0f, endEmitting: true));
            _capturePrefabDispenser.enabled = false;
            _capturePrefabDispenser.FadeAllNotes();
            _captureRopeDispenser.Activated = false;
        }
    }

    protected virtual void SetIsAttracting(bool value)
    {
        if (_isAttracting == value)
            return;

        _isAttracting = value;
        if (_isAttracting)
        {
            _attractParticleSystem.Play();
            _attractPrefabDispenser.enabled = true;
            _attractRopeDispenser.Activated = true;
        }
        else
        {
            _attractParticleSystem.Stop();
            _attractPrefabDispenser.enabled = false;
            _attractRopeDispenser.Activated = false;
        }
    }
}
