using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

public class box : MonoBehaviour
{
    public Rigidbody2D objectRigidbody2D;
    public GameObject restartSpawnPrefab;
    public GameObject prefab;
    public GameObject start;
    public GameObject over;
    public GameObject Pause;
    public Button pauseButton;
    public Text score;
    public Text Hscore;
    public Text overScore;
    public LeaderboardUI leaderboard;
    public AdsInitializer adManager;
    public Button topscore;
    public Camera maincamera;
    private float speed = 30f;
    private bool touchDetected = false;
    private bool gamePaused=false;
    private bool firstT=true;
    private int sco=-1;
    private const string HighScoreKey = "HighScore";
    private int yPosition = 0;
    private Queue<GameObject> spawnedObjects = new Queue<GameObject>();
    public int numberInter=0;

    IEnumerator ResetCameraNextFrame()
    {
        yield return null; // wait for one frame
        maincamera.transform.position = new Vector3(0, 0, -10);
    }
    
    public void restart()
    {
        objectRigidbody2D.velocity = Vector2.zero;
        objectRigidbody2D.transform.position = Vector3.zero;
        objectRigidbody2D.transform.rotation = Quaternion.Euler(0, 0, 45);
        objectRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        objectRigidbody2D.GetComponent<Collider2D>().enabled = true;

        // Reset camera
        StartCoroutine(ResetCameraNextFrame());  

        // Clear old prefabs
        foreach (GameObject obj in spawnedObjects)
        {
            Destroy(obj);
        }
        spawnedObjects.Clear();
        yPosition = 0;

        // Reset score
        sco = -1;
        score.text = "0";
        Hscore.text = "High Score: " + PlayerPrefs.GetInt(HighScoreKey, 0).ToString();
        overScore.text = "";

        // Reset flags and UI
        start.SetActive(true);
        topscore.gameObject.SetActive(true);
        over.SetActive(false);
        Vector3 spawnPosition_restart = new Vector3(0, 0, 0);
        Instantiate(restartSpawnPrefab, spawnPosition_restart, Quaternion.identity);
        objectRigidbody2D.gravityScale = 0;
        StartCoroutine(UnpauseNextFrame());
        numberInter++;
        if (numberInter >= 5)
        {
            adManager.ShowInterAd();
            numberInter = 0;
        }
    }

    IEnumerator UnpauseNextFrame()
    {
        yield return null; // wait 1 frame
        gamePaused = false;
    }
    public void reward()
    {
        objectRigidbody2D.velocity = Vector2.zero;
        objectRigidbody2D.transform.position = Vector3.zero;
        objectRigidbody2D.transform.rotation = Quaternion.Euler(0, 0, 45);
        objectRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        objectRigidbody2D.GetComponent<Collider2D>().enabled = true;

        // Reset camera
        StartCoroutine(ResetCameraNextFrame());

        // Clear old prefabs
        foreach (GameObject obj in spawnedObjects)
        {
            Destroy(obj);
        }
        spawnedObjects.Clear();
        yPosition = 0;
        sco--;
        touchDetected = false;
        gamePaused =true;
        over.SetActive(false);
        Vector3 spawnPosition_restart = new Vector3(0, 0, 0);
        Instantiate(restartSpawnPrefab, spawnPosition_restart, Quaternion.identity);
        touchDetected = true;
        objectRigidbody2D.gravityScale = 0;
        Pause.SetActive(true);
    }

    public void pause()
    {
        int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        if (sco > highScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, sco);
            PlayerPrefs.Save();
        }
        Time.timeScale = 0f;
        Pause.SetActive(true);
    }
    public void resume()
    {
        Time.timeScale = 1f;
        objectRigidbody2D.gravityScale = 1;
        gamePaused=false;
        Pause.SetActive(false);
    }


    void Start()
    {
        Hscore.text= "High Score: " +PlayerPrefs.GetInt(HighScoreKey, 0).ToString();
        Application.targetFrameRate = 120;
    }
    IEnumerator wait1()
    {
        yield return new WaitForSeconds(.3f);
        gamePaused = false;
    }
    void LateUpdate()
    {
        if (firstT)
        {
            gamePaused = true;
            StartCoroutine(wait1());
            firstT = false;
        }
        if (gamePaused) return;
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            return;
        if (!touchDetected && Input.touchCount > 0&& Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchDetected = true;
            objectRigidbody2D.gravityScale = 1;
            start.SetActive(false);
            topscore.gameObject.SetActive(false);
            pauseButton.interactable = true;
        }

        if (touchDetected && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Vector3 touchPosition = maincamera.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0;
            if (touchPosition.x < 0)
            {
                MoveObject(new Vector2(-0.35f, 1f).normalized);
            }
            else
            {
                MoveObject(new Vector2(0.35f, 1f).normalized);
            }
        }
        float difference=maincamera.transform.position.y - objectRigidbody2D.position.y;
        if ( difference< 0){
            Vector3 targetPosition = new Vector3(maincamera.transform.position.x, maincamera.transform.position.y+Mathf.Abs(difference), maincamera.transform.position.z);
            maincamera.transform.position = Vector3.Lerp(maincamera.transform.position, targetPosition, speed);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform") && !gamePaused)
        {
            gamePaused = true;
            touchDetected = false;
            pauseButton.interactable = false;
            int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
            //PlayerPrefs.DeleteKey(HighScoreKey);
            PlayerPrefs.Save();
            if (sco > highScore)
            {
                PlayerPrefs.SetInt(HighScoreKey, sco);
                PlayerPrefs.Save();
                leaderboard.SubmitScore(sco);
            }
            objectRigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
            objectRigidbody2D.GetComponent<Collider2D>().enabled = false;
            StartCoroutine(WaitFun());
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        sco++;
        SpawnPrefab();
        score.text = sco.ToString();
        Destroy(other.gameObject);
    }
    private IEnumerator WaitFun()
    {
        objectRigidbody2D.velocity = new Vector2(0,-1) * speed;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            transform.Rotate(Vector3.forward * 4000 * Time.deltaTime);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        over.SetActive(true);
        objectRigidbody2D.gravityScale = 0f;
        objectRigidbody2D.velocity = Vector3.zero;
        overScore.text = "Your Score: " + sco.ToString() + "\nHigh Score: " + PlayerPrefs.GetInt(HighScoreKey, 0).ToString();
    }

    void MoveObject(Vector2 direction)
    {
        objectRigidbody2D.velocity = direction * speed;
    }

    private void SpawnPrefab()
    {
        yPosition += 26;
        Vector3 spawnPosition = new Vector3(0, yPosition, 0);
        GameObject newObject = Instantiate(prefab, spawnPosition, Quaternion.identity);

        float t = Mathf.Clamp01(sco / 200f);
        t = Mathf.Sqrt(t);

        float k=Mathf.Lerp(3f, 5f, t);

        float randomX = Random.Range(-k, k);
        newObject.transform.GetChild(0).localPosition = new Vector2(randomX, 0);

        k=Mathf.Lerp(1.0f, 0.3f, t);
        newObject.transform.GetChild(0).localScale = new Vector3(k, 1f, 1f);

        float randomx = Random.Range(randomX - 2f, randomX + 2f);
        newObject.transform.GetChild(1).localPosition = new Vector2(randomx, 7f);

        randomx = Random.Range(randomX - 3f, randomX + 3f);
        newObject.transform.GetChild(2).localPosition = new Vector2(randomx, 19f);

        if (spawnedObjects.Count >= 3)
        {
            GameObject oldestObject = spawnedObjects.Dequeue();
            Destroy(oldestObject);
        }

        spawnedObjects.Enqueue(newObject);
    }
}