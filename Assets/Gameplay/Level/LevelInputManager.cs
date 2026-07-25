using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelInputManager : MonoBehaviour
{
    [SerializeField] private LevelLogic level;
    
    [SerializeField] private PlayerVisual playerVisual; 

public void OnPress(InputAction.CallbackContext context)
    {
        if (context.started || context.canceled) 
        {
            string actionName = context.action.name;
            Lane lane = null;
            PosicaoLane direcaoAtacada = PosicaoLane.baixo; 

            switch (actionName)
            {
                case "UpLaneHit":
                    direcaoAtacada = PosicaoLane.cima;
                    lane = level.PegarLanePorPosicao(direcaoAtacada);
                    break;
                case "DownLaneHit":
                    direcaoAtacada = PosicaoLane.baixo;
                    lane = level.PegarLanePorPosicao(direcaoAtacada);
                    break;
                case "LeftLaneHit":
                    direcaoAtacada = PosicaoLane.esquerda;
                    lane = level.PegarLanePorPosicao(direcaoAtacada);
                    break;
                case "RightLaneHit":
                    direcaoAtacada = PosicaoLane.direita;
                    lane = level.PegarLanePorPosicao(direcaoAtacada);
                    break;
            }
            
            if (lane != null)
            {
                Debug.Log("Entrou if lane");
                if (context.started)
                {
                    Debug.Log("Entrou if");
                    lane.AtacarInimigo(level.timerLevel); // APERTOU
                    if (playerVisual != null) playerVisual.MudarDirecao(direcaoAtacada);
                }
                else if (context.canceled)
                {
                    lane.SoltarAtaque(level.timerLevel); // SOLTOU
                }
            }
        }
    }
}
