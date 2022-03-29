using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragSelectionHandler : MonoBehaviour
{

    [SerializeField] private InputAction mouseClick;

    [SerializeField]
    [Tooltip("Layer donde se buscan las unidades")] 
    private LayerMask clickable;
    
    [SerializeField] 
    [Tooltip("Referencia al cuadrado de UI que representa la selección")] 
    RectTransform boxVisual;

    private Rect selectionBox;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private Camera mainCam;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private bool isDragging = false;


    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        DrawVisual();
    }

    void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            isDragging = true;
            startPosition = Mouse.current.position.ReadValue();
            selectionBox = new Rect();
        }
        else if(Mouse.current.leftButton.isPressed)
        {
            endPosition = Mouse.current.position.ReadValue();
            DrawVisual();
            DrawSelection();
        }
        else if(isDragging && !Mouse.current.leftButton.isPressed)
        {
            SelectUnits();
            isDragging = false;
            endPosition = Vector2.zero;
            startPosition = Vector2.zero;
            DrawVisual();
        }
    }

    void DrawVisual()
    {
        Vector2 boxStart = startPosition;
        Vector2 boxEnd = endPosition;

        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        boxVisual.position = boxCenter;

        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));
        boxVisual.sizeDelta = boxSize;
    }

    void DrawSelection()
    {
        if(Mouse.current.position.ReadValue().x < startPosition.x)
        {
            selectionBox.xMin = Mouse.current.position.ReadValue().x;
            selectionBox.xMax = startPosition.x;
        }
        else
        {
            selectionBox.xMin = startPosition.x;
            selectionBox.xMax = Mouse.current.position.ReadValue().x;
        }

        if(Mouse.current.position.ReadValue().y < startPosition.y)
        {
            selectionBox.yMin = Mouse.current.position.ReadValue().y;
            selectionBox.yMax = startPosition.y;
        }
        else
        {
            selectionBox.yMin = startPosition.y;
            selectionBox.yMax = Mouse.current.position.ReadValue().y;
        }
    }

    void SelectUnits()
    {
        foreach(var unit in UnitSelections.Instance.unitsList)
        {
            if(selectionBox.Contains(mainCam.WorldToScreenPoint(unit.transform.position)))
                UnitSelections.Instance.DragSelect(unit);
        }
    }
}
