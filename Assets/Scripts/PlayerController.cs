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
    TypeInstrument instrument;


    void Start()
    {
        
    }

    void Update()
    {
        if (anim.GetInteger("Walk") == 1)
        {
            gameObject.transform.Translate(Vector3.forward * Time.deltaTime * 2.0f);
        }
    }

    void FixedUpdate()
    {
        
    }



    public void Move(InputAction.CallbackContext context)
    {
        Debug.Log("Move");

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
        Debug.Log("PlayInstrumentFollow ");

        if(context.performed)
        {
            anim.SetInteger("Play", (int)instrument + 1);
        }

        else if(context.canceled)
        {
            anim.SetInteger("Play", 0);
        }

        
    }

    public void PlayInstrumentKill(InputAction.CallbackContext context)
    {
        Debug.Log("PlayInstrumentKill");

        if (context.performed)
        {
            anim.SetInteger("Play", (int)instrument + 1);
        }

        else if (context.canceled)
        {
            anim.SetInteger("Play", 0);
        }
    }
}
