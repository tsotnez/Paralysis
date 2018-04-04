using System.Collections.Generic;

public class Team
{
    public int TeamNumber { get; private set; }
    public List<Player> TeamPlayers { get; private set; }

    public Team(int Number)
    {
        TeamNumber = Number;
        TeamPlayers = new List<Player>();
    }

    public Team(int Number, Player Player)
    {
        TeamNumber = Number;
        TeamPlayers = new List<Player>();
        AddPlayer(Player);
    }

    public Team(int Number, Player[] Players)
    {
        TeamNumber = Number;
        TeamPlayers = new List<Player>();
        foreach (Player newPlayer in Players)
        {
            AddPlayer(newPlayer);
        }
    }

    public Team(int Number, List<Player> Players)
    {
        TeamNumber = Number;
        TeamPlayers = Players;
    }

    public void AddPlayer(Player Player)
    {
        TeamPlayers.Add(Player);
    }
}

