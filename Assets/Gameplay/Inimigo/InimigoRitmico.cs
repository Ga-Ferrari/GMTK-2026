using System;
using Unity.Mathematics;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;


public enum TipoDeAcerto{
    MuitoAdiantado,
    Adiantado,
    Perfeito,
    Atrasado,
    MuitoAtrasado
}

public class InimigoRitmico : MonoBehaviour
{
    [Header("Quantos estágios de ataque irão ter, ex:3 = (3,2,1)")]
    [SerializeField] private int maxStages = 3; //Apenas Visual

    [Header("TempoDeAtk, irá virar configuração do nível")]
    [NonSerialized]public int batidaAtk;

    [Header("Que eventos chamar no hit")]
    public UnityEvent<TipoDeAcerto> tomouDano;

    public float SpB;

    private int currentStage; //Apenas Visual
    private bool vivo = true;

    private int batidasPraMudar =3;
    private int contadorBatidas=0;

    private Renderer inimigoRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inimigoRenderer = GetComponent<Renderer>();
        currentStage = maxStages;
    }

    // Update is called once per frame
    void Update()
    {   
        
    }

    public void tomarDano(float tempoDoAtk)
    {
        vivo = false;
        float diferencaTempo = batidaAtk*SpB - tempoDoAtk; 
        ficarInvisivel();
        if ( math.abs(diferencaTempo)>GameManager.instance.hitTimeBuffer)
        {
            if(diferencaTempo>0)tomouDano?.Invoke(TipoDeAcerto.MuitoAdiantado);
            else tomouDano?.Invoke(TipoDeAcerto.MuitoAtrasado);
            return;
        }
        else if (math.abs(diferencaTempo)>GameManager.instance.hitTimePerfect)
        {
            if(diferencaTempo>0)tomouDano?.Invoke(TipoDeAcerto.Adiantado);
            else tomouDano?.Invoke(TipoDeAcerto.Atrasado);
            return;
        }
        tomouDano?.Invoke(TipoDeAcerto.Perfeito);

    }

    private void ficarInvisivel()
    {
        if (inimigoRenderer != null)
        {
            inimigoRenderer.enabled = false; // Desliga o desenho na tela
        }
    }

    private void ficarVisivel()
    {
        if (inimigoRenderer != null)
        {
            inimigoRenderer.enabled = true; // Liga o desenho na tela novamente
        }
    }

    public void OnBeat(int batidaAtual)
    {
        if (batidaAtual < batidaAtk)
        {
            ficarVisivel();
            vivo = true;
        }

        if(!vivo) return;

        currentStage = (batidaAtk - batidaAtual) / batidasPraMudar;
        
        // Atualiza o tamanho apenas uma vez por batida!
        transform.localScale = new Vector3(currentStage, currentStage, currentStage);
    }


}
