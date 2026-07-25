using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform player;

    [Header("Movimiento")]
    [SerializeField] private float velocidadSeguimiento = 8f;

    [Header("Límites del mapa")]
    [SerializeField] private SpriteRenderer fondoNivel;

    private Camera cam;

    private float mitadAlto;
    private float mitadAncho;

    private float limiteIzquierdo;
    private float limiteDerecho;
    private float limiteInferior;
    private float limiteSuperior;

    void Start()
    {
        cam = GetComponent<Camera>();

        CalcularLimites();
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        Vector3 destino = player.position;

        destino.z = transform.position.z;

        destino.x = Mathf.Clamp(
            destino.x,
            limiteIzquierdo,
            limiteDerecho);

        destino.y = Mathf.Clamp(
            destino.y,
            limiteInferior,
            limiteSuperior);

        transform.position =
            Vector3.Lerp(
                transform.position,
                destino,
                velocidadSeguimiento * Time.deltaTime);
    }

    void CalcularLimites()
    {
        mitadAlto = cam.orthographicSize;
        mitadAncho = mitadAlto * cam.aspect;

        Bounds b = fondoNivel.bounds;

        limiteIzquierdo = b.min.x + mitadAncho;
        limiteDerecho = b.max.x - mitadAncho;

        limiteInferior = b.min.y + mitadAlto;
        limiteSuperior = b.max.y - mitadAlto;

        if (limiteIzquierdo > limiteDerecho)
        {
            float centro = b.center.x;

            limiteIzquierdo = centro;
            limiteDerecho = centro;
        }

        if (limiteInferior > limiteSuperior)
        {
            float centro = b.center.y;

            limiteInferior = centro;
            limiteSuperior = centro;
        }
    }
}
