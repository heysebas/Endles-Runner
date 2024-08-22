using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TipoBloque
{
    Normal,
    FullVagones,
    Trenes
}

public class Bloque : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private TipoBloque tipoBloque;
    [SerializeField] private bool tieneRampa;
    
    [Header("Tren")]
    [SerializeField] private Tren[] trenes;

    public TipoBloque TipoDeBloque => tipoBloque;
    public bool TieneRampa => tieneRampa;

    private Tren trenSeleccionado;
    
    public void InicializarBloque()
    {
        if (tipoBloque == TipoBloque.Trenes)
        {
            SeleccionarTren();
        }
    }
    
    private void SeleccionarTren()
    {
        if (trenes == null || trenes.Length == 0)
        {
            return;
        }
        
        int index = Random.Range(0, trenes.Length);
        trenes[index].gameObject.SetActive(true);
        trenSeleccionado = trenes[index];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (trenSeleccionado != null)
            {
                trenSeleccionado.PuedeMoverse = true;
                trenSeleccionado.Player = other.GetComponent<PlayerController>();
            }
        }
    }
}
