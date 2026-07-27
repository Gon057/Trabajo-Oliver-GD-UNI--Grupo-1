using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneradorNivel2 : GeneradorBase
{
    [Header("Prefabs de discos")]
    public GameObject discoNormalPrefab;
    public GameObject discoRapidoPrefab;
    public GameObject discoPesadoPrefab;

    [Header("Tiempo de generación")]
    public float tiempoMinimo = 0.5f;
    public float tiempoMaximo = 1.0f;

    [Header("Radio de aparición")]
    public float radioGeneracion = 0.5f;

    // Lista de todos los discos activos (servirá para guardado y eventos)
    public List<GameObject> discosActivos = new List<GameObject>();

    // Prefijo para diferenciar datos entre niveles
    private string prefijoGuardado = "Nivel2_";
    private Coroutine rutinaGeneracion;

    private bool overdriveActivo = false;
    private float multiplicadorActual = 1f;

    private bool slowActivo = false;
    private float multiplicadorSlow = 1f;

    private float probabilidadSegundoDisco = 0f;
    private float probabilidadTercerDisco = 0f;
    [SerializeField] public Nivel2 nivel;

    public void IniciarGeneracion()
    {
        if (rutinaGeneracion == null)
        {
            rutinaGeneracion = StartCoroutine(GenerarDiscos());
        }
    }

    public void DetenerGeneracion()
    {
        if (rutinaGeneracion != null)
        {
            StopCoroutine(rutinaGeneracion);
            rutinaGeneracion = null;
        }
    }

    void ActualizarDificultad()
    {
        float tiempo = nivel.ObtenerTiempo();

        //-------------------------
        // 0 - 30 segundos
        //-------------------------
        if (tiempo < 30f)
        {
            tiempoMinimo = 2.0f;
            tiempoMaximo = 3.0f;

            probabilidadSegundoDisco = 0f;
            probabilidadTercerDisco = 0f;
        }

        //-------------------------
        // 30 - 60 segundos
        //-------------------------
        else if (tiempo < 60f)
        {
            tiempoMinimo = 1.6f;
            tiempoMaximo = 2.3f;

            probabilidadSegundoDisco = 0.10f;
            probabilidadTercerDisco = 0f;
        }

        //-------------------------
        // 60 - 90 segundos
        //-------------------------
        else if (tiempo < 90f)
        {
            tiempoMinimo = 1.2f;
            tiempoMaximo = 1.8f;

            probabilidadSegundoDisco = 0.25f;
            probabilidadTercerDisco = 0.03f;
        }

        //-------------------------
        // 90 - 120 segundos
        //-------------------------
        else if (tiempo < 120f)
        {
            tiempoMinimo = 0.9f;
            tiempoMaximo = 1.5f;

            probabilidadSegundoDisco = 0.40f;
            probabilidadTercerDisco = 0.08f;
        }

        //-------------------------
        // 120 segundos en adelante
        //-------------------------
        else
        {
            tiempoMinimo = 0.6f;
            tiempoMaximo = 1.1f;

            probabilidadSegundoDisco = 0.60f;
            probabilidadTercerDisco = 0.15f;
        }
    }

    IEnumerator GenerarDiscos()
    {
        while (true)
        {
            // Ajustar dificultad según el tiempo de partida
            ActualizarDificultad();

            float espera = Random.Range(tiempoMinimo, tiempoMaximo);

            yield return new WaitForSeconds(espera);

            GenerarDisco();

            // Posibilidad de un segundo disco
            if (Random.value < probabilidadSegundoDisco)
            {
                GenerarDisco();
            }

            // Posibilidad de un tercer disco
            if (Random.value < probabilidadTercerDisco)
            {
                GenerarDisco();
            }
        }
    }

    void GenerarDisco()
    {
        Vector2 posicionAleatoria =
            (Vector2)transform.position +
            Random.insideUnitCircle * radioGeneracion;

        // Elegimos aleatoriamente el tipo de disco
        int tipo = Random.Range(0, 3);

        GameObject prefabSeleccionado = null;

        switch (tipo)
        {
            case 0:
                prefabSeleccionado = discoNormalPrefab;
                break;

            case 1:
                prefabSeleccionado = discoRapidoPrefab;
                break;

            case 2:
                prefabSeleccionado = discoPesadoPrefab;
                break;
        }

        GameObject nuevoDisco =
            Instantiate(prefabSeleccionado,
                        posicionAleatoria,
                        Quaternion.identity);

        // Obtener el script del disco
        Discos script = nuevoDisco.GetComponent<Discos>();

        // Inicializar el disco recién generado
        script.InicializarDisco(true);

        // Si el evento está activo, aplicar también al disco nuevo
        if (overdriveActivo)
        {
            script.ActivarOverdrive(multiplicadorActual);
        }

        //Si la habilidad Ralentizar está activa, aplicar también al disco nuevo
        if (slowActivo)
        {
            script.ActivarRalentizacion(multiplicadorSlow);
        }

        discosActivos.Add(nuevoDisco);
    }

    public void GuardarDiscos()
    {
        ActualizarListaDiscos();

        PlayerPrefs.SetInt(prefijoGuardado + "CantidadDiscos", discosActivos.Count);

        for (int i = 0; i < discosActivos.Count; i++)
        {
            Discos disco = discosActivos[i].GetComponent<Discos>();

            PlayerPrefs.SetInt(
                prefijoGuardado + "Tipo_" + i,
                (int)disco.ObtenerTipo());

            PlayerPrefs.SetFloat(
                prefijoGuardado + "PosX_" + i,
                disco.ObtenerPosicion().x);

            PlayerPrefs.SetFloat(
                prefijoGuardado + "PosY_" + i,
                disco.ObtenerPosicion().y);

            PlayerPrefs.SetFloat(
                prefijoGuardado + "DirX_" + i,
                disco.ObtenerDireccion().x);

            PlayerPrefs.SetFloat(
                prefijoGuardado + "DirY_" + i,
                disco.ObtenerDireccion().y);
        }

        PlayerPrefs.Save();
    }

    public void RestaurarDiscos()
    {
        int cantidad =
            PlayerPrefs.GetInt(prefijoGuardado + "CantidadDiscos", 0);

        for (int i = 0; i < cantidad; i++)
        {
            int tipo =
                PlayerPrefs.GetInt(prefijoGuardado + "Tipo_" + i);

            GameObject prefab = discoNormalPrefab;

            switch ((Discos.TipoDisco)tipo)
            {
                case Discos.TipoDisco.Normal:
                    prefab = discoNormalPrefab;
                    break;

                case Discos.TipoDisco.Rapido:
                    prefab = discoRapidoPrefab;
                    break;

                case Discos.TipoDisco.Pesado:
                    prefab = discoPesadoPrefab;
                    break;
            }

            Vector2 posicion = new Vector2(
                PlayerPrefs.GetFloat(prefijoGuardado + "PosX_" + i),
                PlayerPrefs.GetFloat(prefijoGuardado + "PosY_" + i));

            Vector2 direccion = new Vector2(
                PlayerPrefs.GetFloat(prefijoGuardado + "DirX_" + i),
                PlayerPrefs.GetFloat(prefijoGuardado + "DirY_" + i));

            GameObject nuevoDisco =
                Instantiate(prefab, posicion, Quaternion.identity);

            Discos script = nuevoDisco.GetComponent<Discos>();

            script.RestaurarDisco(posicion, direccion, false);

            discosActivos.Add(nuevoDisco);
        }
    }

    public void ReanudarDiscos()
    {
        foreach (GameObject disco in discosActivos)
        {
            if (disco != null)
            {
                Discos script = disco.GetComponent<Discos>();
                script.ReanudarMovimiento();
            }
        }
    }

    public void LimpiarDiscos()
    {
        foreach (GameObject disco in discosActivos)
        {
            Destroy(disco);
        }

        discosActivos.Clear();
    }

    public void ActualizarListaDiscos()
    {
        for (int i = discosActivos.Count - 1; i >= 0; i--)
        {
            if (discosActivos[i] == null)
            {
                discosActivos.RemoveAt(i);
            }
        }
    }

    public override void ActivarOverdrive(float multiplicador)
    {
        overdriveActivo = true;
        multiplicadorActual = multiplicador;
        foreach (GameObject disco in discosActivos)
        {
            if (disco != null)
            {
                disco.GetComponent<Discos>().ActivarOverdrive(multiplicador);
            }
        }
    }

    public override void DesactivarOverdrive()
    {
        overdriveActivo = false;
        multiplicadorActual = 1f;

        foreach (GameObject disco in discosActivos)
        {
            if (disco != null)
            {
                disco.GetComponent<Discos>().DesactivarOverdrive();
            }
        }
    }

    public void MostrarOverdriveVisual()
    {
        foreach (GameObject disco in discosActivos)
        {
            if (disco != null)
            {
                disco.GetComponent<Discos>().MostrarOverdriveVisual();
            }
        }
    }

    public void OcultarOverdriveVisual()
    {
        foreach (GameObject disco in discosActivos)
        {
            if (disco != null)
            {
                disco.GetComponent<Discos>().OcultarOverdriveVisual();
            }
        }
    }

    //Efecto de Ralentización a Discos

    public override void ActivarRalentizacion(float multiplicador)
    {
        slowActivo = true;
        multiplicadorSlow = multiplicador;

        foreach (GameObject disco in discosActivos)
        {
            if (disco != null)
            {
                disco.GetComponent<Discos>().ActivarRalentizacion(multiplicador);
            }
        }
    }

    public override void DesactivarRalentizacion()
    {
        slowActivo = false;
        multiplicadorSlow = 1f;

        foreach (GameObject disco in discosActivos)
        {
            if (disco != null)
            {
                disco.GetComponent<Discos>().DesactivarRalentizacion();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
