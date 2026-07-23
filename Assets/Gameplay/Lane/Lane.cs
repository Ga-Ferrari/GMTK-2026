using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    // O seu novo "Map" (Dicionário). A chave (int) é a batida exata em que ele deve ser acertado.
    private Dictionary<int, InimigoPosicao> mapaInimigos = new Dictionary<int, InimigoPosicao>();

    private Queue<InimigoRitmico> InimigosAtivos = new Queue<InimigoRitmico>();
    
    [SerializeField] private LevelLogic level;
    [SerializeField] private GameObject inimigoPrefab;

    [SerializeField] private Transform inicioLane;
    [SerializeField] private Transform fimLane;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SpawnarInimigoVisual(int batida)
    {
        GameObject novoInimigoObj = Instantiate(inimigoPrefab, inicioLane.position, Quaternion.identity);
        InimigoRitmico scriptInimigo = novoInimigoObj.GetComponent<InimigoRitmico>();

        scriptInimigo.SpB = level.segPorBatida;
        scriptInimigo.batidaAtk = batida;
        scriptInimigo.tomouDano.AddListener(teste);
        
        InimigosAtivos.Enqueue(scriptInimigo);
    }

    public void OnBeat(int batidaAtual)
    {
        // 1. Qual é a batida que devemos observar AGORA?
        // Se o inimigo demora 9 batidas para chegar (tempoInimigo), nós olhamos 9 batidas para o futuro.
        int batidaParaSpawnar = batidaAtual + level.tempoInimigo;

        // 2. O Dicionário tem acesso instantâneo! É só perguntar se existe um inimigo mapeado nessa batida alvo.
        if (mapaInimigos.ContainsKey(batidaParaSpawnar))
        {
            SpawnarInimigoVisual(batidaParaSpawnar);
            
            // Note que NÃO removemos o inimigo do mapaInimigos.
            // Os dados originais ficam salvos para podermos voltar no tempo depois!
        }
        
        if (InimigosAtivos.Count > 0)
        {
            // O OnBeat dos inimigos ativos (os visuais) continua funcionando
            InimigosAtivos.Peek().OnBeat(batidaAtual);
        }
    }
    
    public void AtacarInimigo(float tempoDoAtk)
    {
        if (InimigosAtivos.Count > 0)
        {
            InimigosAtivos.Peek().tomarDano(tempoDoAtk);
            InimigosAtivos.Dequeue();
        }
    }

    public void teste(TipoDeAcerto acerto)
    {
        Debug.Log("O acerto foi: " + acerto);
    }

    public void AddInimigo(InimigoPosicao inimigo)
    {
        // Verifica se já não existe um inimigo cadastrado nessa mesma batida (evita erros)
        if (!mapaInimigos.ContainsKey(inimigo.BatidaPosicionar))
        {
            mapaInimigos.Add(inimigo.BatidaPosicionar, inimigo);
            Debug.Log($"Inimigo adicionado na lane para a batida: {inimigo.BatidaPosicionar}");
        }
        else
        {
            Debug.LogWarning($"Aviso: Já existe um inimigo na batida {inimigo.BatidaPosicionar} nesta lane!");
        }
    }
}