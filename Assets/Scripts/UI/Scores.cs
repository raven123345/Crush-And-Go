using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerScores pl_one;
    public PlayerScores pl_two;
    private GameObject winnnerText;

    public float timeBetweenScores = 1.5f;

    public GameObject _3DUI;
    public GameObject _3DUIButtons;

    public GameObject[] UILabels;
    public Transform UILabelsParent;

    private float curTime;
    private bool scoreEnabled = false;

    // Update is called once per frame

    private void OnEnable()
    {
        pl_one.TurnOffElements();
        pl_two.TurnOffElements();

        scoreEnabled = false;
        _3DUI.SetActive(true);
        _3DUIButtons.SetActive(false);

        curTime = timeBetweenScores;
        int pl_one_sum = pl_one.SetSum(GameManager.instance.pl_one.curLaps, GameManager.instance.pl_one.score);
        int pl_two_sum = pl_two.SetSum(GameManager.instance.pl_two.curLaps, GameManager.instance.pl_two.score);

        if (pl_one_sum > pl_two_sum && GameManager.instance.pl_one.curLives > 0 && GameManager.instance.pl_two.curLives > 0)
        {
            winnnerText = Instantiate(UILabels[0], UILabelsParent);
            _3DUIButtons.SetActive(true);
            // winnnerText.text = "PLAYER ONE WINS!";
        }
        else if (pl_one_sum < pl_two_sum && GameManager.instance.pl_one.curLives > 0 && GameManager.instance.pl_two.curLives > 0)
        {
            winnnerText = Instantiate(UILabels[1], UILabelsParent);

            _3DUIButtons.SetActive(true);
            // winnnerText.text = "PLAYER TWO WINS!";
        }
        else if (pl_one_sum == pl_two_sum && GameManager.instance.pl_one.curLives > 0 && GameManager.instance.pl_two.curLives > 0)
        {
            winnnerText = Instantiate(UILabels[2], UILabelsParent);
            _3DUIButtons.SetActive(true);
            //  winnnerText.text = "BOTH PLAYERS ARE EQUALS!";
        }

        if (GameManager.instance.pl_one.curLives <= 0)
        {
            winnnerText = Instantiate(UILabels[3], UILabelsParent);
            _3DUIButtons.SetActive(true);
            // winnnerText.text = "PLAYER ONE IS DEAD. PLAYER TWO WINS!";
        }
        else if (GameManager.instance.pl_two.curLives <= 0)
        {
            winnnerText = Instantiate(UILabels[4], UILabelsParent);
            _3DUIButtons.SetActive(true);
            // winnnerText.text = "PLAYER TWO IS DEAD. PLAYER ONE WINS!";
        }
        winnnerText.transform.position = new Vector3(winnnerText.transform.position.x, 200f, -40f);
        winnnerText.SetActive(false);
    }
    void Update()
    {
        curTime -= Time.deltaTime;
        if (curTime <= 0f && !scoreEnabled)
        {
            curTime = timeBetweenScores;
            if (!pl_one.laps.gameObject.activeInHierarchy)
            {
                pl_one.laps.gameObject.SetActive(true);
            }
            else if (!pl_one.points.gameObject.activeInHierarchy)
            {
                pl_one.points.gameObject.SetActive(true);
            }
            else if (!pl_one.sum.gameObject.activeInHierarchy)
            {
                pl_one.sum.gameObject.SetActive(true);
                scoreEnabled = true;
                curTime = timeBetweenScores * 2f;
            }

            if (!pl_two.laps.gameObject.activeInHierarchy)
            {
                pl_two.laps.gameObject.SetActive(true);
            }
            else if (!pl_two.points.gameObject.activeInHierarchy)
            {
                pl_two.points.gameObject.SetActive(true);
            }
            else if (!pl_two.sum.gameObject.activeInHierarchy)
            {
                pl_two.sum.gameObject.SetActive(true);
            }
        }

        if (scoreEnabled && curTime <= 0f)
        {
            winnnerText.gameObject.SetActive(true);
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Quit()
    {
        Application.Quit();
    }

    [System.Serializable]
    public class PlayerScores
    {
        public Text laps;
        public Text points;
        public Text sum;

        public void TurnOffElements()
        {
            laps.gameObject.SetActive(false);
            points.gameObject.SetActive(false);
            sum.gameObject.SetActive(false);
        }

        private int SetLaps(int value)
        {
            laps.text = "LAPS: ................................. " + value.ToString();
            return value;
        }
        private int SetPoints(int value)
        {
            points.text = "POINTS: ................................. " + value.ToString();
            return value;
        }
        public int SetSum(int laps, int points)
        {
            sum.text = "SUMMARY: ................................. " + (SetLaps(laps) + SetPoints(points)).ToString();
            return laps + points;
        }
    }
}
