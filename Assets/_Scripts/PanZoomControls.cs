using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanZoomControls : MonoBehaviour
{
    [Header("Pan stats")]

    [SerializeField]
    [Tooltip("Velocidad en la que se mueve la cámara")] 
    [Range(2, 15)]
    private float panSpeed = 2;

    [SerializeField]
    [Tooltip("Qué tan cerca del borde, entre 0 y 1, tiene que estar el mouse para que se mueva la cámara")]
    [MinTo(0, 1)]
    private Vector2 panPosition = new Vector2(0.05f, 0.95f);


    [Header("Zoom stats")]

    [SerializeField]
    [Tooltip("Velocidad en la que acerca/aleja la cámara")] 
    [Range(2, 15)]
    private float zoomSpeed = 3;

    [SerializeField]
    [Tooltip("Lo más cercano que puede estar la cámara")] 
    [Range(1, 10)]
    private float zoomInMax = 1;

    [SerializeField]
    [Tooltip("Lo más lejano que puede estar la cámara")] 
    [Range(2, 15)]
    private float zoomOutMax = 15;

    [Header("Unit Selection System")]

    [SerializeField] private InputAction mouseClick;

    [SerializeField]
    [Tooltip("Layer donde se buscan las unidades")] 
    private LayerMask clickable;
    [SerializeField] private LayerMask ground;
    [SerializeField] RectTransform boxVisual;

    private Rect selectionBox;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private CinemachineInputProvider inputProv;
    private CinemachineVirtualCamera virtualCam;
    private Transform camTransform;
    private Camera mainCam;

    void OnEnable()
    {
        mouseClick.Enable();
        mouseClick.performed += MousePressed;
    }

    void OnDisable()
    {
        mouseClick.performed -= MousePressed;
        mouseClick.Disable();
    }

    void Awake()
    {
        mainCam = Camera.main;
        inputProv = GetComponent<CinemachineInputProvider>();
        virtualCam = GetComponent<CinemachineVirtualCamera>();
        camTransform = virtualCam.VirtualCameraGameObject.transform;
    }

    void Update()
    {
        float x = inputProv.GetAxisValue(0);
        float y = inputProv.GetAxisValue(1);
        float z = inputProv.GetAxisValue(2);

        if(x != 0 || y != 0)
            Pan(x, y);

        if(z != 0)
            Zoom(z);
    }

    public void Zoom(float increment)
    {   
        float fov = virtualCam.m_Lens.OrthographicSize;
        float target = Mathf.Clamp(fov + increment, zoomInMax, zoomOutMax);
        virtualCam.m_Lens.OrthographicSize = Mathf.MoveTowards(fov, 
                                                               target,
                                                               zoomSpeed * Time.deltaTime);
    }

    public void Pan(float x, float y)
    {
        Vector2 direction = PanDirection(x, y);
        camTransform.position = Vector3.Lerp(camTransform.position, 
                                             camTransform.position + (Vector3) direction,
                                             panSpeed * Time.deltaTime);
    }

    private Vector2 PanDirection(float x, float y)
    {
        Vector2 direction = Vector2.zero;

        if(y >= Screen.height * panPosition.y)
            direction.y += 1;
        else if(y <= Screen.height * panPosition.x)
            direction.y -= 1;

        if(x >= Screen.width * panPosition.y)
            direction.x += 1;
        else if(x <= Screen.width * panPosition.x)
            direction.x -= 1;

        return direction;
    }

    private void MousePressed(InputAction.CallbackContext context)
    {
        RaycastHit2D hit = Physics2D.GetRayIntersection(mainCam.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if(hit.collider != null)
        {
            if(Keyboard.current.shiftKey.isPressed)
                UnitSelections.Instance.ShiftClickSelect(hit.collider.gameObject);
            else
                UnitSelections.Instance.ClickSelect(hit.collider.gameObject);
        }
        else 
        {
            if(!Keyboard.current.shiftKey.isPressed)
                UnitSelections.Instance.DeselectAll();
        }
    }
}
