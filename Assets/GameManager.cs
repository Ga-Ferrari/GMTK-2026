using UnityEngine;

public class GameManager : MonoBehaviour
{

    // O Padrão Singleton permite que outros scripts achem o GameManager facilmente
    public static GameManager instance;

    public float hitTimeBuffer;
    public float hitTimePerfect;

    void Awake()
    {
        // Configurando o Singleton
        if (instance == null) 
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }
}
