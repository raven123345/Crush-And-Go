using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float gameLength = 20f;

    public Canvas GamePlayUI;
    public Canvas end_screen;

    [SerializeField]
    GameObject pl_one_car;
    [SerializeField]
    GameObject pl_two_car;
    public Player pl_one;
    public Player pl_two;

    public Transform[] checkPoints;

    public AudioClip[] sound_effects;

    public AudioClip[] soundtracks;

    internal bool gameStarted;

    AudioSource audio_source;

    [SerializeField]
    GameObject mini_map;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        pl_one.player = Instantiate(pl_one_car, pl_one.spawner_point);
        pl_two.player = Instantiate(pl_two_car, pl_two.spawner_point);

        pl_one.setInitLives();
        pl_two.setInitLives();

        pl_one.setInitScore();
        pl_two.setInitScore();

        pl_one.setCarController();
        pl_two.setCarController();

        pl_one.setCheckpoints(checkPoints.Length, true);
        pl_two.setCheckpoints(checkPoints.Length, true);

        pl_one.relatedCamera_shake = pl_one.relatedCamera.GetComponent<CameraShake>();
        pl_two.relatedCamera_shake = pl_two.relatedCamera.GetComponent<CameraShake>();

        pl_one.car.SetCanvasCamera(pl_one.relatedCamera, pl_two.relatedCamera);
        pl_two.car.SetCanvasCamera(pl_two.relatedCamera, pl_one.relatedCamera);

        gameStarted = false;

        audio_source = GetComponent<AudioSource>();
        audio_source.clip = soundtracks[Random.Range(0, soundtracks.Length)];
        audio_source.Play();

        if (mini_map.activeInHierarchy)
        {
            mini_map.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (pl_one.player)
            PlayerManager(pl_one);
        if (pl_two.player)
            PlayerManager(pl_two);

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (gameStarted && !mini_map.activeInHierarchy)
        {
            mini_map.SetActive(true);
        }

    }

    void PlayerManager(Player pl)
    {
        if (pl.curLives <= 0)
        {
            end_screen.gameObject.SetActive(true);
            GamePlayUI.gameObject.SetActive(false);

            gameStarted = false;

            pl.car.selfDestroy();
        }

        if (pl.curHeapthPoints <= 0)
        {
            ResetCar(pl);
        }
    }
    public void ResetCar(Player pl)
    {
        ChangeHealthPoints(pl, pl.initHP, false);
        ChangeLives(pl, -1);
        pl.player.transform.position = pl.spawner_point.position;
        pl.player.transform.rotation = pl.spawner_point.rotation;

        if (!pl.car)
        {
            pl.setCarController();
        }
        pl.car.StopCar();
    }
    public void AddScore(Player pl, int value)
    {
        pl.score += value;
        pl.scoreTxt.text = "Score: " + pl.score;
    }

    public void ChangeLives(Player pl, int value)
    {
        pl.curLives += value;
        pl.livesTxt.text = "Lives: " + pl.curLives;
    }

    public void ChangeHealthPoints(Player pl, int value, bool addShake)
    {
        pl.curHeapthPoints += value;
        pl.HPSlider.value = pl.curHeapthPoints;
        if (addShake)
            pl.relatedCamera_shake.Shake();
    }

    public void ChangeLaps(Player pl)
    {
        pl.curLaps++;
        pl.lapsTxt.text = "LAPS " + pl.curLaps.ToString();
        AddScore(pl, 5);
        ChangeHealthPoints(pl, 1, false);
    }

    public void CheckPointAchieved(Player pl, int checkPointPassed)
    {
        if (checkPointPassed > 0)
        {
            if (pl.checkpoints[checkPointPassed - 1] == true)
            {

                pl.checkpoints[checkPointPassed] = true;
            }
        }
        else
        {
            if (pl.checkpoints[pl.checkpoints.Count - 1] == true)
            {
                // цяла обиколка

                ChangeLaps(pl);
                pl.setCheckpoints(checkPoints.Length, false);
            }
            pl.checkpoints[0] = true;
        }
    }

    public void PlaySoundFX(int index)
    {
        audio_source.PlayOneShot(sound_effects[index]);
    }
}

[System.Serializable]
public class Player
{
    [HideInInspector]
    public int score = 0;
    [HideInInspector]
    public GameObject player;

    public Transform spawner_point;

    public Camera relatedCamera;
    internal CameraShake relatedCamera_shake;

    public Text lapsTxt;
    public Text livesTxt;
    public Text scoreTxt;

    internal Slider HPSlider;

    public int initLives = 5;
    public int initHP = 5;

    [HideInInspector]
    public int curLives;
    [HideInInspector]
    public int curLaps = 0;
    [HideInInspector]
    public int curHeapthPoints;
    [HideInInspector]
    public CarController car;
    // [HideInInspector]
    public List<bool> checkpoints;

    public void setCarController()
    {
        if (player)
        {
            car = player.GetComponent<CarController>();
            car.camera = relatedCamera;
        }
    }

    public void setHP()
    {
        if (!car)
        {
            setCarController();
        }
        else
        {
            curHeapthPoints = initHP;
            car.health_slider.maxValue = initHP;
            car.health_slider.value = initHP;
        }
    }
    public void setInitLives()
    {
        curLives = initLives;
        livesTxt.text = "Lives: " + initLives;
    }
    public void setInitScore()
    {
        scoreTxt.text = "Score: " + score;
    }
    public void setCheckpoints(int count, bool initialize)
    {
        checkpoints = new List<bool>();

        for (int i = 0; i < count; i++)
        {
            checkpoints.Add(false);
        }
        if (initialize)
            checkpoints[0] = true;
    }
}