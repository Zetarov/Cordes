using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatRace : MonoBehaviour
{
    public float Speed = 0.01f;

    public ANIMAL_STATE state = ANIMAL_STATE.STATE_IDLE;

    int cptFrames = 0;


    [SerializeField]
    List<GameObject> players = new List<GameObject>();


    Vector3 moveDir;
    int cptMove = 0;


    Rigidbody _rb;
    Animator _animator;

    GameObject target;


    public enum ANIMAL_STATE
    {
        STATE_IDLE,
        STATE_AVOIDING,
        STATE_FLEEING,
        STATE_BLOCKED
    }



    void Start()
    {
        moveDir = new Vector3();

        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }



    void Update()
    {
        cptFrames++;
    }



    void FixedUpdate()
    {
        bool isPlayerNear = false;
        bool isPlayerFar = false;

        foreach (GameObject player in players)
        {
            float playerDist = (player.transform.position - transform.position).magnitude;

            isPlayerNear = playerDist < 2.5f || isPlayerNear; // au moins un joueur près
            isPlayerFar = playerDist > 6.2f; // les deux joueurs loin

            if(playerDist < 2.5f)
            {
                target = player;
            }
        }

        CalcState(isPlayerNear, isPlayerFar);

        //Debug.Log(playerDist);


        switch (state)
        {
            case ANIMAL_STATE.STATE_IDLE:
                MoveIdle();
                break;


            case ANIMAL_STATE.STATE_AVOIDING:
                MoveAvoiding();
                break;

            case ANIMAL_STATE.STATE_FLEEING:
                MoveFleeing();
                break;

            case ANIMAL_STATE.STATE_BLOCKED:
                _animator.SetFloat("Speed", 0f);
                break;
        }

        //_rb.velocity = new Vector2();

    }



    void MoveIdle()
    {
        Debug.Log("JUST CHILLING");

        //float targetDist = (moveTarget - transform.position).magnitude;

        //if(targetDist < 0.1f && cptFrames % 180 == 0)
        if (cptMove <= 0)
        {
            float newDirX = Random.Range(-1.0f, 1.0f);
            float newDirY = Random.Range(-1.0f, 1.0f);

            //moveDir = transform.position + new Vector3(newX, newY, 0f);

            moveDir = new Vector3(newDirX, 0f, newDirY).normalized;
            cptMove = Random.Range(0, 180);

        }

        MoveTowardsTarget();
    }

    void MoveFleeing()
    {
        Debug.Log("FLEE");

        moveDir = (transform.position - target.transform.position).normalized;
        moveDir.y = 0f;

        MoveTowardsTarget(6.0f);
    }

    void MoveAvoiding()
    {
        Debug.Log("AVOID " + target.name);

        //moveTarget = transform.position + (transform.position - player.transform.position).normalized;

        moveDir = (transform.position - target.transform.position).normalized;
        moveDir.y = 0f;

        MoveTowardsTarget(6.0f);
    }


    void MoveTowardsTarget(float s = 2.0f)
    {
        // en fait on va limiter à 4 directions

        /*if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y)) // déplacement horizontal
        {
            //_spriteRenderer.flipX = (moveDir.x < 0);
            if (_animator.GetInteger("dir") != 3) _animator.SetInteger("dir", 3);


            moveDir.y = 0;
        }

        else // déplacement vertical
        {
            if (moveDir.y > 0) _animator.SetInteger("dir", 1);
            else if (_animator.GetInteger("dir") != 2) _animator.SetInteger("dir", 2);

            moveDir.x = 0;
        }*/


        float angle = Vector2.SignedAngle(Vector2.up, new Vector2(moveDir.x, moveDir.z));

        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, -angle, 0f));

        if (s > 0.01f)
        {
            _animator.SetFloat("Speed", s);
            _rb.MovePosition(transform.position + moveDir * Speed * s);
        }

        else
        {
            _animator.SetFloat("Speed", 0f);
        }

        --cptMove;
    }


    void Die()
    {
        Debug.Log("RIP");

        /*if (Type == ANIMAL_TYPE.TYPE_WOLF) SoundSystem.Instance.PlayCriToutou();
        else SoundSystem.Instance.PlayCriLapinou();*/

        Destroy(gameObject);
    }

    // on passe les booléens utiles à la machine d'états directement (plutôt que la distance ou autre)
    // pour rester générique
    void CalcState(bool isPlayerNear, bool isPlayerFar)
    {
        if (isPlayerFar) // oklm
        {
            state = ANIMAL_STATE.STATE_IDLE;
        }

        /*else if (isPlayerBiting) // on est mordu donc québlo
        {
            //Debug.Log("OUILLE");
            state = ANIMAL_STATE.STATE_BLOCKED;
        }*/

        /*else if (state == ANIMAL_STATE.STATE_ATTACKING) // seul moyen de sortir du mode attaque = player trop loin ou se faire mordre
        {

        }*/

        /*else if (isPlayerAttacking) // il y a une agression pas trop loin (moins loin que far) :o
        {
            if (Type == ANIMAL_TYPE.TYPE_RABBIT) // lapin : on court comme un lapin
            {
                state = ANIMAL_STATE.STATE_FLEEING;
            }

            else // loup : on attaque aussi
            {
                state = ANIMAL_STATE.STATE_ATTACKING;

                SoundSystem.Instance.PlayWolfAttack();
            }
        }*/

        else if (isPlayerNear) // player trop près : on a pas trop confiance
        {
            if (state == ANIMAL_STATE.STATE_BLOCKED) // on vient d'être relaché
            {
                //state = ANIMAL_STATE.STATE_FLEEING;

                //if (Type == ANIMAL_TYPE.TYPE_RABBIT)
                {
                    state = ANIMAL_STATE.STATE_FLEEING;
                }

                /*else
                {
                    state = ANIMAL_STATE.STATE_ATTACKING;
                }*/
            }

            else
            {
                state = ANIMAL_STATE.STATE_AVOIDING;
            }
        }

        else
        {
            state = ANIMAL_STATE.STATE_IDLE;
        }
    }

}
