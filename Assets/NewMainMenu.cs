using UnityEngine;
using DG.Tweening;

public class NewMainMenu : MonoBehaviour
{
    public Camera FPSCamera;
    public GameObject MainMenu;
    public DialogueManager DialogueManager;
    public CanvasGroup PlayerUI;

    public GameObject TaskDisplay;
    public void Play()
    {
        MainMenu.SetActive(false);

        var seq = DOTween.Sequence();
        seq.Join(FPSCamera.DOFieldOfView(60f, 2f));
        seq.Join(FPSCamera.transform.DOLocalMove(new Vector3(0f, 0.55f, 0f), 2f));
        seq.Join(FPSCamera.transform.DORotate(new Vector3(0f, 0f, 0f), 2f));
        seq.Join(PlayerUI.DOFade(1f, 2f));
        seq.SetEase(Ease.InQuad);
        seq.onComplete += ActivateFPSControls;
    }

    public void ActivateFPSControls()
    {
        UIManager.ActiveUICount--;
        UIManager.UpdateCameraAndCursor();
        TaskDisplay.SetActive(true);

        DialogueManager.ShowDialogue(1000);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
