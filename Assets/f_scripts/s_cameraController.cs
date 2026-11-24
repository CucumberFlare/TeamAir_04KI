using UnityEngine;

public class s_cameraController : MonoBehaviour
{
    public Transform cameraTransform; //Позиция камера
    public float sensitivity = 150f; //Чувствительность мыши
    private float xRotation = 0f;

    void Start()
    { 
        //Фиксируем мышь в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //Получаем движение мыши
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //Поворот игрока влево/вправо
        transform.Rotate(Vector3.up * mouseX);

        //Наклон камеры вверх/вниз
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
