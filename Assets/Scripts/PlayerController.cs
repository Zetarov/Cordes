using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum TypeInstrument
    {
        Guitar,
        Flute
    }

    [SerializeField]
    Animator anim;

    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    PlayerNotesGenerator notes;

    [SerializeField]
    GameMusic music;

    [SerializeField]
    TypeInstrument instrument;

    [SerializeField]
    GameObject instrumentStored;
    [SerializeField]
    GameObject instrumentPlayed;


    bool isFollowing = false;
    bool isKilling = false;


    void Start()
    {
        if (notes != null)
        {
            notes.SetIsAttracting(true);
            notes.SetIsCapturing(true);
        }
    }

    void Update()
    {
        bool walking = anim.GetInteger("Walk") == 1;
        bool playing = anim.GetInteger("Play") > 0;

        if (walking)
        {
            //gameObject.transform.Translate(Vector3.forward * Time.deltaTime * 2.0f);
            rb.MovePosition(rb.position + gameObject.transform.forward * Time.deltaTime * 2.0f);
        }


        instrumentStored.SetActive(!playing);
        instrumentPlayed.SetActive(playing);

        if(instrument == TypeInstrument.Guitar)
        {
            if (playing)
                music.PlayTheme1();
            else
                music.StopTheme1();
        }

        else if (instrument == TypeInstrument.Flute)
        {
            if (playing)
                music.PlayTheme2();
            else
                music.StopTheme2();
        }


        if(notes != null)
        {
            notes.SetIsAttracting(isFollowing);
            notes.SetIsCapturing(isKilling);
        }
    }

    void FixedUpdate()
    {
        
    }



    public void Move(InputAction.CallbackContext context)
    {
        //Debug.Log("Move");

        Vector2 direction = context.ReadValue<Vector2>();

        if(Mathf.Abs(direction.x) > 0.25f || Mathf.Abs(direction.y) > 0.25f)
        {
            anim.SetInteger("Walk", 1);

            float angle = Vector2.SignedAngle(Vector2.up, direction);

            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, -angle, 0f));

            //gameObject.transform.Translate(Vector3.forward * Time.deltaTime * 5.0f);
        }

        else
        {
            anim.SetInteger("Walk", 0);
        }
    }

    public void PlayInstrumentFollow(InputAction.CallbackContext context)
    {
        //Debug.Log("PlayInstrumentFollow ");

        if(context.performed && !isKilling)
        {
            anim.SetInteger("Play", (int)instrument + 1);
            isFollowing = true;
        }

        else if(context.canceled)
        {
            anim.SetInteger("Play", 0);
            isFollowing = false;
        }

        
    }

    public void PlayInstrumentKill(InputAction.CallbackContext context)
    {
        //Debug.Log("PlayInstrumentKill");

        if (context.performed && !isFollowing)
        {
            anim.SetInteger("Play", (int)instrument + 1);
            isKilling = true;
        }

        else if (context.canceled)
        {
            anim.SetInteger("Play", 0);
            isKilling = false;
        }
    }
}
