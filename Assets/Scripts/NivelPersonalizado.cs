using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class NivelPersonalizado : MonoBehaviour
{
    //Tema de Menu de Nivel
    public GameObject Menupanel;
    public GameObject BotonMenu;

    // Tema de Tiempo
    public TextMeshProUGUI tiempoTexto;
    private float tiempo = 0f;

    //Variables de Opciones
    public Slider soundSlider;
    public AudioMixer masterMixer;
    public Toggle pantallaCompletaToggle;

    [Header("Player")]
    public Player player;

    //Música
    public AudioSource audioSource;

    [Header("Música")]
    public AudioClip musicaMenuPrincipal;
    public AudioClip musicaNivel1;
    public AudioClip musicaNivel2;
    public AudioClip musicaNivel3;


    //Eventos
    private bool ovActivado;
    private bool blActivado;
    public SistemaEventosNivelPersonalizado sistemaEventos;

    private int ConfigEventos;
    // Tiempo del próximo evento
    private float SiguienteEvento = 45f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.InicializarJugador();
        EstablecerConfiguracionEventos();

        Time.timeScale = 1f;

        //Buscamos si hay volumen guardado
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenDelJuego", 1f);

        //Movemos el slider al volumen guardado
        if (soundSlider != null)
        {
            soundSlider.value = volumenGuardado;
        }

        int musicaGuardada = PlayerPrefs.GetInt("MusicaElegida", 1);
        PonerMusica(musicaGuardada);

        float decibelios = Mathf.Log10(volumenGuardado) * 20;
        masterMixer.SetFloat("MasterVolume", decibelios);

        //Pantalla Completa Inicial
        int pantallaGuardada = PlayerPrefs.GetInt("PantallaGuardada", 0);

        bool esCompleta = (pantallaGuardada == 1);

        if (pantallaCompletaToggle != null)
        {
            pantallaCompletaToggle.isOn = esCompleta;
        }
        Screen.fullScreen = esCompleta;
    }

    // Update is called once per frame
    void Update()
    {
        tiempo += Time.deltaTime;

        int minutos = Mathf.FloorToInt(tiempo / 60);
        int segundos = Mathf.FloorToInt(tiempo % 60);
        tiempoTexto.text = string.Format("{0:00}:{1:00}", minutos, segundos);

        if (tiempo >= SiguienteEvento)
        {
            GenerarEvento();

            SiguienteEvento += 45f;
        }

        //==========================================
        // PRUEBA DE HABILIDADES
        //==========================================

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.ActivarTeleport();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            player.ActivarRalentizacion();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            player.ActivarMuro();
        }
    }

    public void CambiarVolumen(float volumen)
    {
        float decibelios = Mathf.Log10(volumen) * 20;
        masterMixer.SetFloat("MasterVolume", decibelios);

        //Guardar Cambio
        PlayerPrefs.SetFloat("VolumenDelJuego", volumen);
        PlayerPrefs.Save();
        Debug.Log("El volumen actual en dB es: " + decibelios);
    }

    public void PonerMusica(int indiceMusica)
    {
        // Elige la canción según el valor que le mandó el nivel personalizado
        switch (indiceMusica)
        {
            case 0:
                audioSource.clip = musicaMenuPrincipal;
                break;
            case 1:
                audioSource.clip = musicaNivel1;
                break;
            case 2:
                audioSource.clip = musicaNivel2;
                break;
            case 3:
                audioSource.clip = musicaNivel3;
                break;
        }

        // La reproduce
        audioSource.Play();
    }

    public void CambiarPantallaCompleta(bool activado)
    {
        Screen.fullScreen = activado;

        PlayerPrefs.SetInt("PantallaGuardada", activado ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Guardado en memoria. ¿Pantalla completa?: " + activado);
    }

    public void EstablecerConfiguracionEventos()
    {
        ovActivado = PlayerPrefs.GetInt("EvOverdrive", 0) == 1;
        blActivado = PlayerPrefs.GetInt("EvBlackout", 0) == 1;

        // 1. Ambos eventos
        if (ovActivado && blActivado)
        {
            ConfigEventos = 3;
            Debug.Log("Configuración: Ambos eventos activados");
        }
        // 2. Solo Overdrive
        else if (ovActivado && !blActivado)
        {
            ConfigEventos = 1;
            Debug.Log("Configuración: Solo Overdrive");
        }
        // 3. Solo Blackout
        else if (!ovActivado && blActivado)
        {
            ConfigEventos = 2;
            Debug.Log("Configuración: Solo Blackout");
        }
        // 4. Ningún evento
        else
        {
            ConfigEventos = 0;
            Debug.Log("Configuración: Ningún evento");
        }
    }

    public void GenerarEvento()
    {
        // Ningún evento
        if (ConfigEventos == 0)
        {
            Debug.Log("No hay eventos configurados.");
            return;
        }

        // Solo Overdrive
        if (ConfigEventos == 1)
        {
            Debug.Log("Generando Overdrive.");
            sistemaEventos.EjecutarOverdrive();
            return;
        }

        // Solo Blackout
        if (ConfigEventos == 2)
        {
            Debug.Log("Generando Blackout.");
            sistemaEventos.EjecutarBlackout();
            return;
        }

        // Ambos eventos
        if (ConfigEventos == 3)
        {
            int evento = UnityEngine.Random.Range(0, 2);

            if (evento == 0)
            {
                Debug.Log("Generando Overdrive.");
                sistemaEventos.EjecutarOverdrive();
            }
            else
            {
                Debug.Log("Generando Blackout.");
                sistemaEventos.EjecutarBlackout();
            }
        }
    }

    //Boton para abrir el panel
    public void OpenMenuPanel()
    {
        BotonMenu.SetActive(false);
        Time.timeScale = 0f;
        Menupanel.SetActive(true);
    }

    //Botones de Panel Menu
    public void VolverAlNivel()
    {
        Menupanel.SetActive(false);
        Time.timeScale = 1f;
        BotonMenu.SetActive(true);
    }
}
