using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public enum TipoDeAcerto { MuitoAdiantado, Adiantado, Perfeito, Atrasado, MuitoAtrasado }

public class InimigoRitmico : MonoBehaviour
{
    [Header("TempoDeAtk")]
    [NonSerialized] public int batidaAtk;

    [Header("Eventos")]
    public UnityEvent<TipoDeAcerto> tomouDano;
    public float SpB;

    [Header("Visual e Movimento")]
    public TMP_Text textoContagem;
    private Vector3 posInicio;
    private Vector3 posFim;
    private int batidasAvisoPrevio; 

    private bool vivo = true;
    private SpriteRenderer inimigoRenderer;

    private float progressoAtual = 0f; 

    [Header("Tipos Especiais")]
    public TipoInimigo tipoInimigo;
    [NonSerialized] public bool sendoSegurado = false;
    [NonSerialized] public int batidaParaSoltar;

    private Animator animatorInimigo;
    
    private bool ninjaRevelado = false;

    void Start()
    {
        inimigoRenderer = GetComponent<SpriteRenderer>();
        animatorInimigo = GetComponent<Animator>();
    }

    void Update()
    {
        if (!vivo) return;

        float tempoTotalDeMovimento = batidasAvisoPrevio * SpB;

        if (tempoTotalDeMovimento > 0 && !sendoSegurado)
        {
            progressoAtual += Time.deltaTime / tempoTotalDeMovimento;
            progressoAtual = Mathf.Clamp01(progressoAtual); 
            
            transform.position = Vector3.Lerp(posInicio, posFim, progressoAtual);

            // LOGICA DO NINJA: Fica invisível após 50%, MAS APENAS se ainda não foi revelado
            if (tipoInimigo == TipoInimigo.Ninja && progressoAtual > 0.5f && !ninjaRevelado)
            {
                ficarInvisivel();
                if (textoContagem != null) textoContagem.enabled = false;
            }
        }
    }

    public void ConfigurarMovimento(Vector3 inicio, Vector3 fim, int tempoInimigo)
    {
        posInicio = inicio;
        posFim = fim;
        batidasAvisoPrevio = tempoInimigo;
        transform.position = inicio;
        
        if (textoContagem != null)
        {
            textoContagem.text = tempoInimigo.ToString();
        }
    }

    public void DefinirSpritePorLane(PosicaoLane laneDeOrigem)
    {
        if (inimigoRenderer == null) inimigoRenderer = GetComponent<SpriteRenderer>();
        if(animatorInimigo == null) 
        {
            animatorInimigo = GetComponent<Animator>();
            Debug.Log("Entrou if null");
            if(animatorInimigo==null)Debug.Log("Null ainda");
        }
        
        switch (laneDeOrigem)
        {
            case PosicaoLane.cima:
                animatorInimigo.SetInteger("Direcao",1);
                break;
            case PosicaoLane.baixo:
                animatorInimigo.SetInteger("Direcao",2);

                break;
            case PosicaoLane.esquerda:
                animatorInimigo.SetInteger("Direcao",3);

                break;
            case PosicaoLane.direita:
                animatorInimigo.SetInteger("Direcao",4);
                break;
        }
    }

    public void tomarDano(float tempoDoAtk)
    {
        vivo = false;
        float diferencaTempo = batidaAtk * SpB - tempoDoAtk; 
        

        
        if (textoContagem != null) textoContagem.enabled = false; 

        if (math.abs(diferencaTempo) > GameManager.instance.hitTimeBuffer)
        {
            if(diferencaTempo > 0)
            {
                tomouDano?.Invoke(TipoDeAcerto.MuitoAdiantado);
            }
            else {
                tomouDano?.Invoke(TipoDeAcerto.MuitoAtrasado);
                
            }
        }
        else if (math.abs(diferencaTempo) > GameManager.instance.hitTimePerfect)
        {
            if(diferencaTempo > 0) tomouDano?.Invoke(TipoDeAcerto.Adiantado);
            else tomouDano?.Invoke(TipoDeAcerto.Atrasado);
        }
        else 
        {
            tomouDano?.Invoke(TipoDeAcerto.Perfeito);
        }

        Destroy(gameObject,0.2f);
        
    }

    public void DestruirAposAnimacao()
    {
        ficarInvisivel(); // Caso queira que ele suma um frame antes de destruir
        Destroy(gameObject);
    }

    // Funções exclusivas do Beefy-Boy (Segurar o botão)
    public void IniciarHold(float tempoDoAtk)
    {
        float diferencaTempo = batidaAtk * SpB - tempoDoAtk; 
        if (math.abs(diferencaTempo) > GameManager.instance.hitTimePerfect)
        {
            tomouDano?.Invoke(TipoDeAcerto.MuitoAdiantado);
            ForcarMortePorPassarDoTempo();
        }
        else
        {
            sendoSegurado = true;
            batidaParaSoltar = batidaAtk + 1; 
            if (textoContagem != null) textoContagem.text = "HOLD!";
        }
    }

    public void FinalizarHold(float tempoDoAtk)
    {
        vivo = false;
        float diferencaTempo = batidaParaSoltar * SpB - tempoDoAtk; 
        
        if (math.abs(diferencaTempo) > GameManager.instance.hitTimePerfect)
            tomouDano?.Invoke(TipoDeAcerto.MuitoAtrasado); 
        else 
            tomouDano?.Invoke(TipoDeAcerto.Perfeito); 

        ficarInvisivel();
        if (textoContagem != null) textoContagem.enabled = false;
        Destroy(gameObject, 0.2f);
    }

    public void ForcarMortePorPassarDoTempo()
    {
        vivo = false;
        animatorInimigo.SetTrigger("Atirar");
        if (textoContagem != null) textoContagem.enabled = false;
    }

    private void ficarInvisivel() { if (inimigoRenderer != null) inimigoRenderer.enabled = false; }
    private void ficarVisivel() { if (inimigoRenderer != null) inimigoRenderer.enabled = true; }

public void OnBeat(int batidaAtual)
    {
        if (batidaAtual < batidaAtk - batidasAvisoPrevio) return;
        
        if (batidaAtual < batidaAtk)
        {
            // Mantém visível apenas se não for a fase escondida do Ninja
            if (!(tipoInimigo == TipoInimigo.Ninja && progressoAtual > 0.5f && !ninjaRevelado))
            {
                ficarVisivel();
            }
            vivo = true;
        }

        // Se for o Ninja e o contador chegou a exatos 0
        if (batidaAtual == batidaAtk && tipoInimigo == TipoInimigo.Ninja)
        {
            ninjaRevelado = true;
            ficarVisivel(); 
            if (textoContagem != null) 
            {
                textoContagem.enabled = true; // Religa o canvas do texto
            }
        }

        if (!vivo) return;

        int batidasRestantes = batidaAtk - batidaAtual;

        // Atualiza o texto (ignorando se estiver na fase invisível do Ninja)
        if (textoContagem != null && !(tipoInimigo == TipoInimigo.Ninja && progressoAtual > 0.5f && !ninjaRevelado))
        {
            if (batidasRestantes == 0 && tipoInimigo == TipoInimigo.Ninja)
            {
                textoContagem.text = "!";
            }
            else
            {
                textoContagem.text = batidasRestantes.ToString();
            }
        }
    }
}
