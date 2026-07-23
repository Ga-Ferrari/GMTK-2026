using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelInputManager : MonoBehaviour
{

    [SerializeField] private LevelLogic level;
    void Start()
    {
        
    }

    public void OnPress(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            string actionName = context.action.name;
            Lane lane = null;;
            switch (actionName)
            {
                case "UpLaneHit":
                    lane = level.PegarLanePorPosicao(PosicaoLane.cima);
                    break;
                case "DownLaneHit":
                    lane =level.PegarLanePorPosicao(PosicaoLane.baixo);
                    break;
                case "LeftLaneHit":
                    lane = level.PegarLanePorPosicao(PosicaoLane.esquerda);
                    break;
                case "RightLaneHit":
                    lane = level.PegarLanePorPosicao(PosicaoLane.direita);
                    break;
            }
            lane.AtacarInimigo(level.timerLevel);
        }
    }

}
