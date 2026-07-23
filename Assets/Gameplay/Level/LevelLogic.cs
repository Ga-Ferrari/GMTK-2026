using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum PosicaoLane
{
    cima = 0,
    baixo = 1,
    esquerda = 2,
    direita = 3
}

public class LevelLogic : MonoBehaviour
{


    [Header("As lanes que existem no mapa")]
    [SerializeField]public List<MapeamentoLane> lanes;


    [Header("Configuracoes Do Level")]
    [Header("BPM")]
    public int bpm;
    [Header("Duração")]
    public int duracao; //Em segundos
    [Header("Tempo de aparecimento do inimigo (em batidas)")]
    public int tempoInimigo = 9;
    public float segPorBatida = 0;

    [SerializeField] private List<InimigoPosicao> inimigos;


    [NonSerialized]public int batidasLevel;

    private float timerLevel;
    
    [NonSerialized] public int batidaAtual;

    public UnityEvent<int> PassouBatida;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        batidasLevel = (int)(duracao/60f)*bpm;
        timerLevel = 0.0f;
        batidaAtual = 0;
        segPorBatida = 60f/bpm;
        PosicionarInimigos();
    }

    

    // Update is called once per frame
    void Update()
    {
        timerLevel+= Time.deltaTime;
        int batidaCalculada = (int)math.floor(timerLevel / segPorBatida);
        if (batidaAtual != batidaCalculada)
        {  
            batidaAtual = (int)math.floor(timerLevel/((float)duracao/(float)batidasLevel));
            PassouBatida?.Invoke(batidaAtual);
            foreach (MapeamentoLane mapLane in lanes)
            {
                mapLane.laneAtribuida.OnBeat(batidaAtual);
            }
        }
    }


    private void PosicionarInimigos()
    {
        inimigos.Sort((a, b) => a.BatidaPosicionar.CompareTo(b.BatidaPosicionar));
        foreach(InimigoPosicao inimigo in inimigos)
        {
            PegarLanePorPosicao(inimigo.lanePosicionar).AddInimigo(inimigo);
        }
    }

    public Lane PegarLanePorPosicao(PosicaoLane posicaoDesejada)
    {
        foreach (var config in lanes)
        {
            if (config.posicao == posicaoDesejada)
            {
                return config.laneAtribuida;
            }
        }
        
        Debug.LogWarning("Nenhuma Lane foi configurada para a posição: " + posicaoDesejada);
        return null;
    }

}
