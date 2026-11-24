using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class s_playerController : MonoBehaviour
{
    private Rigidbody rb;
    public int speed = 7;
    private Renderer renderer;

    public float maxDistance = 3.5f;
    public Slider progressBar;

    public GameObject currentFlora;
    private Coroutine Symbiosis;
    private bool isProggresing = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveInput = (transform.forward * z + transform.right * x).normalized;
        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
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

                if (Input.GetKeyDown(KeyCode.E) && !isProggresing)
                {
                    Symbiosis = StartCoroutine(StartSymbiosis());
                }
            }
        }
        else { checkRayDinstance(); }

        if (Input.GetKeyUp(KeyCode.E) && isProggresing) { StopSymbiosis(); }
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

    void checkRayDinstance()
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
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
            
        }
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