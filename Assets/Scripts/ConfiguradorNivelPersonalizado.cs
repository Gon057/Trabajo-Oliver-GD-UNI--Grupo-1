using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfiguradorNivelPersonalizado : MonoBehaviour
{
    //========================================
    // DISCOS
    //========================================

    [Header("Discos")]

    public Toggle disco1;
    public Toggle disco2;
    public Toggle disco3;
    public Toggle disco4;
    public Toggle disco5;


    //========================================
    // MUSICA
    //========================================

    [Header("Musica")]

    public TMP_Dropdown selectorMusica;


    //========================================
    // MENSAJE DE ERROR PARTE 1
    //========================================

    [Header("Mensaje de Error")]

    public GameObject mensajeError;
    public TMP_Text textoMensajeError;

    private Coroutine corrutinaMensaje;


    //========================================
    // START
    //========================================

    void Start()
    {
        // Parte 1 visible
        parte1.SetActive(true);

        // Parte 2 oculta
        parte2.SetActive(false);

        // Mensajes ocultos
        mensajeError.SetActive(false);
        mensajeErrorParte2.SetActive(false);

        // Música inicial: MainMenu
        selectorMusica.value = 0;

        // === NUEVO: CARGAR LA CONFIGURACIÓN AL INICIAR ===
        CargarConfiguracion();
    }


    //========================================
    // BOTON CONTINUAR
    //========================================

    public void Continuar()
    {
        // Ocultar mensaje anterior
        mensajeError.SetActive(false);


        //====================================
        // COMPROBAR DISCOS
        //====================================

        int cantidadDiscos = 0;

        if (disco1.isOn)
            cantidadDiscos++;

        if (disco2.isOn)
            cantidadDiscos++;

        if (disco3.isOn)
            cantidadDiscos++;

        if (disco4.isOn)
            cantidadDiscos++;

        if (disco5.isOn)
            cantidadDiscos++;


        // Mínimo un disco
        if (cantidadDiscos == 0)
        {
            MostrarError("Debes seleccionar al menos un disco.");
            return;
        }


        //====================================
        // COMPROBAR MUSICA
        //====================================

        int musicaElegida = selectorMusica.value;


        //====================================
        // TODO CORRECTO
        //====================================

        Debug.Log("=================================");
        Debug.Log("CONFIGURACION PARTE 1");
        Debug.Log("=================================");

        Debug.Log("Discos seleccionados:");

        if (disco1.isOn)
            Debug.Log("- Disco Normal");

        if (disco2.isOn)
            Debug.Log("- Disco Rapido");

        if (disco3.isOn)
            Debug.Log("- Disco Pesado");

        if (disco4.isOn)
            Debug.Log("- Disco Venenoso");

        if (disco5.isOn)
            Debug.Log("- Disco Explosivo");

        Debug.Log("----------------------------------------");


        Debug.Log("MÚSICA SELECCIONADA:");

        switch (musicaElegida)
        {
            case 0:
                Debug.Log("- Main Menu");
                break;

            case 1:
                Debug.Log("- Nivel 1");
                break;

            case 2:
                Debug.Log("- Nivel 2");
                break;

            case 3:
                Debug.Log("- Nivel 3");
                break;

            default:
                Debug.Log("- Música desconocida");
                break;
        }


        Debug.Log("========================================");

        //====================================
        // PASAR A PARTE 2
        //====================================

        parte1.SetActive(false);
        parte2.SetActive(true);

        Debug.Log("Parte 1 completada correctamente.");
        Debug.Log("Parte 2 iniciada.");
    }


    //========================================
    // MOSTRAR ERROR
    //========================================

    void MostrarError(string mensaje)
    {
        // Detener una animación anterior
        if (corrutinaMensaje != null)
        {
            StopCoroutine(corrutinaMensaje);
        }

        textoMensajeError.text = mensaje;

        // Mostrar completamente
        mensajeError.SetActive(true);

        // Reiniciar transparencia
        CambiarAlpha(1f);

        // Comenzar desvanecimiento
        corrutinaMensaje = StartCoroutine(
            DesvanecerMensaje()
        );
    }


    //========================================
    // DESVANECER MENSAJE
    //========================================

    IEnumerator DesvanecerMensaje()
    {
        // Tiempo completamente visible
        yield return new WaitForSeconds(1.5f);

        float duracion = 1.5f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            float alpha = Mathf.Lerp(
                1f,
                0f,
                tiempo / duracion
            );

            CambiarAlpha(alpha);

            yield return null;
        }

        CambiarAlpha(0f);

        mensajeError.SetActive(false);

        // Restaurar alpha para el siguiente mensaje
        CambiarAlpha(1f);

        corrutinaMensaje = null;
    }


    //========================================
    // CAMBIAR TRANSPARENCIA
    //========================================

    void CambiarAlpha(float alpha)
    {
        // Texto
        Color colorTexto = textoMensajeError.color;
        colorTexto.a = alpha;
        textoMensajeError.color = colorTexto;


        // Todos los elementos UI del mensaje
        Graphic[] elementos =
            mensajeError.GetComponentsInChildren<Graphic>(true);

        foreach (Graphic elemento in elementos)
        {
            Color color = elemento.color;
            color.a = alpha;
            elemento.color = color;
        }
    }

    //========================================
    // PARTE 2
    //========================================

    [Header("Parte 2")]

    public GameObject parte1;
    public GameObject parte2;


    //========================================
    // EVENTOS
    //========================================

    [Header("Eventos")]

    public Toggle eventoOverdrive;
    public Toggle eventoBlackout;


    //========================================
    // GENERADORES
    //========================================

    [Header("Generadores")]

    public Toggle generadorCentro;
    public Toggle generadorIzquierdaArriba;
    public Toggle generadorIzquierdaAbajo;
    public Toggle generadorDerechaArriba;
    public Toggle generadorDerechaAbajo;

    //========================================
    // MENSAJE DE ERROR PARTE 2
    //========================================

    [Header("Mensaje Parte 2")]

    public GameObject mensajeErrorParte2;
    public TMP_Text textoMensajeErrorParte2;

    private Coroutine corrutinaMensajeParte2;

    void MostrarErrorParte2(string mensaje)
    {
        if (corrutinaMensajeParte2 != null)
        {
            StopCoroutine(corrutinaMensajeParte2);
        }

        textoMensajeErrorParte2.text = mensaje;

        mensajeErrorParte2.SetActive(true);

        CambiarAlphaParte2(1f);

        corrutinaMensajeParte2 =
            StartCoroutine(DesvanecerMensajeParte2());
    }


    IEnumerator DesvanecerMensajeParte2()
    {
        yield return new WaitForSeconds(1.5f);

        float duracion = 1.5f;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            float alpha = Mathf.Lerp(
                1f,
                0f,
                tiempo / duracion
            );

            CambiarAlphaParte2(alpha);

            yield return null;
        }

        CambiarAlphaParte2(0f);

        mensajeErrorParte2.SetActive(false);

        CambiarAlphaParte2(1f);

        corrutinaMensajeParte2 = null;
    }


    void CambiarAlphaParte2(float alpha)
    {
        Color colorTexto = textoMensajeErrorParte2.color;
        colorTexto.a = alpha;
        textoMensajeErrorParte2.color = colorTexto;


        Graphic[] elementos =
            mensajeErrorParte2.GetComponentsInChildren<Graphic>(true);

        foreach (Graphic elemento in elementos)
        {
            Color color = elemento.color;
            color.a = alpha;
            elemento.color = color;
        }
    }

    public void CrearNivel()
    {
        // Ocultar mensaje anterior
        mensajeErrorParte2.SetActive(false);


        //====================================
        // CONTAR GENERADORES
        //====================================

        int cantidadGeneradores = 0;

        if (generadorCentro.isOn)
            cantidadGeneradores++;

        if (generadorIzquierdaArriba.isOn)
            cantidadGeneradores++;

        if (generadorIzquierdaAbajo.isOn)
            cantidadGeneradores++;

        if (generadorDerechaArriba.isOn)
            cantidadGeneradores++;

        if (generadorDerechaAbajo.isOn)
            cantidadGeneradores++;


        //====================================
        // MINIMO 1
        //====================================

        if (cantidadGeneradores == 0)
        {
            MostrarErrorParte2(
                "Debes escoger al menos un generador."
            );

            return;
        }


        //====================================
        // MAXIMO 3
        //====================================

        if (cantidadGeneradores > 3)
        {
            MostrarErrorParte2(
                "Solo puedes escoger 3 generadores como máximo."
            );

            return;
        }


        //====================================
        // EVENTOS
        //====================================

        Debug.Log("========================================");
        Debug.Log("      CONFIGURACIÓN PARTE 2");
        Debug.Log("========================================");


        Debug.Log("EVENTOS:");

        if (eventoOverdrive.isOn)
            Debug.Log("- Overdrive");

        if (eventoBlackout.isOn)
            Debug.Log("- Blackout");

        if (!eventoOverdrive.isOn && !eventoBlackout.isOn)
            Debug.Log("- Ningún evento");


        Debug.Log("----------------------------------------");


        Debug.Log("GENERADORES:");

        if (generadorCentro.isOn)
            Debug.Log("- Centro");

        if (generadorIzquierdaArriba.isOn)
            Debug.Log("- Izquierda Arriba");

        if (generadorIzquierdaAbajo.isOn)
            Debug.Log("- Izquierda Abajo");

        if (generadorDerechaArriba.isOn)
            Debug.Log("- Derecha Arriba");

        if (generadorDerechaAbajo.isOn)
            Debug.Log("- Derecha Abajo");


        Debug.Log("----------------------------------------");

        Debug.Log("Cantidad de generadores: " + cantidadGeneradores);

        Debug.Log("========================================");
        Debug.Log("      NIVEL PERSONALIZADO LISTO");
        Debug.Log("========================================");

        // === NUEVO: GUARDAR LA CONFIGURACIÓN ANTES DE CREAR EL NIVEL ===
        GuardarConfiguracion();

        SceneManager.LoadScene("NivelPersonalizado");
    }

    //========================================
    // SISTEMA DE GUARDADO (PLAYERPREFS)
    //========================================

    private void GuardarConfiguracion()
    {
        // Creamos una llave para saber que ya existe un guardado previo
        PlayerPrefs.SetInt("ConfigGuardada", 1);

        // Guardar Discos (1 es true, 0 es false)
        PlayerPrefs.SetInt("Disco1", disco1.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Disco2", disco2.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Disco3", disco3.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Disco4", disco4.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Disco5", disco5.isOn ? 1 : 0);

        // Guardar Musica
        PlayerPrefs.SetInt("MusicaElegida", selectorMusica.value);

        // Guardar Eventos
        PlayerPrefs.SetInt("EvOverdrive", eventoOverdrive.isOn ? 1 : 0);
        PlayerPrefs.SetInt("EvBlackout", eventoBlackout.isOn ? 1 : 0);

        // Guardar Generadores
        PlayerPrefs.SetInt("GenCentro", generadorCentro.isOn ? 1 : 0);
        PlayerPrefs.SetInt("GenIzqArr", generadorIzquierdaArriba.isOn ? 1 : 0);
        PlayerPrefs.SetInt("GenIzqAba", generadorIzquierdaAbajo.isOn ? 1 : 0);
        PlayerPrefs.SetInt("GenDerArr", generadorDerechaArriba.isOn ? 1 : 0);
        PlayerPrefs.SetInt("GenDerAba", generadorDerechaAbajo.isOn ? 1 : 0);

        // Guardar los datos en el disco duro
        PlayerPrefs.Save();
        Debug.Log("Configuración guardada exitosamente.");
    }

    private void CargarConfiguracion()
    {
        // Si no existe la llave "ConfigGuardada", significa que es la primera vez.
        // Hacemos un "return" para cancelar la carga y dejar los toggles como los pusiste en el Inspector.
        if (!PlayerPrefs.HasKey("ConfigGuardada"))
        {
            Debug.Log("Es la primera vez. No hay configuración para cargar.");
            return;
        }

        // Si el código llega hasta aquí, es porque SÍ hay datos guardados (y sabemos que son válidos).
        disco1.isOn = PlayerPrefs.GetInt("Disco1") == 1;
        disco2.isOn = PlayerPrefs.GetInt("Disco2") == 1;
        disco3.isOn = PlayerPrefs.GetInt("Disco3") == 1;
        disco4.isOn = PlayerPrefs.GetInt("Disco4") == 1;
        disco5.isOn = PlayerPrefs.GetInt("Disco5") == 1;

        // Cargar Musica
        selectorMusica.value = PlayerPrefs.GetInt("MusicaElegida");

        // Cargar Eventos
        eventoOverdrive.isOn = PlayerPrefs.GetInt("EvOverdrive") == 1;
        eventoBlackout.isOn = PlayerPrefs.GetInt("EvBlackout") == 1;

        // Cargar Generadores
        generadorCentro.isOn = PlayerPrefs.GetInt("GenCentro") == 1;
        generadorIzquierdaArriba.isOn = PlayerPrefs.GetInt("GenIzqArr") == 1;
        generadorIzquierdaAbajo.isOn = PlayerPrefs.GetInt("GenIzqAba") == 1;
        generadorDerechaArriba.isOn = PlayerPrefs.GetInt("GenDerArr") == 1;
        generadorDerechaAbajo.isOn = PlayerPrefs.GetInt("GenDerAba") == 1;

        Debug.Log("Configuración previa cargada correctamente.");
    }

}
