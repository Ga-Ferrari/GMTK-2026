using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lane : MonoBehaviour
{
    private Dictionary<int, InimigoPosicao> mapaInimigos = new Dictionary<int, InimigoPosicao>();
    private Queue<InimigoRitmico> InimigosAtivos = new Queue<InimigoRitmico>();
    
    [Header("Referências")]
    [SerializeField] private LevelLogic level;
    
    [Header("Identificação desta Lane")]
    public PosicaoLane posicaoDestaLane;

    [Header("Prefabs dos Inimigos")]
    [Tooltip("Arraste os Prefabs na ordem: Element 0 = Normal, Element 1 = Ninja, Element 2 = Beefy")]
    public GameObject[] inimigoPrefabs; 

    [Header("Posições de Movimento")]
    [SerializeField] private Transform inicioLane;
    [SerializeField] private Transform fimLane;

    [Header("Espaçamento de Inimigos")]
    public int espacoMinimoEmBatidas = 2; 
    private int ultimaBatidaAdicionada = -999; 

    private AnimacaoAcerto animador;

    void Start()
    {
        animador = GetComponent<AnimacaoAcerto>();
    }

    // O SpawnarInimigoVisual agora recebe a InimigoPosicao inteira para saber o Tipo (Normal, Ninja ou Beefy)
    public void SpawnarInimigoVisual(InimigoPosicao infoInimigo)
    {
        // Proteção para caso o Array esteja vazio no Inspector
        if (inimigoPrefabs == null || inimigoPrefabs.Length == 0)
        {
            Debug.LogError("Você esqueceu de colocar os Prefabs no array 'Inimigo Prefabs' da Lane!");
            return;
        }

        int indicePrefab = (int)infoInimigo.tipo;
        
        // Evita erros se você tentar spawnar um tipo que não tem prefab configurado
        if (indicePrefab >= inimigoPrefabs.Length) 
        {
            indicePrefab = 0; 
        }

        // Instancia o inimigo correto e pega o script dele
        GameObject prefabEscolhido = inimigoPrefabs[indicePrefab];
        GameObject novoInimigoObj = Instantiate(prefabEscolhido, inicioLane.position, Quaternion.identity);
        InimigoRitmico scriptInimigo = novoInimigoObj.GetComponent<InimigoRitmico>();

        // Configura todas as variáveis do inimigo
        scriptInimigo.SpB = level.segPorBatida;
        scriptInimigo.batidaAtk = infoInimigo.BatidaPosicionar;
        scriptInimigo.tipoInimigo = infoInimigo.tipo; 
        scriptInimigo.tomouDano.AddListener(animador.animarAcerto);
        scriptInimigo.tomouDano.AddListener(teste);
        scriptInimigo.ConfigurarMovimento(inicioLane.position, fimLane.position, level.tempoInimigo);
        scriptInimigo.DefinirSpritePorLane(posicaoDestaLane);
        
        InimigosAtivos.Enqueue(scriptInimigo);
    }

    public void OnBeat(int batidaAtual)
    {
        int batidaParaSpawnar = batidaAtual + level.tempoInimigo;

        if (mapaInimigos.ContainsKey(batidaParaSpawnar))
        {
            SpawnarInimigoVisual(mapaInimigos[batidaParaSpawnar]); 
        }
        
        while (InimigosAtivos.Count > 0)
        {
            InimigoRitmico inimigoTopo = InimigosAtivos.Peek();

            // Lógica para saber se deixou passar (O Beefy tem uma batida extra de limite por causa do Hold)
            int batidaLimite = inimigoTopo.tipoInimigo == TipoInimigo.Beefy && inimigoTopo.sendoSegurado 
                               ? inimigoTopo.batidaParaSoltar 
                               : inimigoTopo.batidaAtk;

            if (batidaAtual > batidaLimite)
            {
                Debug.Log("Deixou o inimigo passar ou esqueceu de soltar o Beefy! Perdeu vida.");
                GameManager.instance.PerderVida();
                inimigoTopo.ForcarMortePorPassarDoTempo(); 
                InimigosAtivos.Dequeue(); 
            }
            else
            {
                break; // Se o primeiro da fila ainda está dentro do tempo, podemos parar de checar
            }
        }

        foreach (InimigoRitmico inimigo in InimigosAtivos)
        {
            inimigo.OnBeat(batidaAtual);
        }
    }
    
    // CHAMADO QUANDO O JOGADOR APERTA A TECLA
    public void AtacarInimigo(float tempoDoAtk)
    {
        if (InimigosAtivos.Count > 0)
        {
            InimigoRitmico inimigo = InimigosAtivos.Peek();
            
            if (inimigo.tipoInimigo == TipoInimigo.Beefy)
            {
                inimigo.IniciarHold(tempoDoAtk); // Inicia a mecânica de segurar botão
            }
            else
            {
                inimigo.tomarDano(tempoDoAtk); // Dano normal / kill
                InimigosAtivos.Dequeue();
            }
        }
    }

    // CHAMADO QUANDO O JOGADOR SOLTA A TECLA
    public void SoltarAtaque(float tempoDoAtk)
    {
        if (InimigosAtivos.Count > 0)
        {
            InimigoRitmico inimigo = InimigosAtivos.Peek();
            if (inimigo.tipoInimigo == TipoInimigo.Beefy && inimigo.sendoSegurado)
            {
                inimigo.FinalizarHold(tempoDoAtk);
                InimigosAtivos.Dequeue();
            }
        }
    }

    

    public void teste(TipoDeAcerto acerto)
    {
        Debug.Log(acerto);
        if (acerto == TipoDeAcerto.MuitoAdiantado || acerto == TipoDeAcerto.MuitoAtrasado)
        {
            GameManager.instance.PerderVida();
        }
    }

    // Tenta adicionar o inimigo verificando a distância mínima
    public bool AddInimigo(InimigoPosicao inimigo)
    {
        if (Mathf.Abs(inimigo.BatidaPosicionar - ultimaBatidaAdicionada) < espacoMinimoEmBatidas) 
        {
            return false; // Rejeitou: Muito perto do último
        }

        if (!mapaInimigos.ContainsKey(inimigo.BatidaPosicionar))
        {
            mapaInimigos.Add(inimigo.BatidaPosicionar, inimigo);
            ultimaBatidaAdicionada = inimigo.BatidaPosicionar; 
            return true; // Aceitou
        }
        
        return false;
    }
}