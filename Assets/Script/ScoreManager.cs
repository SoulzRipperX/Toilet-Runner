using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TextMeshProUGUI[] scoreTexts;

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

            UpdateAllScoreTexts(displayedScore);

            yield return new WaitForSeconds(0.02f);
        }

        foreach (var text in scoreTexts)
            text.transform.localScale = Vector3.one * 1.2f;

        yield return new WaitForSeconds(0.05f);

        foreach (var text in scoreTexts)
            text.transform.localScale = Vector3.one;
    }

    void UpdateAllScoreTexts(int value)
    {
        string formatted = value.ToString("D6");

        foreach (var text in scoreTexts)
        {
            if (text != null)
                text.text = formatted;
        }
    }

    void UpdateScoreInstant()
    {
        displayedScore = score;
        UpdateAllScoreTexts(displayedScore);
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
