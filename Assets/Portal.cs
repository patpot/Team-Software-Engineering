using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Portal : MonoBehaviour
{
    public Image GameOverFade;
    public GameObject GameOverContent;
    private bool _isActive;
    private bool _ranOnce;
    public void Activate()
        => _isActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (_isActive && other.gameObject.name == "Player" && !_ranOnce)
        {
            _ranOnce = true;

            GameOverFade.gameObject.SetActive(true);
            var seq = DOTween.Sequence();
            seq.Join(GameOverFade.DOFade(1f, 1f));
            seq.SetEase(Ease.InQuad);
            seq.onComplete += delegate { GameOverContent.SetActive(true); };
        }
    }
}
