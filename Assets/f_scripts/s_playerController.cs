using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class s_playerController : MonoBehaviour
{
    private Rigidbody rb;
    public int speed = 7;
    public float jumpVelocity = 5f; //Скорость прыжка
    private float runMultiplate = 1f; //Мультипликатор для бега
    private Renderer renderer;

    public float maxDistance = 3.5f;
    public Slider progressBar;
    public GameObject collectMessage; //Сообщение-подсказка

    public GameObject currentFlora;
    private Coroutine Symbiosis;
    private bool isProggresing = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        progressBar?.gameObject.SetActive(false);
        collectMessage?.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveInput = (transform.forward * z + transform.right * x).normalized;
        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime * runMultiplate);
    }

    void Update()
    {
        Vector3 rayDirection = transform.forward;
        rayDirection.y = 0;
        rayDirection.Normalize();

        Ray ray = new Ray(transform.position, rayDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("SwampFlora"))
            {
                currentFlora = hit.collider.gameObject;
                collectMessage?.gameObject.SetActive(true); //Показываем сообщение-подсказку при подходе

                if (Input.GetKeyDown(KeyCode.E) && !isProggresing)
                {
                    Symbiosis = StartCoroutine(StartSymbiosis());
                }
            }
        }
        else { CheckRayDinstance(); }

        if (Input.GetKeyUp(KeyCode.E) && isProggresing) { StopSymbiosis(); }

        //Механика прыжка
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
        }

        //Механика бега
        if (Input.GetKey(KeyCode.LeftShift)) { runMultiplate = 2f; }
        else { runMultiplate = 1f; }

        //Прячем подсказку при зажатии E, отходе от currentFlora, уничтожении currentFlora
        if (isProggresing ||
            (currentFlora != null &&
            Vector3.Distance(transform.position, currentFlora.transform.position) > maxDistance))
        {
            collectMessage?.gameObject.SetActive(false);
        }
    }

    

    IEnumerator StartSymbiosis()
    {
        isProggresing = true;
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0;
        }

        float symTime = 3f;
        float symPlayerTime = 0f;

        while(symPlayerTime < symTime)
        {
            symPlayerTime += Time.deltaTime;

            if (progressBar != null)
            {
                progressBar.value = symPlayerTime / symTime;
            }

            if (currentFlora == null || Vector3.Distance(transform.position, currentFlora.transform.position) > maxDistance)
            {
                StopSymbiosis();
                yield break;
            }

            yield return null;
        }
        SymbiosisCleared();
    }

    void CheckRayDinstance()
    {
        if (isProggresing && currentFlora != null)
        {
            if (Vector3.Distance(transform.position, currentFlora.transform.position) > maxDistance)
            {
                StopSymbiosis();
            }
        }
    }
    void StopSymbiosis()
    {
        if (Symbiosis != null)
        {
            StopCoroutine(Symbiosis);
            Symbiosis = null;
        }

        isProggresing = false;
        if (progressBar != null)
        {
            progressBar.value = 0;
            progressBar.gameObject.SetActive(false);
        }
    }

    void SymbiosisCleared()
    {
        isProggresing = false;
        Symbiosis = null;
        progressBar?.gameObject.SetActive(false);
        Debug.Log("+10 к Био-Энергии!");

        if (renderer != null)
        {
            renderer.material.color = Color.white;
        }

        if(currentFlora != null)
        {
            Destroy(currentFlora);
        }
    }
}