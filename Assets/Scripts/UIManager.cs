using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text scoreText = null;
    [SerializeField] private Text highScoreText = null;
    [SerializeField] private GameObject bonusTexts = null;
    [SerializeField] private GameObject sideWalls = null;
    [SerializeField] private GameObject gameOverPanal = null;

    private Text[] bonusText = null;
    private SpriteRenderer[] sideWall = null;
    private void Awake()
    {
        bonusText = bonusTexts.GetComponentsInChildren<Text>();
        sideWall = GetComponents<SpriteRenderer>(sideWalls);
    }

    public T[] GetComponents<T>(GameObject gameObject)
    {
        T[] arr = gameObject.GetComponentsInChildren<T>();
        T[] returnArr = new T[arr.Length - 1];
        for (int i = 1; i < arr.Length; i++)
        {
            returnArr[i - 1] = arr[i];
        }

        return returnArr;
    }

    public void ActiveGameOVerPanal(bool isActive)
    {
        gameOverPanal.transform.DOScale(isActive ? Vector3.one : Vector3.zero, 0.2f);
    }

    public void ActiveMoveEffect(int num)
    {
        sideWall[num].DOFade(1f, 0.1f).OnComplete(() => sideWall[num].DOFade(0f, 0.1f));
    }

    public void OnClickReStart()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnCkickQuit()
    {
        Application.Quit();
    }

    public void ActiveBonusText(int num, bool isActive)
    {
        bonusText[num].DOFade(isActive ? 1f : 0f, 0.3f);
    }
    public void SetScoreText(int score)
    {
        scoreText.text = string.Format("Score: {0}", score);
        if(PlayerPrefs.GetInt("HighScore") < score)
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScoreText.text = string.Format("HighScore: {0}", PlayerPrefs.GetInt("HighScore"));
        }
    }
}
