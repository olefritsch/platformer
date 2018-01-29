using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerDetector : MonoBehaviour {

    public int maxPlayers = 4;
    private int rewiredPlayerIdCounter = 0;

    private List<int> assignedJoysticks;
    private Player systemPlayer;

    void Awake()
    {
        assignedJoysticks = new List<int>();

        ReInput.ControllerConnectedEvent += OnControllerConnected;
    }

    void Start()
    {
        // Assign all Joysticks to the System Player initially removing assignment from other Players.
        systemPlayer = ReInput.players.GetSystemPlayer();
        AssignAllJoysticksToSystemPlayer();
    }

    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        if (args.controllerType != ControllerType.Joystick)
            return;

        // Check if this Joystick has already been assigned. If so, just let Auto-Assign do its job.
        if (assignedJoysticks.Contains(args.controllerId))
            return;

        // Assign Joystick to the System Player until it's been explicitly assigned
        ReInput.players.GetSystemPlayer().controllers.AddController<Joystick>(args.controllerId, true);
    }

    void AssignAllJoysticksToSystemPlayer()
    {
        foreach (Joystick j in ReInput.controllers.Joysticks)
        {
            ReInput.players.GetSystemPlayer().controllers.AddController(j, true);
        }
    }

    void Update()
    {
        // Watch for JoinGame action in System Player
        if (systemPlayer.GetButtonDown("Join Game"))
        {
            Debug.Log("Join Game Pressed");
            AssignNextPlayer();
        }
    }

    void AssignNextPlayer()
    {
        Player player = ReInput.players.GetPlayer(GetNextGamePlayerId());

        // Determine which Controller was used to generate the JoinGame Action
        var inputSources = systemPlayer.GetCurrentInputSources("Join Game");

        foreach (InputActionSourceData source in inputSources)
        {
            if (source.controllerType == ControllerType.Keyboard || source.controllerType == ControllerType.Mouse)
            {
                // Assign KB/Mouse to the Player
                AssignKeyboardAndMouseToPlayer(player);

                // Disable KB/Mouse Assignment category in System Player so it doesn't assign through the keyboard/mouse anymore
                systemPlayer.controllers.maps.SetMapsEnabled(false, ControllerType.Keyboard, "Assignment");
                systemPlayer.controllers.maps.SetMapsEnabled(false, ControllerType.Mouse, "Assignment");
                break;
            }
            else if (source.controllerType == ControllerType.Joystick)
            {
                // Assign the joystick to the Player. This will also un-assign it from System Player
                AssignJoystickToPlayer(player, source.controller as Joystick);
                break;
            }
            else
            {
                throw new System.NotImplementedException();
            }
        }

        // Enable Setup map so Player can start customizing self
        player.controllers.maps.SetMapsEnabled(true, "Setup");
    }

    private void AssignKeyboardAndMouseToPlayer(Player player)
    {
        // Assign mouse to Player
        player.controllers.hasMouse = true;

        // Load the keyboard and mouse maps into the Player
        player.controllers.maps.LoadMap(ControllerType.Keyboard, 0, "Setup", "Default", true);
        player.controllers.maps.LoadMap(ControllerType.Mouse, 0, "Default", "Default", true);

        // Exclude this Player from Joystick auto-assignment because it is the KB/Mouse Player now
        player.controllers.excludeFromControllerAutoAssignment = true;

        Debug.Log("Assigned Keyboard/Mouse to Player " + player.name);
    }

    private void AssignJoystickToPlayer(Player player, Joystick joystick)
    {
        // Assign the joystick to the Player, removing it from System Player
        player.controllers.AddController(joystick, true);

        // Mark this joystick as assigned so we don't give it to the System Player again
        assignedJoysticks.Add(joystick.id);

        Debug.Log("Assigned " + joystick.name + " to Player " + player.name);
    }

    private int GetNextGamePlayerId()
    {
        return rewiredPlayerIdCounter++;
    }

}
