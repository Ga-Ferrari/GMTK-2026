using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animatorPlayer;
    [SerializeField] AudioClip[] Clips;
    
    void Awake()
    {
        animatorPlayer = GetComponent<Animator>();
    }

    

    public void MudarDirecao(PosicaoLane direcaoAtaque)
    {
        GameManager.instance.TocarAudio(Clips[UnityEngine.Random.Range(0,2)], UnityEngine.Random.Range(0.85f, 1.15f));
        switch (direcaoAtaque)
        {
            case PosicaoLane.cima:
                animatorPlayer.SetInteger("direcao",1);; // Atirando para cima
                break;
            case PosicaoLane.baixo:
                animatorPlayer.SetInteger("direcao",2);; // Atirando para cima

                break;
            case PosicaoLane.esquerda:
                animatorPlayer.SetInteger("direcao",3);; // Atirando para cima

                break;
            case PosicaoLane.direita:
                animatorPlayer.SetInteger("direcao",4);; // Atirando para cima

                break;
        }
        animatorPlayer.SetTrigger("Atacar");
    }
}