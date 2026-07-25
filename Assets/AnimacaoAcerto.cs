using TMPro;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class CoresAcerto
{
    // Adicionado "public" para que você consiga editar no Inspector da Unity
    public TipoDeAcerto tipoDeAcerto;
    public Color cor;
}

public class AnimacaoAcerto : MonoBehaviour
{
    [SerializeField] private TextMeshPro textoAcuracia;
    [SerializeField] private List<CoresAcerto> cores;

    [Header("Configurações de Animação")]
    [SerializeField] private float tempoSalto = 0.3f;
    [SerializeField] private float tempoSumir = 0.6f;
    
    // Nova variável para você controlar o quanto o texto pode deitar para os lados
    [SerializeField] private float rotacaoMaxima = 15f; 

    // NOVA VARIÁVEL: Define o quão grande o texto vai ficar no pico do pulo
    [SerializeField] private float tamanhoMaximo = 1.5f;

    public void animarAcerto(TipoDeAcerto acerto)
    {
        // Interrompe animações anteriores para que não buguem se o jogador acertar duas notas muito rápido
        StopAllCoroutines(); 
        StartCoroutine(RotinaAnimacaoTexto(acerto));
    }

    private IEnumerator RotinaAnimacaoTexto(TipoDeAcerto tipo)
    {
        // 1. Atualiza o que o texto diz dependendo do acerto
        switch (tipo)
        {
            case TipoDeAcerto.MuitoAdiantado: textoAcuracia.text = "TOO\nEARLY!"; break;
            case TipoDeAcerto.Adiantado: textoAcuracia.text = "EARLY!"; break;
            case TipoDeAcerto.Perfeito: textoAcuracia.text = "PERFECT!"; break;
            case TipoDeAcerto.Atrasado: textoAcuracia.text = "LATE!"; break;
            case TipoDeAcerto.MuitoAtrasado: textoAcuracia.text = "TOO\nLATE!"; break;
        }

        // 2. Busca a cor correspondente na sua lista do Inspector
        Color corEscolhida = Color.white; // Cor padrão caso dê erro ou não ache
        foreach (CoresAcerto item in cores)
        {
            if (item.tipoDeAcerto == tipo)
            {
                corEscolhida = item.cor;
                break; // Achou a cor, pode parar de procurar
            }
        }

        textoAcuracia.enabled = true; 

        // Aplica a cor escolhida e garante o Alpha em 1
        textoAcuracia.color = new Color(corEscolhida.r, corEscolhida.g, corEscolhida.b, 1f);
        textoAcuracia.transform.localScale = Vector3.one; 

        // 3. Prepara a Rotação Suave
        float inclinacaoAleatoria = UnityEngine.Random.Range(-rotacaoMaxima, rotacaoMaxima);
        Quaternion rotacaoInicial = Quaternion.identity; // Começa totalmente reto (0,0,0)
        Quaternion rotacaoDestino = Quaternion.Euler(0f, 0f, inclinacaoAleatoria); 
        
        // Garante que o texto inicie reto na tela
        textoAcuracia.transform.rotation = rotacaoInicial;

        Vector3 tamanhoMaior = Vector3.one * tamanhoMaximo;
        float tempoDecorrido = 0f;

        // PARTE 1: O Salto
        while (tempoDecorrido < tempoSalto)
        {
            // Calcula uma curva mais suave (acelera no começo, freia no final)
            float progressoSuave = Mathf.SmoothStep(0f, 1f, tempoDecorrido / tempoSalto);
            
            textoAcuracia.transform.localScale = Vector3.Lerp(Vector3.one, tamanhoMaior, progressoSuave);
            textoAcuracia.transform.rotation = Quaternion.Lerp(rotacaoInicial, rotacaoDestino, progressoSuave);
            tempoDecorrido += Time.deltaTime;
            yield return null; 
        }

        tempoDecorrido = 0f; 

        // PARTE 2: Sumir 
        while (tempoDecorrido < tempoSumir)
        {
            float alpha = Mathf.Lerp(1f, 0f, tempoDecorrido / tempoSumir);
            
            // Usa a 'corEscolhida' para não perder a cor durante o fade
            textoAcuracia.color = new Color(corEscolhida.r, corEscolhida.g, corEscolhida.b, alpha);
            
            textoAcuracia.transform.localScale = Vector3.Lerp(tamanhoMaior, Vector3.one, tempoDecorrido / tempoSumir);

            tempoDecorrido += Time.deltaTime;
            yield return null;
        }

        textoAcuracia.enabled = false;
    }
}