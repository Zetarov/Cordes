using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI labelScore;



    void Awake()
    {
        RatRace.NbrRats = 0;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(labelScore != null)
        {
            labelScore.text = RatRace.NbrRats.ToString("00");
        }
    }
}
