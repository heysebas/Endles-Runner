using System;
using UnityEngine;

public enum EstadosDelJuego
{
    Inicio,
    Jugando,
    GameOver
}

public class GameManager : Singleton<GameManager>
{
    public EstadosDelJuego EstadoActual { get; set; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CambiarEstado(EstadosDelJuego.Jugando);
        }
    }

    public void CambiarEstado(EstadosDelJuego nuevoEstado)
    {
        if (EstadoActual != nuevoEstado)
        {
            EstadoActual = nuevoEstado;
        }
    }
}
