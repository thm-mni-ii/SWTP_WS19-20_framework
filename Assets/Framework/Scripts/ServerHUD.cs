using UnityEngine;
using Mirror;

/// <summary>
/// ServerHUD shows the Start Server button
/// If the client is also the Server Host the Stop Button will be visible else it will disappear
/// </summary>
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
   
   /// <summary>
   /// Called once the Component is called for the first time,
   /// To get the reference of the networkManager and Server scripts
   /// </summary>
   void Awake()
   {
      manager = GetComponent<NetworkManager>();
      cServer = GetComponent<Server>();
   }
   
   /// <summary>
   /// Shows the Start server Button GUI for the Client if theres no Connection
   /// Once the connection is established the Stop button will show for the Host and disappear for the Clients
   /// </summary>
   void OnGUI()
   {
      if (!showGUI)
         return;

      GUILayout.BeginArea(new Rect(10 + offsetX, 140 + offsetY, 100, 9999));
      if (!NetworkClient.isConnected && !NetworkServer.active)
      {
        if (!NetworkClient.active)
        { 
            // LAN Server Only
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            { 
                // cant be a server in webgl build
                GUILayout.Box("(  WebGL cannot be server  )");
            }
            else
            { 
                if (GUILayout.Button("Start Server"))
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
              GUILayout.Label("Server: active.");
                //stop
                if (GUILayout.Button("Stop"))
                {
                    if (cServer.server.Active)
                    {
                        cServer.server.Stop();
                    }
                    manager.StopHost();
                }
            }
      }

      GUILayout.EndArea();
   }
}
