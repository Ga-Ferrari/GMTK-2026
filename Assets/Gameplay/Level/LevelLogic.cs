using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public enum PosicaoLane { cima = 0, baixo = 1, esquerda = 2, direita = 3 }

public class LevelLogic : MonoBehaviour
{
    [Header("As lanes que existem no mapa")]
    [SerializeField] public List<MapeamentoLane> lanes;

    [Header("BPM")]
    public int bpm;
    [Header("Duração (Apenas para Fase Normal)")]
    public int duracao; 
    [Header("Tempo de aparecimento do inimigo (em batidas)")]
    public int tempoInimigo = 9;
    public float segPorBatida = 0;

    [Header("Inimigos (Apenas para Fase Normal)")]
    [SerializeField] private List<InimigoPosicao> inimigos;

    [Header("Dificuldade - Modo Infinito")]
    [Range(0f, 1f)]
    public float chanceDeSpawn = 0.15f; 
    public int aumentarDificuldadeAcadaXBatidas = 30; 
    
    // Variável de controle para o respiro no começo
    private int ultimaBatidaComSpawn = -99; 

    // Variável interna decidida pelo Menu Principal
    private bool modoInfinitoAtivado;

    [NonSerialized] public int batidasLevel;
    public float timerLevel;
    [NonSerialized] public int batidaAtual;

    public UnityEvent<int> PassouBatida;

    void Start()
    {
        timerLevel = 0.0f;
        batidaAtual = 0;
        segPorBatida = 60f / bpm;

        // Puxa da memória o que o jogador escolheu no Menu (padrão é 0/Normal caso não ache)
        modoInfinitoAtivado = PlayerPrefs.GetInt("ModoInfinito", 0) == 1;

        if (!modoInfinitoAtivado)
        {
            batidasLevel = (int)(duracao / 60f) * bpm;
            PosicionarInimigos();
        }
    }

    void Update()
    {
        timerLevel += Time.deltaTime;
        
        int batidaCalculada = (int)math.floor(timerLevel / segPorBatida);
        
        if (batidaAtual != batidaCalculada)
        {  
            batidaAtual = batidaCalculada;
            PassouBatida?.Invoke(batidaAtual);
            
            if (modoInfinitoAtivado)
            {
                ProcessarModoInfinito();
            }
            else
            {
                ProcessarModoNormal();
            }
        }
    }

    private void ProcessarModoNormal()
    {
        foreach (MapeamentoLane mapLane in lanes)
        {
            mapLane.laneAtribuida.OnBeat(batidaAtual);
        }
    }

    private void ProcessarModoInfinito()
    {
        AumentarDificuldade();
        GerarInimigosInfinito();

        foreach (MapeamentoLane mapLane in lanes)
        {
            mapLane.laneAtribuida.OnBeat(batidaAtual);
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

    private void GerarInimigosInfinito()
    {
        int batidaAlvo = batidaAtual + tempoInimigo;

        //Proíbe inimigos colados no início
        if (chanceDeSpawn < 0.35f && batidaAtual - ultimaBatidaComSpawn < 2)
        {
            return; 
        }

        if (UnityEngine.Random.value <= chanceDeSpawn)
        {
            PosicaoLane posicaoAleatoria = (PosicaoLane)UnityEngine.Random.Range(0, 4);
            Lane laneSorteada = PegarLanePorPosicao(posicaoAleatoria);

            if (laneSorteada != null)
            {
                // Lógica para sortear os inimigos especiais baseados no tempo (batidaAtual)
                TipoInimigo tipoSorteado = TipoInimigo.Normal;
                float sorteioInimigo = UnityEngine.Random.value;

                // A partir da batida 30, tem 30% de chance de ser Ninja
                if (batidaAtual > 30 && sorteioInimigo > 0.7f) tipoSorteado = TipoInimigo.Ninja;
                // A partir da batida 50, tem 20% de chance de ser Beefy
                else if (batidaAtual > 50 && sorteioInimigo > 0.8f) tipoSorteado = TipoInimigo.Beefy;

                InimigoPosicao novoInimigo = new InimigoPosicao
                {
                    BatidaPosicionar = batidaAlvo,
                    lanePosicionar = posicaoAleatoria,
                    tipo = tipoSorteado // Usa o TipoInimigo
                };
                
                // Tenta adicionar e, se for aceito pela Lane, registra a batida
                bool inimigoFoiAceito = laneSorteada.AddInimigo(novoInimigo);
                if (inimigoFoiAceito)
                {
                    ultimaBatidaComSpawn = batidaAtual; 
                }
            }
        }
    }

    private void AumentarDificuldade()
    {
        if (batidaAtual > 0 && batidaAtual % aumentarDificuldadeAcadaXBatidas == 0)
        {
            chanceDeSpawn = math.min(chanceDeSpawn + 0.05f, 0.85f);
            
            if (tempoInimigo > 4 && batidaAtual % (aumentarDificuldadeAcadaXBatidas * 2) == 0)
            {
                tempoInimigo--;
            }
        }
    }

    public Lane PegarLanePorPosicao(PosicaoLane posicaoDesejada)
    {
        foreach (var config in lanes)
        {
            if (config.posicao == posicaoDesejada) return config.laneAtribuida;
        }
        return null;
    }
}