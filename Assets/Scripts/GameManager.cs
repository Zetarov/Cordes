using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Transform parentRats;
    [SerializeField]
    List<GameObject> prefabsRats = new List<GameObject>();
    [SerializeField]
    List<GameObject> generatorsRats = new List<GameObject>();
    [SerializeField]
    float minTimeBetweenRats = 5.0f;
    [SerializeField]
    float maxTimeBetweenRats = 20.0f;

    [SerializeField]
    DOTweenAnimation animScore;
    [SerializeField]
    DOTweenAnimation animRat;

    [SerializeField]
    TextMeshProUGUI labelScore;

    [SerializeField]
    Color colorWinning = Color.green;
    [SerializeField]
    Color colorLosing = Color.red;

    //[SerializeField]
    int scoreWin = 0;
    [SerializeField]
    int scoreLose = 20;


    bool gameOver = false;

    int cptFrames = 0;
    float cptTime = 0f;


    void Awake()
    {
        RatRace.NbrRats = 0;
    }

    void Start()
    {
        StartCoroutine(RatsGenerator());
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(labelScore != null)
        {
            float t = (float)RatRace.NbrRats / (float)scoreLose;

            if(RatRace.NbrRats.ToString("00") != labelScore.text)
            {
                if (animScore != null)
                {
                    animScore.DORestart();
                }

                if(animRat != null)
                {
                    animRat.DORestart();
                }
            }

            labelScore.text = RatRace.NbrRats.ToString("00");
            labelScore.color = Color.Lerp(colorWinning, colorLosing, t);
        }


        if (!gameOver)
        {
            if (RatRace.NbrRats <= scoreWin)
            {
                StartCoroutine(Win());
            }

            else if (RatRace.NbrRats >= scoreLose)
            {
                StartCoroutine(Lose());
            }
        }

        ++cptFrames;
        cptTime += Time.deltaTime;
    }


    IEnumerator Win()
    {
        Debug.Log("WIN");

        gameOver = true;

        SceneManager.LoadScene(0);

        yield break;
    }

    IEnumerator Lose()
    {
        Debug.Log("LOSE");

        gameOver = true;

        SceneManager.LoadScene(0);

        yield break;
    }


    IEnumerator RatsGenerator()
    {
        while(true)
        {
            int typeRat = Random.Range(0, prefabsRats.Count);
            int spawner = Random.Range(0, generatorsRats.Count);
            float timeWaiting = Random.Range(minTimeBetweenRats, maxTimeBetweenRats);

            yield return new WaitForSeconds(timeWaiting);

            GameObject newRat = GameObject.Instantiate(prefabsRats[typeRat], parentRats);
            newRat.transform.position = generatorsRats[spawner].transform.position.WithY(0f);
        }
    }
}
