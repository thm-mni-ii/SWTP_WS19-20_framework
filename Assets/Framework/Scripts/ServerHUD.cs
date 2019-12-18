
using System.ComponentModel;
using UnityEngine;
using Mirror;
    
/**
 * ServerHUD is a GUI of the main server in the game
 * the main server token from mirror
 */
public class ServerHUD : MonoBehaviour
{
   /// <summary>
   /// NetworkManager is responsible for the network connection.
   /// He has connection settings (network address, maxConnections, etc)
   /// </summary>
   NetworkManager manager;
   
   /// <summary>
   /// ChatServer configuration. Token form Telepathy.Server
   /// He has Information about Chat settings (clienList) and server port
   /// </summary>
   Server cServer;
   
   /// <summary>
   /// Whether to show the default control HUD at runtime.
   /// </summary>
   public bool showGUI = true;

   /// <summary>
   /// The horizontal offset in pixels to draw the HUD runtime GUI at.
   /// </summary>
   public int offsetX;

   /// <summary>
   /// The vertical offset in pixels to draw the HUD runtime GUI at.
   /// </summary>
   public int offsetY;
   
   void Awake()    /*hier kommt noch was*/
   {
      manager = GetComponent<NetworkManager>();
      cServer = GetComponent<Server>();
   }

   /**
    * handle events in NetworkManager    /*hier kommt noch was*
    * events are:
    * LAN Server Only to make and start a new server
    * other events are deleted by us
    */
   void OnGUI()
   {
      if (!showGUI)
         return;

        GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (!NetworkClient.active)
            {
                /*
                // LAN Host
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    if (GUILayout.Button("LAN Host"))
                    {
                        manager.StartHost();
                    }
                }

                // LAN Client + IP
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("LAN Client"))
                {
                    manager.StartClient();
                }
                manager.networkAddress = GUILayout.TextField(manager.networkAddress);
                GUILayout.EndHorizontal();
                */
                // LAN Server Only
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    // cant be a server in webgl build
                    GUILayout.Box("(  WebGL cannot be server  )");
                }
                else
                {
                    if (GUILayout.Button("LAN Server Only"))
                    {
                        manager.StartServer();
                        if (!cServer.server.Active)
                        {
                            cServer.server.Start(cServer.port);
                        }
                    }
                }
            }
            else
            {
                // Connecting
                GUILayout.Label("Connecting to " + manager.networkAddress + "..");
                if (GUILayout.Button("Cancel Connection Attempt"))
                {
                    manager.StopClient();
                }
            }
        }
        else
        {
            // server / client status message
            if (NetworkServer.active)
            {
                GUILayout.Label("Server: active. Transport: " + Transport.activeTransport);
            }
            if (NetworkClient.isConnected)
            {
                GUILayout.Label("Client: address=" + manager.networkAddress);
            }
        }

        // client ready
        if (NetworkClient.isConnected && !ClientScene.ready)
        {
            if (GUILayout.Button("Client Ready"))
            {
                ClientScene.Ready(NetworkClient.connection);

                if (ClientScene.localPlayer == null)
                {
                    ClientScene.AddPlayer();
                }
            }
        }

        // stop
        if (NetworkServer.active || NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop"))
            {
                if (cServer.server.Active)
                {
                    cServer.server.Stop();
                }
                manager.StopHost();
            }
        }

        GUILayout.EndArea();
    }
}
