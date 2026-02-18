using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreValueText;

    private int score;
    private int displayedScore;
    private Coroutine scoreCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateScoreInstant();
    }

    public void AddScore(int value)
    {
        score += value;

        if (scoreCoroutine != null)
            StopCoroutine(scoreCoroutine);

        scoreCoroutine = StartCoroutine(AnimateScore());
    }

    IEnumerator AnimateScore()
    {
        while (displayedScore < score)
        {
            displayedScore += Mathf.CeilToInt((score - displayedScore) * 0.2f);

            if (displayedScore > score)
                displayedScore = score;

            scoreValueText.text = displayedScore.ToString("D6");

            yield return new WaitForSeconds(0.02f);
        }

        scoreValueText.transform.localScale = Vector3.one * 1.2f;
        yield return new WaitForSeconds(0.05f);
        scoreValueText.transform.localScale = Vector3.one;
    }

    void UpdateScoreInstant()
    {
        displayedScore = score;
        scoreValueText.text = displayedScore.ToString("D6");
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
