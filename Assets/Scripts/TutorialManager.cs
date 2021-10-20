using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialManager : MonoSingleTon<TutorialManager>
{
    [SerializeField] RectTransform guideCharacter = null;
    [SerializeField] GameObject textPanal = null;

    Camera mainCam = null;
    private Text messageText = null;
    private void Start()
    {
        mainCam = Camera.main;
        messageText = textPanal.transform.GetChild(1).GetComponent<Text>();
    }
    public void StartTutorial()
    {
        mainCam.transform.DOMoveY(0f, 2f).SetEase(Ease.InOutCubic).SetDelay(1f).OnComplete(() => SpawnGuide());
    }

    private void SpawnGuide()
    {
        guideCharacter.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCirc);
        guideCharacter.DOAnchorPos(new Vector3(317f, -135f, 0f), 0.5f).SetEase(Ease.OutCirc);
        textPanal.transform.DOScaleX(1f, 0.5f).SetEase(Ease.OutCirc).SetDelay(0.5f);
    }


}
