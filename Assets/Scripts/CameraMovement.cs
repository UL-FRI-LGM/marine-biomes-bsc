    using UnityEngine;
    using UnityEngine.InputSystem;

    public class CameraMovement : MonoBehaviour
    {
        private CameraControls cameraActions;
        private InputAction movement;
        private Transform cameraTransform;

        [SerializeField]
        private float maxSpeed = 50f;
        private float speed;

        [SerializeField]
        private float acceleration = 10f;

        [SerializeField]
        private float damping = 15f;

        [SerializeField]
        private float stepSize = 2f;

        [SerializeField]
        private float zoomDampening = 7.5f;

        [SerializeField]
        private float minHeight = 5f;

        [SerializeField]
        private float maxHeight = 20f;

        [SerializeField]
        private float zoomSpeed = 20f;

        [SerializeField]
        private float maxRotationSpeed = 0.1f;

        //value set in various functions 
        //used to update the position of the camera base object.
        private Vector3 targetPosition;

        private float zoomHeight;

        //used to track and maintain velocity w/o a rigidbody
        private Vector3 horizontalVelocity;
        private Vector3 lastPosition;

        //tracks where the dragging action started
        Vector3 startDrag;

        private void Awake()
        {
            cameraActions = new CameraControls();
            cameraTransform = this.GetComponentInChildren<Camera>().transform;
        }

        private void OnEnable()
        {
            zoomHeight = cameraTransform.localPosition.y;
            cameraTransform.LookAt(this.transform);

            lastPosition = this.transform.position;

            movement = cameraActions.ControlCamera.MoveCamera;
            cameraActions.ControlCamera.RotateCamera.performed += RotateCamera;
            cameraActions.ControlCamera.ZoomCamera.performed += ZoomCamera;
            cameraActions.ControlCamera.Enable();
        }

        private void OnDisable()
        {
            cameraActions.ControlCamera.RotateCamera.performed -= RotateCamera;
            cameraActions.ControlCamera.ZoomCamera.performed -= ZoomCamera;
            cameraActions.ControlCamera.Disable();
        }

        private void Update()
        {
            //inputs
            GetKeyboardMovement();
            DragCamera();

            //move base and camera objects
            UpdateVelocity();
            UpdateBasePosition();
            UpdateCameraPosition();
        }

        private void UpdateVelocity()
        {
            horizontalVelocity = (this.transform.position - lastPosition) / Time.deltaTime;
            horizontalVelocity.y = 0f;
            lastPosition = this.transform.position;
        }

        private void GetKeyboardMovement()
        {
            Vector3 inputValue = movement.ReadValue<Vector2>().y * GetCameraUp();

            inputValue = inputValue.normalized;

            if (inputValue.sqrMagnitude > 0.1f)
            {
                Vector3 newPosition = transform.position + inputValue;

                // Get terrain height at the new position
                float terrainHeight = Terrain.activeTerrain.SampleHeight(newPosition);

                // Calculate the maximum allowed height (terrain max height + 100)
                float maxAllowedHeight = Terrain.activeTerrain.terrainData.bounds.max.y + 200f;

                // Clamp the new position vertically within both lower and upper limits
                newPosition.y = Mathf.Clamp(newPosition.y, Mathf.Max(terrainHeight + minHeight, transform.position.y - maxHeight), maxAllowedHeight);

                targetPosition += newPosition - transform.position;
            }
        }

        private void DragCamera()
        {
            if (!Mouse.current.rightButton.isPressed)
                return;

            //create plane to raycast to
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
            if(plane.Raycast(ray, out float distance))
            {
                if (Mouse.current.rightButton.wasPressedThisFrame)
                    startDrag = ray.GetPoint(distance);
                else
                    targetPosition += startDrag - ray.GetPoint(distance);
            }
        }

        private void UpdateBasePosition()
        {
            if (targetPosition.sqrMagnitude > 0.1f)
            {
                //create a ramp up or acceleration
                speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
                    // Calculate the new position
                    Vector3 newPosition = transform.position + targetPosition * speed * Time.deltaTime;

                    // Get terrain bounds
                    Vector3 terrainMin = Terrain.activeTerrain.GetPosition();
                    Vector3 terrainMax = terrainMin + Terrain.activeTerrain.terrainData.size;

                    // Clamp the new position to terrain bounds
                    newPosition.x = Mathf.Clamp(newPosition.x, terrainMin.x, terrainMax.x);
                    newPosition.z = Mathf.Clamp(newPosition.z, terrainMin.z, terrainMax.z);

                    // Assign the new position
                    transform.position = newPosition;

            }
            else
            {
                //create smooth slow down
                horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
                transform.position += horizontalVelocity * Time.deltaTime;
            }

            //reset for next frame
            targetPosition = Vector3.zero;
        }

        private void ZoomCamera(InputAction.CallbackContext obj)
        {
            float inputValue = -obj.ReadValue<Vector2>().y / 100f;

            if (Mathf.Abs(inputValue) > 0.1f)
            {
                zoomHeight = cameraTransform.localPosition.y + inputValue * stepSize;

                if (zoomHeight < minHeight)
                    zoomHeight = minHeight;
                else if (zoomHeight > maxHeight)
                    zoomHeight = maxHeight;
            }
        }

        private void UpdateCameraPosition()
        {
            //set zoom target
             Vector3 zoomTarget = new Vector3(cameraTransform.localPosition.x, zoomHeight, cameraTransform.localPosition.z);
            //add vector for forward/backward zoom
            zoomTarget -= zoomSpeed * (zoomHeight - cameraTransform.localPosition.y) * Vector3.forward;

            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, zoomTarget, Time.deltaTime * zoomDampening);
            cameraTransform.LookAt(this.transform);
        }
     
        private void RotateCamera(InputAction.CallbackContext obj)
        {
            if (!Mouse.current.middleButton.isPressed)
                return;

            float inputValueX = obj.ReadValue<Vector2>().x;
            float inputValueY = obj.ReadValue<Vector2>().y;
            transform.rotation = Quaternion.Euler(-inputValueY * maxRotationSpeed + transform.rotation.eulerAngles.x, inputValueX * maxRotationSpeed + transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }

        private Vector3 GetCameraUp()
        {
            return Vector3.up;
        }

        //gets the horizontal forward vector of the camera
        private Vector3 GetCameraForward()
        {
            Vector3 forward = cameraTransform.forward;
            forward.y = 0f;
            return forward;
        }

        //gets the horizontal right vector of the camera
        private Vector3 GetCameraRight()
        {
            Vector3 right = cameraTransform.right;
            right.y = 0f;
            return right;
        }

    }
