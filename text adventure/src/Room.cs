using System.Collections.Generic;

class Room
{
	// Private fields
	private string description;
	private Inventory chest;
	private Dictionary<string, Room> exits; // store room exits


    public Inventory Chest
    {
        get { return chest; }
    }

    // Constructor
    public Room(string desc)
    {
        description = desc;
        chest = new Inventory(99999); 
        exits = new Dictionary<string, Room>();
    }
	
	public void AddExit(string direction, Room neighbor)
	{
		exits.Add(direction, neighbor);
	}

	// Return the description of the room.
	public string GetShortDescription()
	{
		return description;
	}

	// Return a long description of this room, in the form:
	//     You are in the kitchen.
	//     Exits: north, west
    public string GetLongDescription(Player player)
    {
        string str = "You are ";
        str += description;
        str += ".\n";
        if (player.IsAlive()) 
        {
            str += GetExitString();
        }
        return str;
    }

	// Return the room that is reached if we go from this room in direction
	// "direction". If there is no room in that direction, return null.
	public Room GetExit(string direction)
	{
		if (exits.ContainsKey(direction))
		{
			return exits[direction];
		}
		return null;
	}

	// Return a string describing the room's exits, for example
	// "Exits: north, west".
	private string GetExitString()
	{
		string str = "Exits:";
		int countCommas = 0;
		foreach (string key in exits.Keys)
		{
			if (countCommas != 0)
			{
				str += ",";
			}
			str += " " + key;
			countCommas++;
		}

		return str;
	}
}
