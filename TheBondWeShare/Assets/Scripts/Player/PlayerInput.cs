using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using Unity.VisualScripting;

public class PlayerInput : MonoBehaviour
{
    public int playerID;
    PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        switch (playerID)
        {
            case 0:

                if (Input.GetKey("a"))
                {
                    _playerMovement.Move(-1);
                }
                else if (Input.GetKey("d"))
                {
                    _playerMovement.Move(1);
                }
                else 
                    _playerMovement.Move(0);

                if (Input.GetKeyDown("w"))
                {
                    _playerMovement.Jump();
                }
                else if (Input.GetKeyDown("s"))
                {
                    _playerMovement.Down(true, playerID);
                }
                else if (Input.GetKeyUp("s"))
                {
                    _playerMovement.Down(false, playerID);
                }
                if (Input.GetKey("q"))
                {
                    _playerMovement.Crane(true);
                }

                if (Input.GetKey("e"))
                {
                    _playerMovement.Crane(false);
                }

                if(Input.GetKeyDown(KeyCode.LeftControl))
                {
                    _playerMovement.BoundUnbound();
                }

                break;

            case 1:
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    _playerMovement.Move(-1);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    _playerMovement.Move(1);
                }
                else
                    _playerMovement.Move(0);

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    _playerMovement.Jump();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _playerMovement.Down(true, playerID);
                }
                else if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    _playerMovement.Down(false, playerID);
                }

                if (Input.GetKey("o"))
                {
                    _playerMovement.Crane(true);
                }

                if (Input.GetKey("p"))
                {
                    _playerMovement.Crane(false);
                }

                if (Input.GetKeyDown(KeyCode.RightControl))
                {
                    _playerMovement.BoundUnbound();
                }

                break;
            default: 
                break;
        }
    }

    
}
