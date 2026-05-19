using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Pulsador : MonoBehaviour
{
    [SerializeField] private Secuencia _secuencia;

    private XRBaseInteractable m_interactable;

    private void Awake()
    {
        m_interactable = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        m_interactable.selectEntered.AddListener(OnCajaSeleccionadaEnVR);
    }

    private void OnDisable()
    {
        m_interactable.selectEntered.RemoveListener(OnCajaSeleccionadaEnVR);
    }

    private void OnCajaSeleccionadaEnVR(SelectEnterEventArgs args)
    {
        IntentarPulsarCaja();
    }

    private void IntentarPulsarCaja()
    {
        if (!_secuencia.PuedePulsarCajas())
        {
            return;
        }

        _secuencia.PulsarCaja(gameObject);
    }
}