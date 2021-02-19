using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatRace : MonoBehaviour
{
    public static int NbrRats = 0;

    public float Speed = 0.01f;

    public ANIMAL_STATE state = ANIMAL_STATE.STATE_IDLE;

    int cptFrames = 0;


    [SerializeField]
    List<PlayerController> players = new List<PlayerController>();


    Vector3 moveDir;
    int cptMove = 0;


    Rigidbody _rb;
    Animator _animator;

    PlayerController target;


    public enum ANIMAL_STATE
    {
        STATE_IDLE,
        STATE_AVOIDING,
        //STATE_FLEEING,
        //STATE_BLOCKED,
        STATE_FOLLOWING
    }



    void Start()
    {
        ++NbrRats;

        moveDir = new Vector3();

        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        players = new List<PlayerController>(GameObject.FindObjectsOfType<PlayerController>());
    }

    void OnDestroy()
    {
        --NbrRats;
    }

    void Update()
    {
        cptFrames++;
    }



    void FixedUpdate()
    {
        bool isPlayerNear = false;
        bool isPlayerFar = true;

        target = null;

        foreach (PlayerController player in players)
        {
            float playerDist = (player.transform.position - transform.position).magnitude;

            isPlayerNear = playerDist < 5.5f || isPlayerNear; // au moins un joueur près
            isPlayerFar = isPlayerFar && playerDist > 7.5f; // les deux joueurs loin

            if(playerDist < 5.5f)
            {
                target = player;

                if(target.IsFollowing)
                {
                    break;
                }
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

            case ANIMAL_STATE.STATE_FOLLOWING:
                MoveFollowing();
                break;
        }
    }



    void MoveIdle()
    {
        //Debug.Log("JUST CHILLING");

        if (cptMove <= 0)
        {
            moveDir = new Vector3(Random.value * 2f - 1f, 0f, Random.value * 2f - 1f).normalized;

            cptMove = Random.Range(0, 450);

        }

        MoveTowardsTarget();
    }

    void MoveAvoiding()
    {
        //Debug.Log("AVOID " + target.name);

        moveDir = (transform.position - target.transform.position).normalized;
        moveDir.y = 0f;

        MoveTowardsTarget(14.0f);
    }


    void MoveFollowing()
    {
        moveDir = -(transform.position - target.transform.position).normalized;
        moveDir.y = 0f;

        if (Vector3.Distance(_rb.position, target.transform.position) > 2.0f)
        {
            MoveTowardsTarget(3.5f);
        }

        else
        {
            //_animator.SetFloat("Speed", 0f);
            MoveTowardsTarget(0.0f);
        }
    }


    void MoveTowardsTarget(float s = 6.5f)
    {
        float angle = Vector2.SignedAngle(Vector2.up, new Vector2(moveDir.x, moveDir.z));

        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, -angle, 0f));

        Debug.DrawLine(gameObject.transform.position, transform.position + moveDir, Color.red);

        Vector3 oldPos = _rb.position;

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

        Vector3 newPos = _rb.position;

        if(Vector3.Distance(oldPos, newPos) < 0.01f)
        {
            cptMove = 0;
            state = ANIMAL_STATE.STATE_IDLE;
        }

    }


    void Die()
    {
        Debug.Log("RIP");

        Destroy(gameObject);
    }

    // on passe les booléens utiles à la machine d'états directement (plutôt que la distance ou autre)
    // pour rester générique
    void CalcState(bool isPlayerNear, bool isPlayerFar)
    {
        if (isPlayerFar || target == null) // oklm
        {
            state = ANIMAL_STATE.STATE_IDLE;
        }

        else if (isPlayerNear) // player trop près : on a pas trop confiance
        {
            if (target.IsFollowing)
            {
                state = ANIMAL_STATE.STATE_FOLLOWING;
            }

            else
            {
                state = ANIMAL_STATE.STATE_AVOIDING;
            }

        }
    }


    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("COLL WITH " + collision.collider.name);

        /*cptMove = 0;
        state = ANIMAL_STATE.STATE_IDLE;*/

        cptMove /= 2;
        moveDir = -moveDir;
    }

}
