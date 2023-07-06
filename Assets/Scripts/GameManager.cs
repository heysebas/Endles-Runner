using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EstadoDelJuego
{
    Inicio,
    Jugando,
    GameOver
}

public class GameManager : Singleton <GameManager>
{

    public EstadoDelJuego EstadoActual { get; set; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CambiarEstado(EstadoDelJuego.Jugando);
        }
    }

    public void CambiarEstado(EstadoDelJuego nuevoEstado)
    {
        if (EstadoActual != nuevoEstado)
        {
            EstadoActual = nuevoEstado;
        }
    }
}
