using UnityEngine;
using static Discos;

public class Discos : MonoBehaviour
{
    //==========================================
    // TIPOS DE DISCO
    //==========================================

    public enum TipoDisco
    {
        Normal,
        Rapido,
        Pesado,
        Explosivo,
        Venenoso
    }

    public enum EstadoExplosivo
    {
        Normal,
        Explosion,
        PostExplosion,
        Destruido
    }

    [Header("Tipo de Disco")]
    public TipoDisco tipo;

    //==========================================
    // ESTADÍSTICAS
    //==========================================

    [Header("Movimiento")]
    public float velocidadMovimiento;
    public float velocidadGiro;

    [Header("Combate")]
    public int danoContacto = 1;

    [Header("Explosivo")]
    public float tiempoExplosion = 5f;
    public float duracionExplosion = 0.4f;
    public float duracionPostExplosion = 0.4f;
    public float radioExplosion = 2.5f;
    private bool explosivoPausado = false;

    [Header("Venenoso")]
    public float duracionVeneno = 5f;
    public float multiplicadorVeneno = 0.5f;

    //==========================================
    // VARIABLES
    //==========================================
    
    //Animator Variable
    private Animator animator;

    //==========================================
    // MODIFICADORES DE VELOCIDAD
    //==========================================

    // Estado de efectos
    private bool overdriveActivo = false;
    private bool ralentizacionActiva = false;

    // Multiplicadores activos
    private float multiplicadorOverdrive = 1f;
    private float multiplicadorRalentizacion = 1f;

    private Rigidbody2D rb;
    private Vector2 direccion;
    public void InicializarDisco(bool movimientoInicial) 
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        ConfigurarDisco();

        direccion = Random.insideUnitCircle.normalized;

        if (movimientoInicial)
        {
            ActualizarVelocidad();
        }

