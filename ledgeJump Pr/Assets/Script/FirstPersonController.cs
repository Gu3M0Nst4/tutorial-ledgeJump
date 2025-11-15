using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    public float velocidadCorrer = 9f; //velocidad al correr
    public float sensibilidadMouse = 2f;
    public float fuerzaSalto = 5f;
    public float fuerzaSaltoLargo = 7f; // salto largo con shift + espacio
    public float impulsoSaltoLargo = 3f; //empuje extra hacia adelante
    public float gravedad = -9.81f;
    public float tiempoCoyote = 0.15f;

    [Header("Ledge Grab")]
    public float duracionAgarre = 2f;
    public float velocidadSubida = 3f;
    public Vector3 offsetColgado = new Vector3(0f, -0.5f, 0.3f);
    public float alturaSubida = 1.6f;

    [Header("Cámara")]
    public Transform camara;

    private CharacterController controller;
    private Vector3 velocidadJugador;
    private float rotacionX = 0f;
    private float tiempoDesdeTocarSuelo = 0f;

    // Estados de agarre/subida
    private bool estaAgarrado = false;
    private bool estaSubiendo = false;
    private Transform puntoAgarre;
    private float tiempoAgarrado;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (estaAgarrado && !estaSubiendo)
        {
            ModoAgarrado();
            MovimientoCamara();
            return;
        }

        if (estaSubiendo)
        {
            MovimientoCamara();
            return;
        }

        MovimientoJugador();
        MovimientoCamara();
    }

    void MovimientoJugador()
    {
        if (controller.isGrounded)
        {
            tiempoDesdeTocarSuelo = 0f;
            if (velocidadJugador.y < 0)
                velocidadJugador.y = -2f;
        }
        else
        {
            tiempoDesdeTocarSuelo += Time.deltaTime;
        }

        //Detectar si está corriendo
        bool corriendo = Input.GetKey(KeyCode.LeftShift);
        float velocidadActual = corriendo ? velocidadCorrer : velocidad;

        // Movimiento horizontal
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 movimiento = transform.right * x + transform.forward * z;
        controller.Move(movimiento * velocidadActual * Time.deltaTime);

        //Salto
        if (Input.GetKeyDown(KeyCode.Space) && tiempoDesdeTocarSuelo < tiempoCoyote)
        {
            if (corriendo) // salto largo
            {
                velocidadJugador.y = Mathf.Sqrt(fuerzaSaltoLargo * -2f * gravedad);
                // Empuje hacia adelante en el salto largo
                Vector3 impulso = transform.forward * impulsoSaltoLargo;
                controller.Move(impulso * Time.deltaTime);
            }
            else // salto normal
            {
                velocidadJugador.y = Mathf.Sqrt(fuerzaSalto * -2f * gravedad);
            }

            tiempoDesdeTocarSuelo = tiempoCoyote;
        }

        // Gravedad
        velocidadJugador.y += gravedad * Time.deltaTime;
        controller.Move(velocidadJugador * Time.deltaTime);
    }

    void MovimientoCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);

        camara.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void ModoAgarrado()
    {
        if (puntoAgarre == null)
        {
            SoltarAgarre();
            return;
        }

        Vector3 posicionObjetivo = puntoAgarre.position + puntoAgarre.TransformDirection(offsetColgado);
        transform.position = Vector3.Lerp(transform.position, posicionObjetivo, 20f * Time.deltaTime);

        tiempoAgarrado += Time.deltaTime;
        if (tiempoAgarrado >= duracionAgarre)
        {
            SoltarAgarre();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SubirLedge());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ledge") && !estaAgarrado && !estaSubiendo)
        {
            AgarrarLedge(other.transform);
        }
    }

    void AgarrarLedge(Transform ledge)
    {
        estaAgarrado = true;
        estaSubiendo = false;
        puntoAgarre = ledge;
        tiempoAgarrado = 0f;
        velocidadJugador = Vector3.zero;
        controller.enabled = false;
    }

    void SoltarAgarre()
    {
        estaAgarrado = false;
        estaSubiendo = false;
        puntoAgarre = null;
        tiempoAgarrado = 0f;

        if (!controller.enabled)
            controller.enabled = true;

        velocidadJugador.y = -2f;
        StartCoroutine(EvitarReengancheFrame());
    }

    IEnumerator EvitarReengancheFrame()
    {
        yield return null;
    }

    IEnumerator SubirLedge()
    {
        if (puntoAgarre == null)
        {
            SoltarAgarre();
            yield break;
        }

        estaSubiendo = true;
        Vector3 inicio = transform.position;

        Vector3 adelanteDelLedge = puntoAgarre.TransformDirection(Vector3.forward) * 0.25f;
        Vector3 destino = puntoAgarre.position + Vector3.up * alturaSubida + adelanteDelLedge;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadSubida;
            transform.position = Vector3.Lerp(inicio, destino, t);
            yield return null;
        }

        if (!controller.enabled) controller.enabled = true;
        velocidadJugador.y = 0f;
        estaSubiendo = false;
        SoltarAgarre();
    }
}
