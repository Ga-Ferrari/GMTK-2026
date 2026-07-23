using System;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{

    private Queue<InimigoPosicao> filaInimigos = new Queue<InimigoPosicao>();

    private Queue<InimigoRitmico> InimigosAtivos = new Queue<InimigoRitmico>();
    [SerializeField] private LevelLogic level;
    [SerializeField] private GameObject inimigoPrefab;

    [SerializeField] private Transform inicioLane;
    [SerializeField] private Transform fimLane;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnarInimigoVisual()
    {
        // 1. Instancia o prefab na posição da Lane, sem rotação especial (Quaternion.identity)
        GameObject novoInimigoObj = Instantiate(inimigoPrefab, inicioLane.position, Quaternion.identity);

        // 2. Pega o script InimigoRitmico que está dentro desse prefab que acabou de nascer
        InimigoRitmico scriptInimigo = novoInimigoObj.GetComponent<InimigoRitmico>();

        InimigosAtivos.Enqueue(scriptInimigo);
        Debug.Log("Inimigo spawnado na Lane: " + gameObject.name);
        
    }

    public void OnBeat(int batidaAtual)
    {

        if (filaInimigos.Count > 0)
        {
            if (filaInimigos.Peek().BatidaPosicionar - batidaAtual< level.tempoInimigo)
            {
                SpawnarInimigoVisual();
                filaInimigos.Dequeue();
            }
        }
        
    }
    
    public void AtacarInimigo()
    {
        InimigosAtivos.Peek().tomarDano();
        InimigosAtivos.Dequeue();
    }

    public void teste(TipoDeAcerto acerto)
    {
        Debug.Log("O acerto foi: " + acerto);
    }

    public void AddInimigo(InimigoPosicao inimigo)
    {
        filaInimigos.Clear();
        filaInimigos.Enqueue(inimigo);
    }

}
