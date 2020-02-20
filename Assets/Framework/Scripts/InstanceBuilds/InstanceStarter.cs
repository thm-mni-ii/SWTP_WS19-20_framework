using System.Diagnostics;
using System;


/// <summary>
/// Instance starter class - not finished yet
/// </summary>
public static class InstanceStarter
{


    public static void RunFile(string filename)
    {
        string path = null;
        switch (filename)
        {
            case "Dexit":
                path = Environment.CurrentDirectory + "/Assets/Framework/Scripts/InstanceBuilds/Dexit.txt";
                break;

            default:
                break;
        }
        if(path != null)
        Process.Start(path);
    }
}

