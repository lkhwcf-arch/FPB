using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class PointerOutlineHighlight : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Color highlightColor = Color.red;
    [SerializeField] private Vector2 outlineDistance = new(3f, -3f);

    private Outline outline;
    private bool pointerInside;

    private void Awake()
    {
        if (!TryGetComponent(out outline))
        {
            outline = gameObject.AddComponent<Outline>();
        }

        outline.effectColor = highlightColor;
        outline.effectDistance = outlineDistance;
        outline.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;
        outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;
        outline.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        outline.enabled = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        outline.enabled = pointerInside;
    }
}
