using System;
using UnityEngine;

[Serializable]
public struct MapeamentoLane
{
    public PosicaoLane posicao; // O dropdown com Cima, Baixo, Esquerda, Direita
    public Lane laneAtribuida;  // O espaço para você arrastar o GameObject da Lane
}
