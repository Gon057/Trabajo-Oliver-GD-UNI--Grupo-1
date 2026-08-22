using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GeneradorControlNivelPersonalizado : GeneradorBase
{
    //==================================================
    // GENERADORES
    //==================================================

    [Header("Generadores")]

    public GeneradorNivelPersonalizado generadorCentro;

    public GeneradorNivelPersonalizado generadorIzquierdaArriba;

    public GeneradorNivelPersonalizado generadorIzquierdaAbajo;

    public GeneradorNivelPersonalizado generadorDerechaArriba;

    public GeneradorNivelPersonalizado generadorDerechaAbajo;


    //==================================================
    // GENERADORES ACTIVOS
    //==================================================

    // Aquí estarán solamente los generadores que
    // fueron seleccionados en la configuración.
    private List<GeneradorNivelPersonalizado> generadoresActivos =
        new List<GeneradorNivelPersonalizado>();


    //==================================================
    // DISCOS
    //==================================================

    // Lista general de todos los discos que existen
    // en los 5 generadores.
    public List<GameObject> discosActivos =
        new List<GameObject>();


    //==================================================
    // GUARDADO
    //==================================================

    // Por ahora solamente dejamos preparado el prefijo.
    // La lógica de guardado se hará posteriormente.
    private string prefijoGuardado =
        "NivelPersonalizado_";


    //==================================================
    // REFERENCIA AL NIVEL
    //==================================================

    [Header("Nivel")]

    public NivelPersonalizado nivel;


    //==================================================
    // ESTADO DE GENERADORES
    //==================================================

    private bool generacionActiva = false;


    //==================================================
    // ESTADO DE EVENTOS
    //==================================================

    private bool overdriveActivo = false;

    private float multiplicadorOverdrive = 1f;

    private bool slowActivo = false;

    private float multiplicadorSlow = 1f;


    //==================================================
    // UNITY
    //==================================================

    void Awake()
    {
        ConfigurarGeneradores();
        ConfigurarPrefijosGuardado();
    }

    //==================================================
    // CONFIGURAR GENERADORES
    //==================================================

    public void ConfigurarGeneradores()
    {
        // Limpiar la lista por seguridad
        generadoresActivos.Clear();


        //==============================================
        // CENTRO
        //==============================================

        if (PlayerPrefs.GetInt("GenCentro", 0) == 1)
        {
            if (generadorCentro != null)
            {
                generadoresActivos.Add(generadorCentro);
            }
        }


        //==============================================
        // IZQUIERDA ARRIBA
        //==============================================

        if (PlayerPrefs.GetInt("GenIzqArr", 0) == 1)
        {
            if (generadorIzquierdaArriba != null)
            {
                generadoresActivos.Add(generadorIzquierdaArriba);
            }
        }


        //==============================================
        // IZQUIERDA ABAJO
        //==============================================

        if (PlayerPrefs.GetInt("GenIzqAba", 0) == 1)
        {
            if (generadorIzquierdaAbajo != null)
            {
                generadoresActivos.Add(generadorIzquierdaAbajo);
            }
        }


        //==============================================
        // DERECHA ARRIBA
        //==============================================

        if (PlayerPrefs.GetInt("GenDerArr", 0) == 1)
        {
            if (generadorDerechaArriba != null)
            {
                generadoresActivos.Add(generadorDerechaArriba);
            }
        }


        //==============================================
        // DERECHA ABAJO
        //==============================================

        if (PlayerPrefs.GetInt("GenDerAba", 0) == 1)
        {
            if (generadorDerechaAbajo != null)
            {
                generadoresActivos.Add(generadorDerechaAbajo);
            }
        }


        //==============================================
        // INFORMACIÓN
        //==============================================

        Debug.Log(
            "GeneradorControl -> Generadores activos: "
            + generadoresActivos.Count
        );

        foreach (GeneradorNivelPersonalizado generador
                 in generadoresActivos)
        {
            Debug.Log(
                "Generador activo: "
                + generador.gameObject.name
            );
        }
    }

    //==================================================
    // CONFIGURAR PREFIJOS DE GUARDADO
    //==================================================

    public void ConfigurarPrefijosGuardado()
    {
        if (generadorCentro != null)
        {
            generadorCentro.ConfigurarPrefijoGuardado(
                prefijoGuardado + "GenCentro_"
            );
        }

        if (generadorIzquierdaArriba != null)
        {
            generadorIzquierdaArriba.ConfigurarPrefijoGuardado(
                prefijoGuardado + "GenIzqArr_"
            );
        }

        if (generadorIzquierdaAbajo != null)
        {
            generadorIzquierdaAbajo.ConfigurarPrefijoGuardado(
                prefijoGuardado + "GenIzqAba_"
            );
        }

        if (generadorDerechaArriba != null)
        {
            generadorDerechaArriba.ConfigurarPrefijoGuardado(
                prefijoGuardado + "GenDerArr_"
            );
        }

        if (generadorDerechaAbajo != null)
        {
            generadorDerechaAbajo.ConfigurarPrefijoGuardado(
                prefijoGuardado + "GenDerAba_"
            );
        }
    }

    //==================================================
    // GENERACIÓN
    //==================================================

    public void IniciarGeneracion()
    {
        if (generacionActiva)
            return;

        generacionActiva = true;

        foreach (GeneradorNivelPersonalizado generador
                 in generadoresActivos)
        {
            if (generador != null)
            {
                generador.IniciarGeneracion();
            }
        }

        Debug.Log(
            "GeneradorControl -> Generación iniciada en "
            + generadoresActivos.Count
            + " generadores."
        );
    }


    public void DetenerGeneracion()
    {
        if (!generacionActiva)
            return;

        generacionActiva = false;

        foreach (GeneradorNivelPersonalizado generador
                 in generadoresActivos)
        {
            if (generador != null)
            {
                generador.DetenerGeneracion();
            }
        }

        Debug.Log(
            "GeneradorControl -> Generación detenida."
        );
    }


    //==================================================
    // LISTA DE DISCOS
    //==================================================

    public void ActualizarListaDiscos()
    {
        discosActivos.Clear();

        foreach (GeneradorNivelPersonalizado generador
                 in generadoresActivos)
        {
            if (generador == null)
                continue;

            generador.ActualizarListaDiscos();

            foreach (GameObject disco
                     in generador.discosActivos)
            {
                if (disco != null)
                {
                    discosActivos.Add(disco);
                }
            }
        }

        Debug.Log(
            "GeneradorControl -> Lista general actualizada. "
            + "Discos activos: "
            + discosActivos.Count
        );
    }


    //==================================================
    // EVENTO OVERDRIVE
    //==================================================

    public override void ActivarOverdrive(float multiplicador)
    {
        overdriveActivo = true;
        multiplicadorOverdrive = multiplicador;

        foreach (GeneradorNivelPersonalizado generador
                 in generadoresActivos)
        {
            if (generador != null)
            {
                generador.ActivarOverdrive(multiplicador);
            }
        }

        Debug.Log(
            "GeneradorControl -> Overdrive activado."
        );
    }


    public override void DesactivarOverdrive()
    {
        overdriveActivo = false;
        multiplicadorOverdrive = 1f;

        foreach (GeneradorNivelPersonalizado generador
                 in generadoresActivos)
        {
            if (generador != null)
            {
                generador.DesactivarOverdrive();
            }
        }

        Debug.Log(
            "GeneradorControl -> Overdrive desactivado."
        );
    }

    public void MostrarOverdriveVisual()
    {
        foreach (GeneradorNivelPersonalizado generador in generadoresActivos)
        {
            if (generador != null)
            {
                generador.MostrarOverdriveVisual();
            }
        }
    }


    public void OcultarOverdriveVisual()
    {
        foreach (GeneradorNivelPersonalizado generador in generadoresActivos)
        {
            if (generador != null)
            {
                generador.OcultarOverdriveVisual();
            }
        }
    }


    //==================================================
    // RALENTIZACIÓN
    //==================================================

    public override void ActivarRalentizacion(float multiplicador)
    {
        slowActivo = true;
        multiplicadorSlow = multiplicador;

        foreach (GeneradorNivelPersonalizado generador
                 in generadoresActivos)
        {
            if (generador != null)
            {
                generador.ActivarRalentizacion(multiplicador);
            }
        }

        Debug.Log(
            "GeneradorControl -> Ralentización activada."
        );
    }


    public override void DesactivarRalentizacion()
    {
        slowActivo = false;
        multiplicadorSlow = 1f;

        foreach (GeneradorNivelPersonalizado generador
                 in generadoresActivos)
        {
            if (generador != null)
            {
                generador.DesactivarRalentizacion();
            }
        }

        Debug.Log(
            "GeneradorControl -> Ralentización desactivada."
        );
    }

    //==================================================
    // GUARDADO Y RESTAURACION
    //==================================================

    public void GuardarDiscos()
    {
        Debug.Log(
            "===== GUARDANDO DISCOS NIVEL PERSONALIZADO ====="
        );

        foreach (GeneradorNivelPersonalizado generador
                 in generadoresActivos)
        {
            if (generador != null)
            {
                generador.GuardarDiscos();
            }
        }

        Debug.Log(
            "GeneradorControl -> Todos los generadores guardados."
        );
    }

    void GuardarDatosExplosivo(Discos disco, int indice)
    {
        // Se implementará en la Parte 5.
    }

    public void RestaurarDiscos()
    {
        Debug.Log(
           "===== RESTAURANDO DISCOS NIVEL PERSONALIZADO ====="
       );

        discosActivos.Clear();

        foreach (GeneradorNivelPersonalizado generador
                 in generadoresActivos)
        {
            if (generador != null)
            {
                generador.RestaurarDiscos();
            }
        }

        ActualizarListaDiscos();

        Debug.Log(
            "GeneradorControl -> Todos los generadores restaurados."
        );
    }

    void RestaurarDatosExplosivo(Discos disco, int indice)
    {
        // Se implementará en la Parte 5.
    }

    public void ReanudarDiscos()
    {
        foreach (GeneradorNivelPersonalizado generador
                 in generadoresActivos)
        {
            if (generador != null)
            {
                generador.ReanudarDiscos();
            }
        }

        Debug.Log(
            "GeneradorControl -> Discos reanudados."
        );
    }

    public void LimpiarDiscos()
    {
        foreach (GeneradorNivelPersonalizado generador
                 in generadoresActivos)
        {
            if (generador != null)
            {
                generador.LimpiarDiscos();
            }
        }

        discosActivos.Clear();

        Debug.Log(
            "GeneradorControl -> Todos los discos eliminados."
        );
    }

    public void LimpiarDatosGuardado()
    {
        generadorCentro.LimpiarDatosGuardado();
        generadorIzquierdaArriba.LimpiarDatosGuardado();
        generadorIzquierdaAbajo.LimpiarDatosGuardado();
        generadorDerechaArriba.LimpiarDatosGuardado();
        generadorDerechaAbajo.LimpiarDatosGuardado();

        PlayerPrefs.Save();

        Debug.Log("Datos guardados de los 5 generadores eliminados.");
    }
}
