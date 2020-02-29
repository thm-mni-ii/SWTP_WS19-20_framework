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
   
   /// <summary>
   /// Get NetworkManager and Server from Mirror
   /// </summary>
   void Awake()
   {
      manager = GetComponent<NetworkManager>();
      cServer = GetComponent<Server>();
   }
   
   /// <summary>
   /// handle events in NetworkManager
   /// events are:
   /// LAN Server Only to make and start a new server
   /// other events are deleted by us
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
          }
      }


      // stop
      if (NetworkServer.active)
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
