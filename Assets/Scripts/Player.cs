using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //==================================================
    // MOVIMIENTO
    //==================================================

    [Header("Movimiento")]

    [SerializeField]
    private float velocidadMovimiento = 6f;

    private Rigidbody2D rb;

    private Vector2 direccionMovimiento;

    //--------------------------
    // VENENO
    //--------------------------

    private bool envenenado = false;

    [SerializeField]
    private float tiempoVeneno = 0f;

    [SerializeField]
    private float velocidadOriginal = 6f;

    private float multiplicadorVeneno = 1f;

    //==================================================
    // DIRECCIÓN
    //==================================================

    private Vector2 ultimaDireccionMovimiento = Vector2.right;

    private Vector2 posicionInicial;

    private enum DireccionCardinal
    {
        Arriba,
        Abajo,
        Izquierda,
        Derecha
    }

    private DireccionCardinal ultimaDireccionCardinal =
        DireccionCardinal.Derecha;

    [SerializeField]
    private float distanciaTeleport = 4f;

    [SerializeField]
    private GameObject prefabMuro;

    [SerializeField]
    private float distanciaMuro = 1.2f;

    const float xmin = -9.85f;//se debe cambiar
    const float xmax = 9.85f;//se debe cambiar

    const float ymin = -7.0f;//se debe cambiar
    const float ymax = 7.0f;//se debe cambiar

    [SerializeField]
    private float anchoMuro = 2.114f;

    //==================================================
    // VIDA
    //==================================================

    [Header("Vida")]

    [SerializeField]
    private int vidasMaximas = 3;

    private int vidasActuales;

    [SerializeField]
    private float duracionInmunidad = 2f;

    private bool esInmune = false;
    private float tiempoInmunidad = 0f;

    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;


    //==================================================
    // HABILIDADES
    //==================================================

    [Header("Cooldown General")]

    [SerializeField]
    private float cooldownGeneral = 10f;

    private bool jugadorPausado = false;

    //--------------------------
    // TELETRANSPORTE
    //--------------------------

    private bool teleportDisponible = true;

    private float cooldownTeleport = 0f;


    //--------------------------
    // RALENTIZAR
    //--------------------------

    private bool slowDisponible = true;

    private float cooldownSlow = 0f;

    private bool slowActivo = false;

    [SerializeField]
    private float duracionSlow = 5f;

    private float tiempoRestanteSlow = 0f;

    public GeneradorBase generador;

    //--------------------------
    // MURO
    //--------------------------

    private bool muroDisponible = true;

    private float cooldownMuro = 0f;

    private bool muroActivo = false;

    [SerializeField]
    private int vidaInicialMuro = 15;

    private int vidaActualMuro = 0;

    private Vector2 posicionMuro;


    //==================================================
    // GUARDADO
    //==================================================

    private string prefijoGuardado;

    private int nivelActual;


    //==================================================
    // UI
    //==================================================

    [Header("UI Habilidades")]
    [SerializeField] private Image iconoTeleport;
    [SerializeField] private Image iconoSlow;
    [SerializeField] private Image iconoMuro;

    private Color colorOriginalTeleport;
    private Color colorOriginalSlow;
    private Color colorOriginalMuro;

    [SerializeField]
    private Color colorCooldown = new Color(0.25f, 0.25f, 0.25f, 1f);

    [Header("UI Vidas")]

    [SerializeField] private Image imagenVidas;

    [SerializeField] private Sprite sprite3Vidas;
    [SerializeField] private Sprite sprite2Vidas;
    [SerializeField] private Sprite sprite1Vida;
    [SerializeField] private Sprite sprite0Vidas;

    public System.Action OnPlayerMuerto;

    //==================================================
    // ICONO-HABILIDADES
    //==================================================

    private void ActualizarIcono(Image icono, Color colorOriginal, bool disponible, float cooldown)
    {
        if (icono == null)
            return;

        if (disponible)
        {
            icono.color = colorOriginal;
            return;
        }

        float progreso = 1f - (cooldown / cooldownGeneral);

        Color colorActual =
            Color.Lerp(colorCooldown,
                       colorOriginal,
                       progreso);

        icono.color = colorActual;
    }

    private void ActualizarIconosHabilidades()
    {
        ActualizarIcono(
        iconoTeleport,
        colorOriginalTeleport,
        teleportDisponible,
        cooldownTeleport);

        ActualizarIcono(
            iconoSlow,
            colorOriginalSlow,
            slowDisponible,
            cooldownSlow);

        ActualizarIcono(
            iconoMuro,
            colorOriginalMuro,
            muroDisponible,
            cooldownMuro);
    }

    //==================================================
    // MOVIMIENTO
    //==================================================

    public void RestaurarPosicionInicial()
    {
        transform.position = posicionInicial;
    }

    private void ActualizarMovimiento()
    {
        direccionMovimiento.x = Input.GetAxisRaw("Horizontal");
        direccionMovimiento.y = Input.GetAxisRaw("Vertical");

        direccionMovimiento.Normalize();

        rb.linearVelocity = direccionMovimiento * velocidadMovimiento;

        if (direccionMovimiento != Vector2.zero)
        {
            ultimaDireccionMovimiento = direccionMovimiento;

            if (Mathf.Abs(direccionMovimiento.x) >
                Mathf.Abs(direccionMovimiento.y))
            {
                if (direccionMovimiento.x > 0)
                    ultimaDireccionCardinal = DireccionCardinal.Derecha;
                else
                    ultimaDireccionCardinal = DireccionCardinal.Izquierda;
            }
            else
            {
                if (direccionMovimiento.y > 0)
                    ultimaDireccionCardinal = DireccionCardinal.Arriba;
                else
                    ultimaDireccionCardinal = DireccionCardinal.Abajo;
            }
        }
    }

    private Vector2 AjustarPosicionMuro(Vector2 posicion, bool horizontal)
    {
        if (horizontal)
        {
            float mitadLargo = anchoMuro * 0.5f;

            posicion.x = Mathf.Clamp(
                posicion.x,
                xmin + mitadLargo,
                xmax - mitadLargo);

            posicion.y = Mathf.Clamp(
                posicion.y,
                ymin,
                ymax);
        }
        else
        {
            float mitadLargo = anchoMuro * 0.5f;

            posicion.x = Mathf.Clamp(
                posicion.x,
                xmin,
                xmax);

            posicion.y = Mathf.Clamp(
                posicion.y,
                ymin + mitadLargo,
                ymax - mitadLargo);
        }

        return posicion;
    }

    //==================================================
    // UNITY
    //==================================================
    public void InicializarJugador()
    {
        // Vida
        InicializarVidas();

        // Teletransporte
        teleportDisponible = true;
        cooldownTeleport = 0f;

        // Ralentización
        slowDisponible = true;
        cooldownSlow = 0f;
        slowActivo = false;
        tiempoRestanteSlow = 0f;

        // Muro
        muroDisponible = true;
        cooldownMuro = 0f;
        muroActivo = false;

        vidaActualMuro = 0;
        posicionMuro = Vector2.zero;
        if (muroActual != null)
        {
            Destroy(muroActual.gameObject);
        }
        muroActual = null;

        // Mostrar correctamente los iconos.
        ActualizarIconosHabilidades();
    }

    public void PausarJugador()
    {
        jugadorPausado = true;
    }

    public void ReanudarJugador()
    {
        jugadorPausado = false;
        if (slowActivo && generador != null)
        {
            generador.ActivarRalentizacion(0.5f);
        }
    }

    void Start()
    {

    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); //Provisional

        if (iconoTeleport != null)
            colorOriginalTeleport = iconoTeleport.color;

        if (iconoSlow != null)
            colorOriginalSlow = iconoSlow.color;

        if (iconoMuro != null)
            colorOriginalMuro = iconoMuro.color;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }

        posicionInicial = transform.position;
    }

    void Update()
    {
        if (jugadorPausado)
            return;

        //--------------------------
        // MOVIMIENTO
        //--------------------------

        ActualizarInmunidad();

        //--------------------------
        // MOVIMIENTO (PROVISIONAL)
        //--------------------------

        ActualizarMovimiento();

        ActualizarVeneno();

        //--------------------------
        // TELETRANSPORTE
        //--------------------------

        ActualizarCooldownTeleport();

        //--------------------------
        // RALENTIZACIÓN
        //--------------------------

        ActualizarRalentizacion();

        ActualizarCooldownSlow();

        //--------------------------
        // MURO
        //--------------------------

        ActualizarMuro();

        ActualizarCooldownMuro();

        ActualizarIconosHabilidades();
    }


    //==================================================
    // CONFIGURACIÓN
    //==================================================

    public void ConfigurarGuardado(int nivel)
    {
        nivelActual = nivel;

        prefijoGuardado =
            "Nivel" + nivelActual + "_Player_";
    }


    //==========================================
    // SISTEMA DE VIDAS
    //==========================================

    // Inicializa las vidas al comenzar una partida nueva.
    public void InicializarVidas()
    {
        vidasActuales = vidasMaximas;

        ActualizarUIVidas();
    }

    // Devuelve las vidas actuales.
    public int ObtenerVidas()
    {
        return vidasActuales;
    }

    // Devuelve las vidas máximas.
    public int ObtenerVidasMaximas()
    {
        return vidasMaximas;
    }

    // Devuelve la posición actual del jugador.
    public Vector2 ObtenerPosicion()
    {
        return transform.position;
    }

    // Restaura las vidas guardadas.
    public void RestaurarVidas(int vidas)
    {
        vidasActuales = vidas;

        ActualizarUIVidas();
    }

    // Restaura la posición del jugador.
    public void RestaurarPosicion(Vector2 posicion)
    {
        transform.position = posicion;
    }

    // El jugador recibe daño.
    public void PerderVida(int cantidad)
    {
        if (esInmune)
            return;

        if (vidasActuales <= 0)
            return;

        vidasActuales -= cantidad;

        if (vidasActuales < 0)
            vidasActuales = 0;

        esInmune = true;
        tiempoInmunidad = duracionInmunidad;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.cyan;
        }

        Debug.Log(
            "Player -> Vida perdida. Vidas restantes: "
            + vidasActuales);

        ActualizarUIVidas();

        if (vidasActuales <= 0)
        {
            Morir();
        }
    }

    // Actualizará la UI cuando exista el Canvas.
    private void ActualizarUIVidas()
    {
        if (imagenVidas == null)
            return;

        switch (vidasActuales)
        {
            case 3:
                imagenVidas.sprite = sprite3Vidas;
                break;

            case 2:
                imagenVidas.sprite = sprite2Vidas;
                break;

            case 1:
                imagenVidas.sprite = sprite1Vida;
                break;

            default:
                imagenVidas.sprite = sprite0Vidas;
                break;
        }
    }

    // Lógica cuando el jugador pierde todas las vidas.
    private void Morir()
    {
        Debug.Log("Player derrotado.");

        OnPlayerMuerto?.Invoke();
    }

    private void ActualizarInmunidad()
    {
        if (!esInmune)
            return;

        tiempoInmunidad -= Time.deltaTime;

        if (tiempoInmunidad <= 0f)
        {
            esInmune = false;

            if (spriteRenderer != null)
                spriteRenderer.color = colorOriginal;
        }
    }

    //==================================================
    // TELETRANSPORTE
    //==================================================

    public void ActivarTeleport()
    {
        // Si está en cooldown,
        // no puede utilizarse.
        if (!teleportDisponible)
            return;

        teleportDisponible = false;

        cooldownTeleport = cooldownGeneral;

        Vector2 destino =
        (Vector2)transform.position +
        ultimaDireccionMovimiento * distanciaTeleport;

        destino.x = Mathf.Clamp(destino.x, xmin, xmax);
        destino.y = Mathf.Clamp(destino.y, ymin, ymax);

        transform.position = destino;

        ActualizarIconosHabilidades();

        Debug.Log("Player -> Teletransporte activado.");
    }


    private void ActualizarCooldownTeleport()
    {
        if (teleportDisponible)
            return;

        cooldownTeleport -= Time.deltaTime;

        if (cooldownTeleport <= 0f)
        {
            cooldownTeleport = 0f;

            teleportDisponible = true;

            //ActualizarIconosHabilidades();

            Debug.Log(
                "Player -> Teletransporte disponible nuevamente.");
        }
    }


    //==================================================
    // RALENTIZAR
    //==================================================

    // Activa la habilidad de ralentización.
    public void ActivarRalentizacion()
    {
        // Si está en cooldown, no puede usarse.
        if (!slowDisponible)
            return;

        slowDisponible = false;

        ActualizarIconosHabilidades();

        slowActivo = true;

        tiempoRestanteSlow = duracionSlow;

        cooldownSlow = cooldownGeneral;

        // Aplica el efecto a todos los discos activos.
        if (generador != null)
        {
            generador.ActivarRalentizacion(0.5f);
        }

        Debug.Log("Player -> Ralentización activada.");
    }

    // Controla la duración de la habilidad.
    private void ActualizarRalentizacion()
    {
        if (!slowActivo)
            return;

        tiempoRestanteSlow -= Time.deltaTime;

        if (tiempoRestanteSlow <= 0f)
        {
            FinalizarRalentizacion();
        }
    }

    // Finaliza la ralentización.
    private void FinalizarRalentizacion()
    {
        slowActivo = false;

        tiempoRestanteSlow = 0f;

        if (generador != null)
        {
            generador.DesactivarRalentizacion();
        }

        Debug.Log("Player -> Ralentización finalizada.");
    }

    // Controla el cooldown.
    private void ActualizarCooldownSlow()
    {
        if (slowDisponible)
            return;

        cooldownSlow -= Time.deltaTime;

        if (cooldownSlow <= 0f)
        {
            cooldownSlow = 0f;

            slowDisponible = true;

            //ActualizarIconosHabilidades();

            Debug.Log("Player -> Ralentización disponible nuevamente.");
        }
    }

    //==================================================
    // MURO
    //==================================================

    // Referencia al muro actualmente colocado.
    private Muro muroActual;

    // Activa la habilidad del muro.
    public void ActivarMuro()
    {
        // Si está en cooldown, no puede usarse.
        if (!muroDisponible)
            return;

        // Si ya existe un muro activo, no crear otro.
        if (muroActivo)
            return;

        muroDisponible = false;

        muroActivo = true;

        cooldownMuro = cooldownGeneral;

        Vector2 offset = Vector2.zero;

        float rotacion = 0f;

        switch (ultimaDireccionCardinal)
        {
            case DireccionCardinal.Arriba:

                offset = Vector2.up * distanciaMuro;
                rotacion = 0f;
                break;

            case DireccionCardinal.Abajo:

                offset = Vector2.down * distanciaMuro;
                rotacion = 0f;
                break;

            case DireccionCardinal.Izquierda:

                offset = Vector2.left * distanciaMuro;
                rotacion = 90f;
                break;

            case DireccionCardinal.Derecha:

                offset = Vector2.right * distanciaMuro;
                rotacion = 90f;
                break;
        }

        Vector2 posicionFinal = (Vector2)transform.position + offset;

        bool horizontal =
            (ultimaDireccionCardinal == DireccionCardinal.Arriba ||
             ultimaDireccionCardinal == DireccionCardinal.Abajo);

        posicionFinal =
            AjustarPosicionMuro(
                posicionFinal,
                horizontal);

        GameObject nuevoMuro =
            Instantiate(
                prefabMuro,
                posicionFinal,
                Quaternion.Euler(0f, 0f, rotacion));

        muroActual =
            nuevoMuro.GetComponent<Muro>();

        vidaActualMuro = vidaInicialMuro;

        posicionMuro = muroActual.transform.position;

        ActualizarIconosHabilidades();

        Debug.Log("Player -> Muro colocado.");
    }

    // Actualiza continuamente la información del muro.
    private void ActualizarMuro()
    {
        if (!muroActivo)
            return;

        // Si el muro fue destruido.
        if (muroActual == null)
        {
            muroActivo = false;

            vidaActualMuro = 0;

            return;
        }

        // Mantener actualizados los datos para el guardado.
        vidaActualMuro =
            muroActual.ObtenerResistencia();

        posicionMuro =
            muroActual.transform.position;
    }

    // Controla el cooldown del muro.
    private void ActualizarCooldownMuro()
    {
        if (muroDisponible)
            return;

        cooldownMuro -= Time.deltaTime;

        if (cooldownMuro <= 0f)
        {
            cooldownMuro = 0f;

            muroDisponible = true;

            //ActualizarIconosHabilidades();

            Debug.Log("Player -> Muro disponible nuevamente.");
        }
    }

    //--------------------------
    // VENENO
    //--------------------------

    public void AplicarVeneno(float duracion, float multiplicador)
    {
        envenenado = true;

        tiempoVeneno = duracion;

        multiplicadorVeneno = multiplicador;

        velocidadMovimiento = velocidadOriginal * multiplicadorVeneno;

        Debug.Log("Player envenenado.");
    }

    private void ActualizarVeneno()
    {
        if (!envenenado)
            return;

        tiempoVeneno -= Time.deltaTime;

        if (tiempoVeneno <= 0f)
        {
            envenenado = false;

            velocidadMovimiento = velocidadOriginal;

            Debug.Log("Veneno finalizado.");
        }
    }

    //==================================================
    // GUARDAR
    //==================================================

    public void GuardarJugador()
    {
        //--------------------------
        // VIDAS
        //--------------------------

        PlayerPrefs.SetInt(
            prefijoGuardado + "VidasActuales",
            vidasActuales);

        //--------------------------
        // POSICIÓN
        //--------------------------

        PlayerPrefs.SetFloat(
            prefijoGuardado + "PosX",
            transform.position.x);

        PlayerPrefs.SetFloat(
            prefijoGuardado + "PosY",
            transform.position.y);

        //--------------------------
        // DIRECCIÓN
        //--------------------------

        PlayerPrefs.SetFloat(
            prefijoGuardado + "DireccionX",
            ultimaDireccionMovimiento.x);

        PlayerPrefs.SetFloat(
            prefijoGuardado + "DireccionY",
            ultimaDireccionMovimiento.y);

        PlayerPrefs.SetInt(
            prefijoGuardado + "DireccionCardinal",
            (int)ultimaDireccionCardinal);

        //--------------------------
        // TELETRANSPORTE
        //--------------------------

        PlayerPrefs.SetInt(
            prefijoGuardado + "TeleportDisponible",
            teleportDisponible ? 1 : 0);

        PlayerPrefs.SetFloat(
            prefijoGuardado + "CooldownTeleport",
            cooldownTeleport);

        //--------------------------
        // RALENTIZAR
        //--------------------------

        PlayerPrefs.SetInt(
            prefijoGuardado + "SlowDisponible",
            slowDisponible ? 1 : 0);

        PlayerPrefs.SetFloat(
            prefijoGuardado + "CooldownSlow",
            cooldownSlow);

        PlayerPrefs.SetInt(
            prefijoGuardado + "SlowActivo",
            slowActivo ? 1 : 0);

        PlayerPrefs.SetFloat(
            prefijoGuardado + "TiempoSlow",
            tiempoRestanteSlow);

        //--------------------------
        // MURO
        //--------------------------

        PlayerPrefs.SetInt(
            prefijoGuardado + "MuroDisponible",
            muroDisponible ? 1 : 0);

        PlayerPrefs.SetFloat(
            prefijoGuardado + "CooldownMuro",
            cooldownMuro);

        PlayerPrefs.SetInt(
            prefijoGuardado + "MuroActivo",
            muroActivo ? 1 : 0);

        PlayerPrefs.SetInt(
            prefijoGuardado + "VidaMuro",
            vidaActualMuro);

        //--------------------------
        // POSICIÓN DEL MURO
        //--------------------------

        if (muroActivo && muroActual != null)
        {
            PlayerPrefs.SetFloat(
                prefijoGuardado + "MuroPosX",
                muroActual.transform.position.x);

            PlayerPrefs.SetFloat(
                prefijoGuardado + "MuroPosY",
                muroActual.transform.position.y);

            PlayerPrefs.SetFloat(
                prefijoGuardado + "MuroRotacion",
                muroActual.transform.eulerAngles.z);
        }

        //--------------------------
        // INMUNIDAD
        //--------------------------

        PlayerPrefs.SetInt(
            prefijoGuardado + "EsInmune",
            esInmune ? 1 : 0);

        PlayerPrefs.SetFloat(
            prefijoGuardado + "TiempoInmunidad",
            tiempoInmunidad);

        //--------------------------
        // VENENO
        //--------------------------

        PlayerPrefs.SetInt(
            prefijoGuardado + "Envenenado",
            envenenado ? 1 : 0);

        PlayerPrefs.SetFloat(
            prefijoGuardado + "TiempoVeneno",
            tiempoVeneno);

        PlayerPrefs.SetFloat(
            prefijoGuardado + "MultiplicadorVeneno",
            multiplicadorVeneno);

        //--------------------------
        // GUARDAR
        //--------------------------

        PlayerPrefs.Save();
    }

    //==================================================
    // RESTAURAR
    //==================================================

    public void RestaurarJugador()
    {
        //--------------------------
        // VIDAS
        //--------------------------

        vidasActuales =
            PlayerPrefs.GetInt(
                prefijoGuardado + "VidasActuales",
                vidasMaximas);

        ActualizarUIVidas();

        //--------------------------
        // INMUNIDAD
        //--------------------------

        esInmune =
            PlayerPrefs.GetInt(
                prefijoGuardado + "EsInmune",
                0) == 1;

        tiempoInmunidad =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "TiempoInmunidad",
                0f);

        if (esInmune && spriteRenderer != null)
        {
            spriteRenderer.color = Color.cyan;
        }

        //--------------------------
        // VENENO
        //--------------------------

        envenenado =
            PlayerPrefs.GetInt(
                prefijoGuardado + "Envenenado",
                0) == 1;

        tiempoVeneno =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "TiempoVeneno",
                0f);

        multiplicadorVeneno =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "MultiplicadorVeneno",
                1f);

        if (envenenado)
        {
            velocidadMovimiento =
                velocidadOriginal *
                multiplicadorVeneno;
        }
        else
        {
            velocidadMovimiento =
                velocidadOriginal;
        }

        //--------------------------
        // POSICIÓN
        //--------------------------

        float posX =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "PosX",
                transform.position.x);

        float posY =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "PosY",
                transform.position.y);

        transform.position =
            new Vector2(posX, posY);

        //--------------------------
        // DIRECCIÓN
        //--------------------------

        float dirX =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "DireccionX",
                1f);

        float dirY =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "DireccionY",
                0f);

        ultimaDireccionMovimiento =
            new Vector2(dirX, dirY);

        if (ultimaDireccionMovimiento != Vector2.zero)
        {
            ultimaDireccionMovimiento.Normalize();
        }

        ultimaDireccionCardinal =
            (DireccionCardinal)PlayerPrefs.GetInt(
                prefijoGuardado + "DireccionCardinal",
                (int)DireccionCardinal.Derecha);

        //--------------------------
        // TELETRANSPORTE
        //--------------------------

        teleportDisponible =
            PlayerPrefs.GetInt(
                prefijoGuardado + "TeleportDisponible",
                1) == 1;

        cooldownTeleport =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "CooldownTeleport",
                0f);

        //--------------------------
        // RALENTIZAR
        //--------------------------

        slowDisponible =
            PlayerPrefs.GetInt(
                prefijoGuardado + "SlowDisponible",
                1) == 1;

        cooldownSlow =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "CooldownSlow",
                0f);

        slowActivo =
            PlayerPrefs.GetInt(
                prefijoGuardado + "SlowActivo",
                0) == 1;

        tiempoRestanteSlow =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "TiempoSlow",
                0f);

        //--------------------------
        // MURO
        //--------------------------

        muroDisponible =
            PlayerPrefs.GetInt(
                prefijoGuardado + "MuroDisponible",
                1) == 1;

        cooldownMuro =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "CooldownMuro",
                0f);

        muroActivo =
            PlayerPrefs.GetInt(
                prefijoGuardado + "MuroActivo",
                0) == 1;

        vidaActualMuro =
            PlayerPrefs.GetInt(
                prefijoGuardado + "VidaMuro",
                vidaInicialMuro);

        float muroX =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "MuroPosX",
                0f);

        float muroY =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "MuroPosY",
                0f);

        float muroRotacion =
            PlayerPrefs.GetFloat(
                prefijoGuardado + "MuroRotacion",
                0f);

        posicionMuro =
            new Vector2(
                muroX,
                muroY);

        if (muroActivo)
        {
            GameObject nuevoMuro =
                Instantiate(
                    prefabMuro,
                    posicionMuro,
                    Quaternion.Euler(0f, 0f, muroRotacion));

            muroActual =
                nuevoMuro.GetComponent<Muro>();

            muroActual.RestaurarResistencia(
                vidaActualMuro);
        }

        // Restaurar el estado visual de las habilidades.
        ActualizarIconosHabilidades();
    }

    //==================================================
    // GETTERS
    //==================================================

    //--------------------------
    // TELETRANSPORTE
    //--------------------------

    public bool TeleportDisponible()
    {
        return teleportDisponible;
    }

    //--------------------------
    // RALENTIZAR
    //--------------------------

    public bool SlowDisponible()
    {
        return slowDisponible;
    }

    public bool SlowActivo()
    {
        return slowActivo;
    }

    //--------------------------
    // MURO
    //--------------------------

    public bool MuroDisponible()
    {
        return muroDisponible;
    }

    public bool MuroActivo()
    {
        return muroActivo;
    }

    public int ObtenerVidaMuro()
    {
        return vidaActualMuro;
    }

    //--------------------------
    // NIVEL
    //--------------------------

    public int ObtenerNivel()
    {
        return nivelActual;
    }

}