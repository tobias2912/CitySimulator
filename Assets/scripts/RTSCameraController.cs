using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RTSCameraController : MonoBehaviour
{
    public float screenEdgeBorderThickness = 5.0f; // distance from screen edge. Used for mouse movement

    [Header("Camera Mode")] [Space] public bool rtsMode = true;
    public bool flyCameraMode;

    [Header("Movement Speeds")] [Space] public float minPanSpeed;
    public float maxPanSpeed;
    public float secToMaxSpeed; //seconds taken to reach max speed;
    public float zoomSpeed;

    [Header("Movement Limits")] [Space] public bool enableMovementLimits;
    public Vector2 heightLimit;
    public Vector2 lenghtLimit;
    public Vector2 widthLimit;
    private Vector2 _zoomLimit;

    private float _panSpeed;
    private Vector3 _panMovement;
    private Vector3 _pos;
    private Quaternion _rot;
    private bool _rotationActive;
    private Vector3 _lastMousePosition;
    private float _panIncrease;

    [Header("Rotation")] [Space] public bool rotationEnabled;
    public float rotateSpeed;
    private Camera camera;

    void Start()
    {
        _zoomLimit.x = 15;
        _zoomLimit.y = 65;
        //check that ony one mode is choosen
        if (rtsMode) flyCameraMode = false;
        if (flyCameraMode) rtsMode = false;
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        #region Movement

        _panMovement = Vector3.zero;

        var transformForward = camera.transform.forward;
        var transformRight = camera.transform.right;
        transformForward.y = 0;
        transformRight.y = 0;
        transformForward.Normalize();
        transformRight.Normalize();
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - screenEdgeBorderThickness)
        {
            _panMovement += transformForward * (_panSpeed * Time.fixedDeltaTime);
        }

        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= screenEdgeBorderThickness)
        {
            _panMovement -= transformForward * (_panSpeed * Time.fixedDeltaTime);
        }

        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= screenEdgeBorderThickness)
        {
            _panMovement -= transformRight * (_panSpeed * Time.fixedDeltaTime);
        }

        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - screenEdgeBorderThickness)
        {
            _panMovement += transformRight * (_panSpeed * Time.fixedDeltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            _panMovement += Vector3.up * (_panSpeed * Time.fixedDeltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            _panMovement -= Vector3.up * (_panSpeed * Time.fixedDeltaTime);
        }

        if (rtsMode) transform.Translate(_panMovement, Space.World);
        else if (flyCameraMode) transform.Translate(_panMovement, Space.Self);

        //increase pan speed
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
                                    || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
                                    || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Q)
            // || Input.mousePosition.y >= Screen.height - screenEdgeBorderThickness
            // || Input.mousePosition.y <= screenEdgeBorderThickness
            // || Input.mousePosition.x <= screenEdgeBorderThickness
            // || Input.mousePosition.x >= Screen.width - screenEdgeBorderThickness
           )
        {
            _panIncrease += Time.fixedDeltaTime / secToMaxSpeed;
            _panSpeed = Mathf.Lerp(minPanSpeed, maxPanSpeed, _panIncrease);
        }
        else
        {
            _panIncrease = 0;
            _panSpeed = minPanSpeed;
        }

        #endregion

        #region Zoom

        Camera.main.fieldOfView -= Input.mouseScrollDelta.y * zoomSpeed;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, _zoomLimit.x, _zoomLimit.y);

        #endregion

        #region mouse rotation

        if (rotationEnabled)
        {
            if (Input.GetMouseButton(1))
            {
                _rotationActive = true;
                Vector3 mouseDelta;
                if (_lastMousePosition.x >= 0 &&
                    _lastMousePosition.y >= 0 &&
                    _lastMousePosition.x <= Screen.width &&
                    _lastMousePosition.y <= Screen.height)
                    mouseDelta = Input.mousePosition - _lastMousePosition;
                else
                {
                    mouseDelta = Vector3.zero;
                }

                var rotation = camera.transform.up * (Time.fixedDeltaTime * rotateSpeed * mouseDelta.x);
                rotation -= camera.transform.right * (Time.fixedDeltaTime * rotateSpeed * mouseDelta.y);

                transform.Rotate(rotation, Space.World);

                // Make sure z rotation stays locked
                rotation = transform.rotation.eulerAngles;
                rotation.z = 0;
                transform.rotation = Quaternion.Euler(rotation);
            }

            if (Input.GetMouseButtonUp(1))
            {
                _rotationActive = false;
                // if (RTSMode) transform.rotation = Quaternion.Slerp(transform.rotation, initialRot, 0.5f * Time.time);
            }

            _lastMousePosition = Input.mousePosition;
        }

        #endregion

        #region boundaries

        if (enableMovementLimits)
        {
            //movement limits
            _pos = transform.position;
            _pos.y = Mathf.Clamp(_pos.y, heightLimit.x, heightLimit.y);
            // _pos.z = Mathf.Clamp(_pos.z, lenghtLimit.x, lenghtLimit.y);
            // _pos.x = Mathf.Clamp(_pos.x, widthLimit.x, widthLimit.y);
            transform.position = _pos;
        }

        #endregion
    }
}