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
    PlayerInput inp;

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

    // retour haptique
    List<Gamepad> listVibrating = new List<Gamepad>();



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
            notes.SetIsAttracting(IsFollowing);
            notes.SetIsCapturing(IsKilling);
        }
    }

    void FixedUpdate()
    {
        if(isFollowing)
        {
            VibrateController(0.20f, 0.04f, 0.08f, inp);
        }

        else if(isKilling)
        {
            VibrateController(0.20f, 0.32f, 0.22f, inp);
        }
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

        if(context.performed && !IsKilling)
        {
            anim.SetInteger("Play", (int)instrument + 1);
            IsFollowing = true;
        }

        else if(context.canceled)
        {
            anim.SetInteger("Play", 0);
            IsFollowing = false;
        }

        
    }

    public void PlayInstrumentKill(InputAction.CallbackContext context)
    {
        //Debug.Log("PlayInstrumentKill");

        if (context.performed && !IsFollowing)
        {
            anim.SetInteger("Play", (int)instrument + 1);
            IsKilling = true;
        }

        else if (context.canceled)
        {
            anim.SetInteger("Play", 0);
            IsKilling = false;
        }
    }



    // faire vibrer une ou plusieurs manettes
    public void VibrateController(float time, float motor1, float motor2, PlayerInput inp = null /*bool allControllers = true*/)
    {
        // Rumble the  low - frequency (left) motor at motor1 / 1 speed and the high-frequency
        // (right) motor at motor2 / 1 speed.

        if (inp == null || inp.devices.Count == 0)
        {
            StartCoroutine(VibrateControllerCoroutine(time, motor1, motor2, null));
        }

        else if (inp.devices[0] is Gamepad)
        {
            StartCoroutine(VibrateControllerCoroutine(time, motor1, motor2, (Gamepad)inp.devices[0]));
        }
    }

    IEnumerator VibrateControllerCoroutine(float time, float motor1, float motor2, Gamepad gamepad)
    {
        //Debug.Log("VibrateControllerCoroutine " + Gamepad.all.Count);

        if (gamepad == null)
        {
            List<Gamepad> l = new List<Gamepad>();

            foreach (Gamepad g in Gamepad.all)
            {
                if (!listVibrating.Contains(g))
                {
                    //Debug.Log("RUMBLE");
                    listVibrating.Add(g);
                    l.Add(g);
                    g.SetMotorSpeeds(motor1, motor2);
                }
            }

            yield return new WaitForSeconds(time);

            foreach (Gamepad g in l)
            {
                g.SetMotorSpeeds(0f, 0f);
                listVibrating.Remove(g);
            }
        }

        else // seulement la manette actuelle (TO FIX -> lier joueur / manette)
        {
            //Debug.Log(gamepad);

            if (!listVibrating.Contains(gamepad))
            {
                listVibrating.Add(gamepad);

                gamepad.SetMotorSpeeds(motor1, motor2);
                yield return new WaitForSeconds(time);
                gamepad.SetMotorSpeeds(0f, 0f);

                listVibrating.Remove(gamepad);
            }
        }

    }



    public bool IsFollowing { get => isFollowing; set => isFollowing = value; }
    public bool IsKilling { get => isKilling; set => isKilling = value; }
}
