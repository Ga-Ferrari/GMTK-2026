using System;
using UnityEngine;

public enum TipoInimigo { Normal = 0, Ninja = 1, Beefy = 2 }

[Serializable]
public class InimigoPosicao
{
    public int BatidaPosicionar; 
    public PosicaoLane lanePosicionar;
    public TipoInimigo tipo; // Atualizado para usar o Enum
}
