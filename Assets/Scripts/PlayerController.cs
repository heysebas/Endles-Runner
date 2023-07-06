using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager; // Reemplaza 'TuNamespace' con el nombre del espacio de nombres donde se encuentra la clase 'GameManager'


public enum DireccionInpunt
{
    Null,
    Arriba,
    Izquieda,
    Derecha,
    Abajo
}

public class PlayerController : MonoBehaviour
{

    [Header("Config")]
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private float valorSalto = 15f;
    [SerializeField] private float gravedad = 20f;

    [Header("Carril")]
    [SerializeField] private float posicionCarrilIzquierdo = -3.1f;
    [SerializeField] private float posicionCarrilDerecho = 3.1f;

    public bool EstaSaltando{get; private set;}
    public bool EstaDelizando { get; private set;}

    private DireccionInpunt direccionInpunt;
    private Coroutine coroutineDelizar;
    private CharacterController characterController;
    private PlayerAnimaciones playerAnimaciones;
    private float posicionVertical;
    private int carrilActual;
    private Vector3 direccionDeseada;

    private float controllerRadio;
    private float controllerAltura;
    private float controllerPosicionY;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimaciones = GetComponent<PlayerAnimaciones>();
    }

    // Start is called before the first frame update
    void Start()
    {
        controllerRadio = characterController.radius;
        controllerAltura = characterController.height;
        controllerPosicionY = characterController.center.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instancia.EstadoActual == EstadoDelJuego.Inicio ||
           GameManager.Instancia.EstadoActual == EstadoDelJuego.GameOver)
        {
            return;
        }
        DetectarInput();
        ControlarCarriles();
        CalcularMovimientoVertical();
        MoverPersonaje();
    }

    private void MoverPersonaje()
    {
        Vector3 nuevaPos = new Vector3(direccionDeseada.x, posicionVertical, velocidadMovimiento);
        characterController.Move(nuevaPos * Time.deltaTime);
    }

    private void CalcularMovimientoVertical()
    {
        if (characterController.isGrounded)
        {
            EstaSaltando = false;
            posicionVertical = 0f;

            if(EstaDelizando == false && EstaSaltando == false)
            {
                playerAnimaciones.MostrarAnimacionCorrer();
            }

            if (direccionInpunt == DireccionInpunt.Arriba)
            {
                posicionVertical = valorSalto;
                EstaSaltando = true;
                playerAnimaciones.MostrarAnimacionSaltar();
                if (coroutineDelizar != null)
                {
                    EstaDelizando = false;
                    ModificarColliderDeslizar(false);
                }
            }
            if (direccionInpunt == DireccionInpunt.Abajo)
            {
                if (EstaDelizando)
                {
                    return;
                }
                if (coroutineDelizar != null)
                {
                    StopCoroutine(coroutineDelizar);
                }
                DeslizarPersonaje();
            }
        }
        else
        {
            if(direccionInpunt == DireccionInpunt.Abajo)
            {
                posicionVertical -= valorSalto;
                DeslizarPersonaje();
            }
        }

        posicionVertical -= gravedad * Time.deltaTime;
    }

    private void ControlarCarriles()
    {
        switch (carrilActual)
        {
            //Mover Izquierda
            case -1:
                LogicaCarriIzquierdo();
                break;
            case 0:
                LogicaCarrilCentral();
                break;
            //Mover Derecha
            case 1:
                LogicaCarriDerecho();
                break;
        }
    }

    private void LogicaCarrilCentral()
    {
        if(transform.position.x > 0.1f)
        {
            MoverHorizontal(0f, Vector3.left);
        }
        else if (transform.position.x < -0.1f)
        {
            MoverHorizontal(0f, Vector3.right);
        }
        else
        {
            direccionDeseada = Vector3.zero;
        }
    }

    private void LogicaCarriIzquierdo()
    {
        MoverHorizontal(posicionCarrilIzquierdo, Vector3.left);
    }

    private void LogicaCarriDerecho()
    {
        MoverHorizontal(posicionCarrilDerecho, Vector3.right);
    }

    private void MoverHorizontal(float posicionX, Vector3 dirMovimiento)
    {
        float posicionHorizontal = Mathf.Abs(transform.position.x - posicionX);
        if (posicionHorizontal > 0.1f)
        {
            direccionDeseada = Vector3.Lerp(direccionDeseada, dirMovimiento * 20f, Time.deltaTime * 500f);
        }
        else
        {
            direccionDeseada = Vector3.zero;
            transform.position = new Vector3(posicionX, transform.position.y, transform.position.z);
        }
    }

    private void DeslizarPersonaje()
    {
        coroutineDelizar = StartCoroutine(CODelizatPersonaje());
    }

    private IEnumerator CODelizatPersonaje()
    {
        EstaDelizando = true;
        playerAnimaciones.MostrarAnimacionDelizar();
        ModificarColliderDeslizar(true);
        yield return new WaitForSeconds(2f);
        EstaDelizando = false;
        ModificarColliderDeslizar(false);


    }

    private void ModificarColliderDeslizar(bool modificar)
    {
        if (modificar)
        {
            // Modificar collider
            characterController.radius = 0.3f;
            characterController.height = 0.6f;
            characterController.center = new Vector3(0f, 0.35f, 0);

        }
        else
        {
            characterController.radius = controllerRadio;
            characterController.height = controllerAltura;
            characterController.center = new Vector3(0f, controllerPosicionY, 0);
        }
    }


    private void DetectarInput()
    {
        direccionInpunt = DireccionInpunt.Null;
        if (Input.GetKeyDown(KeyCode.A))
        {
            direccionInpunt = DireccionInpunt.Izquieda;
            carrilActual--;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            direccionInpunt = DireccionInpunt.Derecha;
            carrilActual++;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            direccionInpunt = DireccionInpunt.Arriba;
           
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            direccionInpunt = DireccionInpunt.Abajo;
            
        }

        carrilActual = Mathf.Clamp(carrilActual, -1, 1);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Obstaculo"))
        {
            if (GameManager.Instancia.EstadoActual == EstadoDelJuego.GameOver)
            {
                return;
            }
            playerAnimaciones.MostrarAnimacionColision();
            GameManager.Instancia.CambiarEstado(EstadoDelJuego.GameOver);
        }
    }

}
