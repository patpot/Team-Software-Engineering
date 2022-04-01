using UnityEngine;
using UnityEngine.EventSystems;

public class UIScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    void Update()
    {
        if (!IsMouseOverUI())
        {
            LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0.1f);
        }
    }
    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0.1f);
    }
}
