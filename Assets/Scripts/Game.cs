using System;
using System.Collections.Generic;

[Serializable]
public class Game
{
    private string GameName;
    private List<UserInfo> PlayerList = new List<UserInfo>();
    private int GameID;
    private string ServerIP;
    private UserInfo GameHost;


	public Game()
	{

	}
}
