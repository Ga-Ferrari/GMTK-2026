using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("Sprites do Player")]
    public Sprite spriteCima;
    public Sprite spriteBaixo;
    public Sprite spriteEsquerda;
    public Sprite spriteDireita;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void MudarDirecao(PosicaoLane direcaoAtaque)
    {
        switch (direcaoAtaque)
        {
            case PosicaoLane.cima:
                spriteRenderer.sprite = spriteCima; // Atirando para cima
                break;
            case PosicaoLane.baixo:
                spriteRenderer.sprite = spriteBaixo; // Atirando para baixo
                break;
            case PosicaoLane.esquerda:
                spriteRenderer.sprite = spriteEsquerda; // Atirando para a esquerda
                break;
            case PosicaoLane.direita:
                spriteRenderer.sprite = spriteDireita; // Atirando para a direita
                break;
        }
    }
}