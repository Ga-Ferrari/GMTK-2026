using System;
using Unity.Mathematics;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;


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
    private int currentStage; //Apenas Visual

    [Header("TempoDeAtk, irá virar configuração do nível")]
    [NonSerialized]public float timeToAtk = 3;
    private float atkTimer;

    [Header("Que eventos chamar no hit")]
    public UnityEvent<TipoDeAcerto> tomouDano;

    private bool vivo = true;

    private int batidasPraMudar =3;
    private int contadorBatidas=0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        atkTimer = timeToAtk;
        currentStage = maxStages;
    }

    // Update is called once per frame
    void Update()
    {   if(!vivo)return;

        transform.localScale = new Vector3(currentStage,currentStage,currentStage);
        if (atkTimer < -GameManager.instance.hitTimeBuffer)
        {
            tomarDano();
        }
        else
        {
            atkTimer-=Time.deltaTime;
        }
    }

    public void tomarDano()
    {
        Debug.Log("Fui atacado em :" + atkTimer);
        vivo = false;
        if (math.abs(atkTimer)>GameManager.instance.hitTimeBuffer)
        {
            if(atkTimer>0)tomouDano?.Invoke(TipoDeAcerto.MuitoAdiantado);
            else tomouDano?.Invoke(TipoDeAcerto.MuitoAtrasado);
            return;
        }
        else if (math.abs(atkTimer)>GameManager.instance.hitTimePerfect)
        {
            if(atkTimer>0)tomouDano?.Invoke(TipoDeAcerto.Adiantado);
            else tomouDano?.Invoke(TipoDeAcerto.Atrasado);
            return;
        }
        tomouDano?.Invoke(TipoDeAcerto.Perfeito);
        Destroy(this);
        enabled = false;

    }

    public void OnBeat(int batidaAtual)
    {
        if(!vivo)return;
        contadorBatidas++;
        Debug.Log("Batidas: " + contadorBatidas);
        Debug.Log("Batidas mudar: " + batidasPraMudar);
        Debug.Log("Current stage: " + currentStage);
        if (contadorBatidas >= batidasPraMudar)
        {
            currentStage--;
            contadorBatidas=0;
        }
    }


}
