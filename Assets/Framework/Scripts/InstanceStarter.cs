using System.Diagnostics;
using System;

/// <summary>
/// Instance starter class, Starts The Game for all players in a party.
/// The start command is sent from the server when the Host click start game Button and all players are ready.
/// The game Builds are in the InstanceBuilds Folder.
/// Every Game must have a Build added to the Folder and the switch case line for the game also be must added.
/// </summary>
public static class InstanceStarter
{


    public static void RunFile(string filename)
    {
        string path = null;
        switch (filename)
        {
            case "Dixit":
                path = Environment.CurrentDirectory + "/Assets/InstanceBuildsBatFiles/Dixit.bat";
                break;

            default:
                break;
        }
        if(path != null)
        Process.Start(path);
    }
}

