using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardPosition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private HoveringPrism prism;
    private GameManager gameManager;

    public int x;
    public int z;

    // Start is called before the first frame update
    public void Start()
    {
        prism = Resources.FindObjectsOfTypeAll<HoveringPrism>()[0];
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    public override string ToString()
    {
        return base.ToString() + $"({x}, {z})";
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        prism.HoverOver(this);
        gameManager.PlaceMovementIcons(this);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        gameManager.KillAllMovementIcons();
        prism.gameObject.SetActive(false);
    }
}
