using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Secuencia : MonoBehaviour
{
    [SerializeField] private GameObject m_box1;
    [SerializeField] private GameObject m_box2;
    [SerializeField] private GameObject m_box3;
    [SerializeField] private GameObject m_box4;
    [SerializeField] private Material m_amarillo;
    [SerializeField] private Material m_blanco;
    [SerializeField] private Material m_verde;
    [SerializeField] private Material m_rojo;
    [SerializeField] private int m_secuenciasParaGanar = 3;

    private System.Random m_numeroAleatorio = new System.Random();
    private GameObject[] m_arrayBoxes;
    private Renderer[] m_arrayBoxRenders;
    private int[] m_arraySecuencia = new int[3];
    private int m_pasoActual = 0;
    private int m_puntos = 0;
    private int m_secuenciasSuperadas = 0;
    private bool m_bloqueado = false;
    private bool m_aceptandoEntrada = false;
    private bool m_mostrandoSecuencia = false;
    private int m_ultimoFramePulsado = -1;

    private void Awake()
    {
        m_arrayBoxes = new GameObject[]
        {
            m_box1,
            m_box2,
            m_box3,
            m_box4
        };

        m_arrayBoxRenders = new Renderer[]
        {
            m_box1.GetComponent<Renderer>(),
            m_box2.GetComponent<Renderer>(),
            m_box3.GetComponent<Renderer>(),
            m_box4.GetComponent<Renderer>()
        };

        PintarTodas(m_blanco);
    }

    private void Update()
    {
        if (!m_bloqueado && !m_mostrandoSecuencia && !m_aceptandoEntrada && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            EjecutarSecuencia();
        }
    }

    public void EjecutarSecuencia()
    {
        if (m_bloqueado)
        {
            return;
        }

        StopAllCoroutines();
        m_pasoActual = 0;
        m_aceptandoEntrada = false;
        m_mostrandoSecuencia = true;
        PintarTodas(m_blanco);

        int[] secuencia = GenerarArraySecuencia();
        StartCoroutine(IniciarSecuencia(secuencia));
    }

    private int[] GenerarArraySecuencia()
    {
        int chivato = -1;

        for (int i = 0; i < m_arraySecuencia.Length; i++)
        {
            do
            {
                int numeroElegido = m_numeroAleatorio.Next(0, m_arrayBoxes.Length);
                m_arraySecuencia[i] = numeroElegido;
            }
            while (chivato == m_arraySecuencia[i]);

            chivato = m_arraySecuencia[i];
        }

        return m_arraySecuencia;
    }

    private IEnumerator IniciarSecuencia(int[] arraySecuencia)
    {
        for (int i = 0; i < arraySecuencia.Length; i++)
        {
            int indiceCaja = arraySecuencia[i];
            m_arrayBoxRenders[indiceCaja].material = m_amarillo;
            yield return new WaitForSeconds(1f);
            m_arrayBoxRenders[indiceCaja].material = m_blanco;
            yield return new WaitForSeconds(0.15f);
        }

        m_mostrandoSecuencia = false;
        m_aceptandoEntrada = true;
    }

    public bool EstaBloqueado()
    {
        return m_bloqueado;
    }

    public bool PuedePulsarCajas()
    {
        return !m_bloqueado && m_aceptandoEntrada;
    }

    public void PulsarCaja(GameObject cajaPulsada)
    {
        int idCaja = ObtenerIndiceCaja(cajaPulsada);

        if (idCaja == -1)
        {
            return;
        }

        PulsarCaja(idCaja);
    }

    public void PulsarCaja(int idCaja)
    {
        if (!PuedePulsarCajas())
        {
            return;
        }

        if (m_ultimoFramePulsado == Time.frameCount)
        {
            return;
        }

        m_ultimoFramePulsado = Time.frameCount;

        if (idCaja < 0 || idCaja >= m_arrayBoxRenders.Length)
        {
            return;
        }

        if (m_arraySecuencia[m_pasoActual] == idCaja)
        {
            m_arrayBoxRenders[idCaja].material = m_verde;
            m_puntos++;
            m_pasoActual++;

            if (m_pasoActual >= m_arraySecuencia.Length)
            {
                m_secuenciasSuperadas++;
                m_aceptandoEntrada = false;

                if (m_secuenciasSuperadas >= m_secuenciasParaGanar)
                {
                    m_bloqueado = true;
                    PintarTodas(m_verde);
                    Debug.Log($"Has ganado. Secuencias superadas: {m_secuenciasSuperadas}. Puntos: {m_puntos}");
                    return;
                }

                PintarTodas(m_blanco);
                Debug.Log($"Secuencia superada: {m_secuenciasSuperadas}/{m_secuenciasParaGanar}. Puntos: {m_puntos}");
            }
        }
        else
        {
            m_bloqueado = true;
            m_aceptandoEntrada = false;
            PintarTodas(m_rojo);
        }
    }

    private void PintarTodas(Material material)
    {
        for (int i = 0; i < m_arrayBoxRenders.Length; i++)
        {
            m_arrayBoxRenders[i].material = material;
        }
    }

    private int ObtenerIndiceCaja(GameObject cajaPulsada)
    {
        if (cajaPulsada == null)
        {
            return -1;
        }

        for (int i = 0; i < m_arrayBoxes.Length; i++)
        {
            if (cajaPulsada == m_arrayBoxes[i])
            {
                return i;
            }

            if (cajaPulsada.transform.IsChildOf(m_arrayBoxes[i].transform) || m_arrayBoxes[i].transform.IsChildOf(cajaPulsada.transform))
            {
                return i;
            }
        }

        return -1;
    }
}