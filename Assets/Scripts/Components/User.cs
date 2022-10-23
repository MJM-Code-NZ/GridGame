using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MJM
{
    public class User : MonoBehaviour
    {
        private RenderSettings _renderSettings;
        
        private Camera _camera;

        private float _panSpeed;
        private float _zoomSpeed;

        private UserInputActions _userInputActions;

        private InputAction _panAction;
        private InputAction _zoomAction;

        void Awake()
        {
            _renderSettings = Resources.Load<RenderSettings>("Settings/RenderSettings");
            
            _camera = GameObject.Find("User").GetComponentInChildren<Camera>();
            _camera.transform.position = new Vector3(0.5f, 6.5f, -10);
            _camera.orthographicSize = _renderSettings.DefaultZoom;

            _panSpeed = _renderSettings.PanSpeed;
            _zoomSpeed = _renderSettings.ZoomSpeed;

            _userInputActions = new UserInputActions();

            _panAction = _userInputActions.User.Pan;
            _zoomAction = _userInputActions.User.Zoom;
        }
         
        void OnEnable()
        {
            _panAction.Enable();
            _zoomAction.Enable();
        }

        // Update is called once per frame
        void Update()
        {
            UpdatePan();
            UpdateZoom();
        }

        private void UpdatePan()
        {
            Vector2 panValue = _panAction.ReadValue<Vector2>();
            Vector3 panDisplacement = _panSpeed * panValue;

            _camera.transform.position = Vector3.Lerp(
                _camera.transform.position,
                _camera.transform.position + panDisplacement,
                Time.deltaTime
                );
        }

        private void UpdateZoom()
        {
            float zoomValue = _zoomAction.ReadValue<float>();
            float zoomDisplacement = _zoomSpeed * zoomValue;

            _camera.orthographicSize = Mathf.Lerp(
                _camera.orthographicSize,
                _camera.orthographicSize + zoomDisplacement,
                Time.deltaTime
                );

            _camera.orthographicSize = Mathf.Clamp(
                _camera.orthographicSize,
                _renderSettings.MinZoom,
                _renderSettings.MaxZoom
                );
        }

        void OnDisable()
        {
            _panAction.Disable();
            _zoomAction.Disable();
        }
    }
}
