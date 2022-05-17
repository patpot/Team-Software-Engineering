using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
        => transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.1f);

    public void OnPointerExit(PointerEventData eventData)
        => transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
}
