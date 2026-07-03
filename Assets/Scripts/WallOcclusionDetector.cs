using System.Collections.Generic;
using UnityEngine;

public class WallOcclusionDetector : MonoBehaviour
{
    [Header("Configuración de Oclusión")]
    [SerializeField] private Transform objetivoJugador; // Arrastra aquí al jugador
    [SerializeField] private LayerMask capasObstaculos;  // Capa (Layer) donde estarán las paredes
    [SerializeField] private float alfaTranslucido = 0.3f; // Qué tan invisible se vuelve (0 = invisible, 1 = sólido)
    [SerializeField] private float velocidadTransicion = 5f; // Qué tan rápido se desvanece

    private Camera camaraPrincipal;
    private List<RendererOccluido> objetosOccluidosActualmente = new List<RendererOccluido>();
    private List<RendererOccluido> objetosParaLimpiar = new List<RendererOccluido>();

    // Estructura interna para recordar el material original y su transparencia actual
    private struct RendererOccluido
    {
        public Renderer renderer;
        public Material[] materialesOriginales;
        public Material[] materialesClonados;
        public float alfaActual;
    }

    void Start()
    {
        camaraPrincipal = GetComponent<Camera>();
        if (objetivoJugador == null)
        {
            // Intentamos buscar al jugador automáticamente por código si no se asignó en el Inspector
            PlayerMovements jugador = FindAnyObjectByType<PlayerMovements>();
            if (jugador != null) objetivoJugador = jugador.transform;
        }
    }

    void LateUpdate()
    {
        if (objetivoJugador == null || camaraPrincipal == null) return;

        // Limpiamos la lista de objetos que se procesarán en este frame
        objetosParaLimpiar.Clear();
        objetosParaLimpiar.AddRange(objetosOccluidosActualmente);
        objetosOccluidosActualmente.Clear();

        // Calculamos la dirección desde la cámara hacia el jugador
        Vector3 direccion = objetivoJugador.position - transform.position;
        float distancia = direccion.magnitude;

        // Lanzamos un rayo que detecte TODOS los objetos en medio (por si hay más de una pared tapando)
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direccion.normalized, distancia, capasObstaculos);

        foreach (RaycastHit hit in hits)
        {
            Renderer renderizador = hit.collider.GetComponent<Renderer>();
            if (renderizador != null)
            {
                // Buscamos si ya estábamos controlando este objeto antes
                int index = objetosParaLimpiar.FindIndex(x => x.renderer == renderizador);
                RendererOccluido infoObjeto;

                if (index != -1)
                {
                    // Si ya existía, lo rescatamos y lo quitamos de la lista de "limpieza" para que siga translúcido
                    infoObjeto = objetosParaLimpiar[index];
                    objetosParaLimpiar.RemoveAt(index);
                }
                else
                {
                    // Si es una pared nueva que te acaba de tapar, registramos sus materiales originales
                    infoObjeto = new RendererOccluido();
                    infoObjeto.renderer = renderizador;
                    infoObjeto.materialesOriginales = renderizador.sharedMaterials;
                    infoObjeto.alfaActual = 1f;

                    // Clonamos los materiales para poder editarlos de forma única sin romper el prefab original
                    infoObjeto.materialesClonados = new Material[infoObjeto.materialesOriginales.Length];
                    for (int i = 0; i < infoObjeto.materialesOriginales.Length; i++)
                    {
                        infoObjeto.materialesClonados[i] = new Material(infoObjeto.materialesOriginales[i]);
                        
                        // Configuración en tiempo de ejecución para que el shader de Unity acepte transparencias (Fade)
                        ConfigurarMaterialTranslucido(infoObjeto.materialesClonados[i]);
                    }
                    renderizador.materials = infoObjeto.materialesClonados;
                }

                // Transición suave hacia la transparencia deseada
                infoObjeto.alfaActual = Mathf.MoveTowards(infoObjeto.alfaActual, alfaTranslucido, velocidadTransicion * Time.deltaTime);
                AplicarAlfa(infoObjeto.materialesClonados, infoObjeto.alfaActual);

                objetosOccluidosActualmente.Add(infoObjeto);
            }
        }

        // Devolvemos a la normalidad los objetos que ya NO están tapando al jugador
        foreach (RendererOccluido objetoSolido in objetosParaLimpiar)
        {
            RendererOccluido infoObjeto = objetoSolido;
            infoObjeto.alfaActual = Mathf.MoveTowards(infoObjeto.alfaActual, 1f, velocidadTransicion * Time.deltaTime);
            AplicarAlfa(infoObjeto.materialesClonados, infoObjeto.alfaActual);

            if (infoObjeto.alfaActual >= 0.99f)
            {
                // Cuando vuelve a ser 100% sólido, le regresamos sus materiales originales intactos para ahorrar memoria
                if (infoObjeto.renderer != null)
                {
                    infoObjeto.renderer.materials = infoObjeto.materialesOriginales;
                }
            }
            else
            {
                // Si aún está desvaneciéndose para volverse sólido, lo mantenemos en observación un frame más
                objetosOccluidosActualmente.Add(infoObjeto);
            }
        }
    }

    private void AplicarAlfa(Material[] materiales, float alfa)
    {
        foreach (Material mat in materiales)
        {
            if (mat != null)
            {
                // El color estándar en la mayoría de shaders de Unity se llama "_Color" o "_BaseColor" (en URP)
                Color colorActual = mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") : mat.color;
                colorActual.a = alfa;

                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", colorActual);
                else mat.color = colorActual;
            }
        }
    }

    private void ConfigurarMaterialTranslucido(Material mat)
    {
        // URP (Universal Render Pipeline)
        if (mat.HasProperty("_Surface")) 
        {
            mat.SetFloat("_Surface", 1); 
            mat.SetFloat("_AlphaClip", 0);
            mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetFloat("_ZWrite", 0);
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
        // Built-in (Estándar)
        else 
        {
            mat.SetFloat("_Mode", 2);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
    }
}