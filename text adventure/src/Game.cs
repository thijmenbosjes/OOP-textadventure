using System;
using System.Reflection.Metadata;

class Game
{
	private Parser parser;
	private Player player;
	Item stone = new Item(15, "Teleportation_stone");
	Item medkit = new Item(20, "Medkit");

	public Game()
	{
		parser = new Parser();
		player = new Player();
		CreateRooms();
	}

	private void CreateRooms()
	{
		Room outside = new Room("outside the main entrance of the school");
		Room theatre = new Room("in a lecture theatre");
		Room pub = new Room("in the campus pub");
		Room lab = new Room("in a computing lab");
		Room office = new Room("in the computing admin office");
		Room hallway = new Room("in the university hallway");
		Room up = new Room("on the second floor of the university");
		Room storage = new Room("in the storage room");
		
		outside.AddExit("east", theatre);
		outside.AddExit("south", lab);
		outside.AddExit("west", pub);

		theatre.AddExit("west", outside);
		theatre.AddExit("door", hallway);

		hallway.AddExit("theatre", theatre);
		hallway.AddExit("stairs", up);

		up.AddExit("down", hallway);
		up.AddExit("storageroom", storage);

		storage.AddExit("hallway", hallway);

		pub.AddExit("east", outside);

		lab.AddExit("north", outside);
		lab.AddExit("east", office);

		office.AddExit("west", lab);

		theatre.Chest.Put("teleportation_stone", stone);
		lab.Chest.Put("medkit", medkit);

		player.CurrentRoom = outside;
	}

	public void Play()
	{
		PrintWelcome();

		bool finished = false;
		while (!finished)
		{
			Command command = parser.GetCommand();
			finished = ProcessCommand(command);
		}
		Console.WriteLine("Thanks for playing my game.");
		Console.WriteLine("press [enter] to continue playing.");
		Console.ReadLine();
	}

	private void PrintWelcome()
	{
		Console.WriteLine();
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));
	}

private bool ProcessCommand(Command command)
{
    bool wantToQuit = false;

    if (!player.IsAlive() && command.CommandWord != "quit")
    {
        Console.WriteLine("You bled out, you died...");
		Console.WriteLine("You can only use the command:");
		Console.WriteLine("quit");
        return wantToQuit;
    }

    if(command.IsUnknown())
    {
        Console.WriteLine("I don't know what you mean...");
        return wantToQuit; 
    }

    switch (command.CommandWord)
    {
        case "help":
            PrintHelp();
            break;
        case "look":
            Look();
            break;
		case "take":
			Take(command);
			break;
		case "drop":
			Drop(command);
			break;
        case "status":
            Health();
            break;
        case "go":
            GoRoom(command);
            break;
		case "use":
			UseItem(command);
			break;
        case "quit":
            wantToQuit = true;
            break;
    }

    return wantToQuit;
}

	private void PrintHelp()
	{
		Console.WriteLine("you are lost and bleeding out, find the exit before your inevitable death");
		Console.WriteLine("You wander around at the school.");
		Console.WriteLine();
		parser.PrintValidCommands();
	}

	private void Look()
	{
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));

		Dictionary<string, Item> roomItems = player.CurrentRoom.Chest.GetItems();
		if (roomItems.Count > 0)
		{
			Console.WriteLine("Items in this room:");
			foreach (var itemEntry in roomItems)
			{
				Console.WriteLine($"{itemEntry.Value.Description} - ({itemEntry.Value.Weight} kg)");
			}
		}
	}


	private void Take(Command command)
{
    if (!command.HasSecondWord())
    {
        Console.WriteLine("Take what?");
        return;
    }

    string itemName = command.SecondWord.ToLower();

    bool success = player.TakeFromChest(itemName);

    if (success && itemName == "teleportation_stone")
    {
        Console.WriteLine("You crushed the stone in your hand and a bright light shines down. when you open your eyes you are back in your room. it was all a dream ");
        Environment.Exit(0); // End the game
    }
}

	private void Drop(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("Drop what?");
			return;
		}

		string itemName = command.SecondWord.ToLower();

		bool success = player.DropToChest(itemName);


	}

	private void Health()
	{
		Console.WriteLine($"Your health is: {player.GetHealth()}");

		Dictionary<string, Item> items = player.GetItems();

		if (items.Count > 0)
		{
			Console.WriteLine("Your current items:");

			foreach (var itemEntry in items)
			{
				Console.WriteLine($"- {itemEntry.Key}: ({itemEntry.Value.Weight} kg)");
			}
		}
		else
		{
			Console.WriteLine("You have no items in your inventory.");
		}
	}

	private void GoRoom(Command command)
	{
		if(!command.HasSecondWord())
		{
			Console.WriteLine("Go where?");
			return;
		}

		string direction = command.SecondWord;

		Room nextRoom = player.CurrentRoom.GetExit(direction);
		if (nextRoom == null)
		{
			Console.WriteLine("There is no door to "+direction+"!");
			return;
		}

		player.Damage(10 );
		player.CurrentRoom = nextRoom;
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player));
		
		if (!player.IsAlive()) 
		{
			Console.WriteLine("Your vision blurs, the world fades. Your wounds draining your strength. You collapse, you have bled out..");
		}
	}

	private void UseItem(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("Use what?");
			return;
		}

		string itemName = command.SecondWord.ToLower();

		player.Use(itemName);
	}

}

