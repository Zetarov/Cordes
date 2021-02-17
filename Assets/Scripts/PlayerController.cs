using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Animator anim;

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
        Debug.Log("PlayInstrumentFollow");
    }

    public void PlayInstrumentKill(InputAction.CallbackContext context)
    {
        Debug.Log("PlayInstrumentKill");
    }
}
