using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Unit : MonoBehaviour
{
    [SerializeField] private GameObject selectedChild;
    [SerializeField] private InputAction mouseRightClick;
    [SerializeField] private Tilemap map;
    [SerializeField] private float moveSpeed;
    private bool isSelected;
    private Vector3 destination;
    private Camera mainCam;

    void OnEnable()
    {
        mouseRightClick.Enable();
        mouseRightClick.performed += MoveUnit;
    }

    void OnDisable()
    {
        mouseRightClick.performed -= MoveUnit;
        mouseRightClick.Disable();
    }

    void Start()
    {
        mainCam = Camera.main;
        UnitSelections.Instance.unitsList.Add(this.gameObject);
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        isSelected = UnitSelections.Instance.unitsSelected.Contains(this.gameObject);
        selectedChild.SetActive(isSelected);

        if(Vector3.Distance(transform.position, destination) > .1f)
        {
            transform.position = Vector3.MoveTowards(
                                    transform.position, 
                                    destination, 
                                    moveSpeed * Time.deltaTime
                                );
        }
    }

    void OnDestroy()
    {
        UnitSelections.Instance.unitsList.Remove(this.gameObject);
    }

    private void MoveUnit(InputAction.CallbackContext context)
    {
        if(isSelected)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            mousePos = mainCam.ScreenToWorldPoint(mousePos);
            Vector3Int gridPos = map.WorldToCell(mousePos);
            if(map.HasTile(gridPos))
                destination = mousePos;
        }
    }
}
