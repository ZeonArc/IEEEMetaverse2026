using UnityEngine;

public class PTBillboard : MonoBehaviour
{
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null) return;
        }

        transform.rotation = Quaternion.LookRotation(transform.position - _mainCamera.transform.position);
    }
}