        rb.angularVelocity = velocidadGiro;
    }

    private SpriteRenderer sprite;

    //==========================================
    // ESTADO DEL DISCO EXPLOSIVO
    //==========================================

    private EstadoExplosivo estadoExplosivo = EstadoExplosivo.Normal;

    private float temporizadorExplosion = 0f;
    private float tiempoEstado = 0f;

    private bool explosionDanioAplicado = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (tipo == TipoDisco.Explosivo)
        {
            ActualizarExplosivo();
        }
    }

    //==========================================
    // CONFIGURAR SEGÚN EL TIPO
    //==========================================

    void ConfigurarDisco()
    {
        switch (tipo)
        {
            case TipoDisco.Normal:

                velocidadMovimiento = 4f;
                velocidadGiro = 180f;

                danoContacto = 1;

                break;

            case TipoDisco.Rapido:

                velocidadMovimiento = 7f;
                velocidadGiro = 360f;

                danoContacto = 1;

                break;

            case TipoDisco.Pesado:

                velocidadMovimiento = 2.5f;
                velocidadGiro = 90f;

                danoContacto = 2;

                break;

            case TipoDisco.Explosivo:

                velocidadMovimiento = 3.5f;
                velocidadGiro = 180f;

                danoContacto = 1;

                break;

            case TipoDisco.Venenoso:

                velocidadMovimiento = 4f;
                velocidadGiro = 180f;

                danoContacto = 1;

                break;
        }
    }

    //==========================================
    // VELOCIDAD FINAL
    //==========================================

    private void ActualizarVelocidad()
    {
        if (rb == null)
            return;

        float velocidadFinal =
            velocidadMovimiento *
            multiplicadorOverdrive *
            multiplicadorRalentizacion;

        rb.linearVelocity =
            direccion * velocidadFinal;
    }

    public void RecalcularMovimiento()
    {
        ActualizarVelocidad();
    }

    //==============================
    // DATOS PARA EL GUARDADO
    //==============================

    // Devuelve el tipo de disco
    public TipoDisco ObtenerTipo()
    {
        return tipo;
    }

    // Devuelve la posición actual
    public Vector2 ObtenerPosicion()
    {
        return transform.position;
    }

    // Devuelve la dirección actual
    public Vector2 ObtenerDireccion()
    {
        return direccion;
    }

    // Restaura la información del disco
    public void RestaurarDisco(Vector2 posicion, Vector2 nuevaDireccion, bool iniciarMovimiento)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        ConfigurarDisco();
        transform.position = posicion;
        direccion = nuevaDireccion;

        if (iniciarMovimiento)
        {
            ActualizarVelocidad();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void ReanudarMovimiento()
    {
        ActualizarVelocidad();
    }

    public void ActivarOverdrive(float multiplicador)
    {

        if (tipo == TipoDisco.Explosivo &&
        estadoExplosivo != EstadoExplosivo.Normal)
        {
            return;
        }

        if (overdriveActivo)
            return;

        overdriveActivo = true;
        multiplicadorOverdrive = multiplicador;

        ActualizarVelocidad();

        Color c = sprite.color;
        c.r = 1f;
        c.g = 0.65f;
        c.b = 0.65f;

        sprite.color = c;
    }

    public void DesactivarOverdrive()
    {
        if (!overdriveActivo)
            return;

        overdriveActivo = false;
        multiplicadorOverdrive = 1f;

        ActualizarVelocidad();

        sprite.color = Color.white;
    }

    public void MostrarOverdriveVisual()
    {
        if (tipo == TipoDisco.Explosivo &&
        estadoExplosivo != EstadoExplosivo.Normal)
        {
            return;
        }

        Color c = sprite.color;
        c.r = 1f;
        c.g = 0.65f;
        c.b = 0.65f;

        sprite.color = c;
    }

    public void OcultarOverdriveVisual()
    {
        sprite.color = Color.white;
    }

    //==========================================
    // DATOS DEL EXPLOSIVO
    //==========================================

    public EstadoExplosivo ObtenerEstadoExplosivo()
    {
        return estadoExplosivo;
    }

    public float ObtenerTiempoEstado()
    {
        return tiempoEstado;
    }

    public float ObtenerTemporizadorExplosion()
    {
        return temporizadorExplosion;
    }

    public bool ObtenerDanioExplosionAplicado()
    {
        return explosionDanioAplicado;
    }

    public void RestaurarEstadoExplosivo(
    EstadoExplosivo estado,
    float temporizador,
    float tiempoFase,
    bool danoAplicado)
    {
        explosivoPausado = true;

        estadoExplosivo = estado;

        temporizadorExplosion = temporizador;

        tiempoEstado = tiempoFase;

        explosionDanioAplicado = danoAplicado;

        switch (estadoExplosivo)
        {
            //--------------------------
            // ETAPA 1
            //--------------------------

            case EstadoExplosivo.Normal:

                // Animator
                if (animator != null)
                {
                    animator.Play("Normal");
                    animator.speed = 0f;
                }

                break;

            //--------------------------
            // ETAPA 2
            //--------------------------

            case EstadoExplosivo.Explosion:

                // Animator
                if (animator != null)
                {
                    animator.Play("Explosion");
                    animator.speed = 0f;
                }

                break;

            //--------------------------
            // ETAPA 3
            //--------------------------

            case EstadoExplosivo.PostExplosion:

                // Animator
                if (animator != null)
                {
                    animator.Play("PostExplosion");
                    animator.speed = 0f;
                }

                break;

            //--------------------------
            // ETAPA 4
            //--------------------------

            case EstadoExplosivo.Destruido:

                Destroy(gameObject);

                break;
        }
    }

    //==========================================
    // COMBATE
    //==========================================

    void InfligirDanio(GameObject objetivo)
    {
        if (objetivo == null)
            return;

        //==========================
        // PLAYER
        //==========================

        Player player = objetivo.GetComponent<Player>();

        if (player != null)
        {
            player.PerderVida(danoContacto);
        }


        //==========================
        // MURO
        //==========================

        Muro muro = objetivo.GetComponent<Muro>();

        if (muro != null)
        {
            muro.RecibirDanio(danoContacto);
        }
    }

    void InfligirVeneno(GameObject objetivo)
    {
        if (objetivo == null)
            return;

        Player player = objetivo.GetComponent<Player>();

        if (player == null)
            return;

        player.AplicarVeneno(
            duracionVeneno,
            multiplicadorVeneno);
    }

    void AplicarDanioExplosion()
    {
        if (explosionDanioAplicado)
            return;

        explosionDanioAplicado = true;

        Collider2D[] objetos =
            Physics2D.OverlapCircleAll(
                transform.position,
                radioExplosion);

        foreach (Collider2D objeto in objetos)
        {
            if (objeto.CompareTag("Player") ||
            objeto.CompareTag("Muro"))
            {
                InfligirDanio(objeto.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Entró Trigger con: " + collision.name +
              " Tag: " + collision.tag);

        // Solo nos interesa el Player
        if (!collision.CompareTag("Player"))
            return;

        switch (tipo)
        {
            case TipoDisco.Normal:

                InfligirDanio(collision.gameObject);
                break;

            case TipoDisco.Rapido:

                InfligirDanio(collision.gameObject);
                break;

            case TipoDisco.Pesado:

                InfligirDanio(collision.gameObject);
                break;

            case TipoDisco.Venenoso:

                // Daño normal
                InfligirDanio(collision.gameObject);

                // Además aplica veneno
                InfligirVeneno(collision.gameObject);
                break;

            case TipoDisco.Explosivo:

                // Solo daña por contacto mientras no haya explotado
                if (estadoExplosivo == EstadoExplosivo.Normal)
                {
                    InfligirDanio(collision.gameObject);
                }
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Muro"))
            return;

        switch (tipo)
        {
            case TipoDisco.Normal:

                InfligirDanio(collision.gameObject);

                break;

            case TipoDisco.Rapido:

                InfligirDanio(collision.gameObject);

                break;

            case TipoDisco.Pesado:

                InfligirDanio(collision.gameObject);

                break;

            case TipoDisco.Venenoso:

                // Siempre hace daño
                InfligirDanio(collision.gameObject);

                break;

            case TipoDisco.Explosivo:

                // Solo hace daño por contacto
                // mientras está en la etapa Normal.

                if (estadoExplosivo == EstadoExplosivo.Normal)
                {
                    InfligirDanio(collision.gameObject);
                }

                break;
        }
    }
    //==========================================
    // EXPLOSIVO
    //==========================================

    void ActualizarExplosivo()
    {
        if (explosivoPausado)
            return;

        switch (estadoExplosivo)
        {
            //----------------------------
            // ETAPA 1
            //----------------------------

            case EstadoExplosivo.Normal:

                temporizadorExplosion += Time.deltaTime;

                if (temporizadorExplosion >= tiempoExplosion)
                {
                    IniciarExplosion();
                }

                break;

            //----------------------------
            // ETAPA 2
            //----------------------------

            case EstadoExplosivo.Explosion:

                tiempoEstado += Time.deltaTime;

                AplicarDanioExplosion();

                if (tiempoEstado >= duracionExplosion)
                {
                    estadoExplosivo = EstadoExplosivo.PostExplosion;
                    tiempoEstado = 0f;
                    animator.Play("PostExplosion");
                }

                break;

            //----------------------------
            // ETAPA 3
            //----------------------------

            case EstadoExplosivo.PostExplosion:

                tiempoEstado += Time.deltaTime;

                if (tiempoEstado >= duracionPostExplosion)
                {
                    FinalizarExplosion();
                }

                break;

            //----------------------------
            // ETAPA 4
            //----------------------------

            case EstadoExplosivo.Destruido:

                Destroy(gameObject);

                break;
        }
    }

    void IniciarExplosion()
    {
        estadoExplosivo = EstadoExplosivo.Explosion;

        tiempoEstado = 0f;

        explosionDanioAplicado = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        animator.Play("Explosion");
    }

    void FinalizarExplosion()
    {
        estadoExplosivo = EstadoExplosivo.Destruido;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        Destroy(gameObject);
    }

    public void PausarExplosivo()
    {
        explosivoPausado = true;

        if (animator != null)
        {
            animator.speed = 0f;
        }
    }

    public void ReanudarExplosivo()
    {
        explosivoPausado = false;

        if (animator != null)
        {
            animator.speed = 1f;
        }
    }

    //Efecto Ralentización

    public void ActivarRalentizacion(float multiplicador)
    {
        if (ralentizacionActiva)
            return;

        ralentizacionActiva = true;
        multiplicadorRalentizacion = multiplicador;

        ActualizarVelocidad();
    }

    public void DesactivarRalentizacion()
    {
        if (!ralentizacionActiva)
            return;

        ralentizacionActiva = false;
        multiplicadorRalentizacion = 1f;

        if (estadoExplosivo == EstadoExplosivo.Normal)
        {
            ActualizarVelocidad();
        }
    }

    public void GolpearPlayer(GameObject player)
    {
        switch (tipo)
        {
            case TipoDisco.Normal:
                InfligirDanio(player);
                break;

            case TipoDisco.Rapido:
                InfligirDanio(player);
                break;

            case TipoDisco.Pesado:
                InfligirDanio(player);
                break;

            case TipoDisco.Venenoso:
                InfligirDanio(player);
                InfligirVeneno(player);
                break;

            case TipoDisco.Explosivo:
                if (estadoExplosivo == EstadoExplosivo.Normal)
                    InfligirDanio(player);
                break;
        }
    }
}