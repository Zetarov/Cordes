using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A debugging class to move a debug character, the final game will use Unity's input system to handle character movement.
/// </summary>
public class CharacterDebugMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;

    [SerializeField]
    private RopeDispenser _captureRopeDispenser = null;

    [SerializeField]
    private RopeDispenser _followRopeDispenser = null;

    [SerializeField]
    private PrefabDispenser _capturePrefabDispenser = null;

    [SerializeField]
    private PrefabDispenser _followPrefabDispenser = null;

    [SerializeField]
    private ParticleSystem _followParticleSystem = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = new Vector2(0f, 0f);
        if(Input.GetKey(KeyCode.Z))
        {
            move.y += 1f;
        }
        if(Input.GetKey(KeyCode.S))
        {
            move.y -= 1f;
        }
        if(Input.GetKey(KeyCode.Q))
        {
            move.x -= 1f;
        }
        if(Input.GetKey(KeyCode.D))
        {
            move.x += 1f;
        }
        move = move.normalized;

        transform.Translate(new Vector3(move.x, 0f, move.y) * _speed * Time.deltaTime);

        bool isCapturing = Input.GetKey(KeyCode.Keypad0);
        bool isMakingFollow = Input.GetKey(KeyCode.Keypad2);

        // Capture
        _captureRopeDispenser.Activated = isCapturing;
        _capturePrefabDispenser.enabled = isCapturing;
        
        // Follow
        _followRopeDispenser.Activated = isMakingFollow;
        if(isMakingFollow)
        {
            _followParticleSystem.Play();
        }
        else
        {
            _followParticleSystem.Stop();
        }
        _followPrefabDispenser.enabled = isMakingFollow;

    }
}
