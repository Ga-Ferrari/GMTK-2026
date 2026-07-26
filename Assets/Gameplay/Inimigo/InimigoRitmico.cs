using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

public enum TipoDeAcerto { MuitoAdiantado, Adiantado, Perfeito, Atrasado, MuitoAtrasado }

public class InimigoRitmico : MonoBehaviour
{
    [Header("TempoDeAtk")]
    [NonSerialized] public int batidaAtk;
    
    [Header("Eventos")]
    public UnityEvent<TipoDeAcerto> tomouDano;
    public float SpB;
    [SerializeField] private AudioSource AD;

    [Header("Visual e Movimento")]
    public TMP_Text textoContagem;
    private Vector3 posInicio;
    private Vector3 posFim;
    private int batidasAvisoPrevio; 

    private int estadoAtual = 5;

    private bool vivo = true;
    private SpriteRenderer inimigoRenderer;

    private float progressoAtual = 0f;
    [SerializeField] private AudioClip[] Clips;

    [Header("Tipos Especiais")]
    public TipoInimigo tipoInimigo;
    [NonSerialized] public bool sendoSegurado = false;
    [NonSerialized] public int batidaParaSoltar;

    private Animator animatorInimigo;

    [SerializeField] private SpriteRenderer textoContagemSprite;
    
    private bool ninjaRevelado = false;

    private float timerTrocaEstado=0f;

    [SerializeField] private List<Sprite> spritesNumeros;

    void Start()
    {
        inimigoRenderer = GetComponent<SpriteRenderer>();
        animatorInimigo = GetComponent<Animator>(); 
        estadoAtual = 4;
    }

    void Update()
    {
        if (!vivo) return;
        //float tempoTrocaEstado = batidasAvisoPrevio/5f*SpB;
        float tempoTotalDeMovimento = batidasAvisoPrevio * SpB;

        timerTrocaEstado +=Time.deltaTime;

        //if (timerTrocaEstado > tempoTrocaEstado)
        /*{
            timerTrocaEstado=0f;
            estadoAtual--;
            Debug.Log("Trocou");
        }*/

        if (tempoTotalDeMovimento > 0 && !sendoSegurado)
        {
            progressoAtual += Time.deltaTime / tempoTotalDeMovimento;
            progressoAtual = Mathf.Clamp01(progressoAtual); 
            
            transform.position = Vector3.Lerp(posInicio, posFim, progressoAtual);

            // LOGICA DO NINJA: Fica invisível após 50%, MAS APENAS se ainda não foi revelado
            if (tipoInimigo == TipoInimigo.Ninja && progressoAtual > 0.5f && !ninjaRevelado)
            {
                ficarInvisivel();
                textoContagemSprite.enabled = false;
                if (textoContagem != null) textoContagem.enabled = false;
            }
        }
    }

    private void TrocouEstado()
    {
        if(estadoAtual>=0&&estadoAtual<spritesNumeros.Count)textoContagemSprite.sprite = spritesNumeros[estadoAtual];
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
            //GameManager.instance.TocarAudio(Clips[1],1);
        }
        else 
        {
            //GameManager.instance.TocarAudio(Clips[1], 1);
            tomouDano?.Invoke(TipoDeAcerto.Perfeito);
        }
        animatorInimigo.SetBool("Morrendo",true);
        textoContagemSprite.enabled = false;

        animatorInimigo.SetTrigger("Morrer");
        
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
        //animatorInimigo.SetTrigger("Atirar");
        tomouDano?.Invoke(TipoDeAcerto.MuitoAtrasado);
        if (textoContagem != null) textoContagem.enabled = false;
    }

    private void ficarInvisivel() { if (inimigoRenderer != null) inimigoRenderer.enabled = false; }
    private void ficarVisivel() { if (inimigoRenderer != null) inimigoRenderer.enabled = true; }

public void OnBeat(int batidaAtual)
    {
        estadoAtual--;
        TrocouEstado();
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
            textoContagemSprite.enabled = true;
            if (textoContagem != null) 
            {
                textoContagem.enabled = true; // Religa o canvas do texto
            }
        }

        if (!vivo) return;

        int batidasRestantes = batidaAtk - batidaAtual;
        
        GameManager.instance.TocarAudio(Clips[0],SpB);  
        
        
        

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
