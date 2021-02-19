using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    Transform parentRats;
    [SerializeField]
    List<GameObject> prefabsRats = new List<GameObject>();
    [SerializeField]
    List<GameObject> generatorsRats = new List<GameObject>();
    [SerializeField]
    float minTimeBetweenRats = 5.0f;
    [SerializeField]
    float maxTimeBetweenRats = 12.0f;

    [SerializeField]
    DOTweenAnimation animScore;
    [SerializeField]
    DOTweenAnimation animRat;

    [SerializeField]
    TextMeshProUGUI labelScore;

    [SerializeField]
    [ColorUsageAttribute(true, true)]
    Color colorWinning = Color.green;
    [SerializeField]
    [ColorUsageAttribute(true, true)]
    Color colorLosing = Color.red;

    [SerializeField]
    GameObject victory;
    [SerializeField]
    GameObject fail;
    [SerializeField]
    GameObject fireworks;

    //[SerializeField]
    int scoreWin = 0;
    [SerializeField]
    int scoreLose = 20;


    bool gameOver = false;

    int cptFrames = 0;
    float cptTime = 0f;

    bool isManagingScoreDisplay = false;


    void Awake()
    {
        Instance = this;

        RatRace.NbrRats = 0;
    }

    void Start()
    {
        #if !UNITY_EDITOR
        UnityEngine.Cursor.visible = false;
        #endif

        StartCoroutine(RatsGenerator());
    }

    void OnDestroy()
    {
        Instance = null;   
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(labelScore != null)
        {
            if (RatRace.NbrRats.ToString("00") != labelScore.text && !isManagingScoreDisplay)
            {
                StartCoroutine(ManageScoreDisplay());
            }
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



    IEnumerator ManageScoreDisplay()
    {
        isManagingScoreDisplay = true;

        /*if(int.Parse(labelScore.text) > RatRace.NbrRats) // on a tu� des rats
        {
            ScreenShake(1.25f, new Vector3(0.30f, 0.25f, 0.30f));
        }*/

        float t = (float)RatRace.NbrRats / (float)scoreLose;

        if (animScore != null)
        {
            animScore.DORestart();
        }

        if (animRat != null)
        {
            animRat.DORestart();
        }

        yield return new WaitForSeconds(0.75f);

        labelScore.text = RatRace.NbrRats.ToString("00");
        labelScore.color = Color.Lerp(colorWinning, colorLosing, t).WithA(labelScore.color.a);

        isManagingScoreDisplay = false;
    }


    IEnumerator Win()
    {
        Debug.Log("WIN");

        gameOver = true;

        victory.SetActive(true);

        yield return new WaitForSeconds(0.75f);
        fireworks.SetActive(true);
        MusicManager.main.PlayEffect("Victory", 3.0f);
        ScreenShake(2.00f, new Vector3(0.70f, 0.20f, 0.70f));
        yield return new WaitForSeconds(4.25f);

        SceneManager.LoadScene(0);

        yield break;
    }

    IEnumerator Lose()
    {
        Debug.Log("LOSE");

        gameOver = true;

        fail.SetActive(true);

        yield return new WaitForSeconds(0.75f);
        ScreenShake(2.00f, new Vector3(0.70f, 0.20f, 0.70f));
        MusicManager.main.PlayEffect("Fail", 3.0f);
        yield return new WaitForSeconds(4.25f);

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

            MusicManager.main.PlayEffect("Pop");
        }
    }



    public void ScreenShake(float time, Vector3 strength)
    {
        Camera.main.transform.DOShakePosition(time, strength);
    }
}
