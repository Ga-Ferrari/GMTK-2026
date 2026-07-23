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
    [SerializeField]private float timeToAtk = 3;
    private float atkTimer;

    [Header("Que eventos chamar no hit")]
    public UnityEvent<TipoDeAcerto> tomouDano;

    private bool vivo = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        atkTimer = timeToAtk;
        currentStage = maxStages;
    }

    // Update is called once per frame
    void Update()
    {   if(!vivo)return;
        currentStage = (int)math.ceil(atkTimer);
        if(atkTimer>0)transform.localScale = new Vector3(atkTimer,atkTimer,atkTimer);
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
    }

}
