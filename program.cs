using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

// Интерфейс для сериализации
public interface ISerializable
{
    string ToJson();
    void FromJson(string json);
}

// Абстрактный класс для участников ( Players и т.д.)
public abstract class Participant
{
    public string Name { get; protected set; }
    public int Rating { get; protected set; }

    public Participant(string name)
    {
        Name = name;
        Rating = 0;
    }

    public virtual void AdjustRating(int delta)
    {
        Rating += delta;
    }
}

// Класс Player наследует Participant и реализует интерфейс ISerializable
public class Player : Participant, ISerializable
{
    public Player(string name) : base(name) { }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public void FromJson(string json)
    {
        var obj = JsonSerializer.Deserialize<Player>(json);
        if (obj != null)
        {
            this.Name = obj.Name;
            this.Rating = obj.Rating;
        }
    }
}


public class Game : ISerializable
{
    public string Title { get; private set; }

    public Game(string title)
    {
        Title = title;
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public void FromJson(string json)
    {
        var obj = JsonSerializer.Deserialize<Game>(json);
        if (obj != null)
        {
            this.Title = obj.Title;
        }
    }
}

//интерфейс для оценки рейтинга
public interface IRatingSystem
{
    void UpdateRating(Match match);
}

//реализует интерфейс IRatingSystem
public class RatingSystem : IRatingSystem
{
    public void UpdateRating(Match match)
    {
        Player winner = match.GetWinner();
        if (winner != null)
        {
            winner.AdjustRating(1);
        }
        foreach (var player in match.Players)
        {
            if (player != winner)
                player.AdjustRating(-1);
        }
    }
}

//
public class Match
{
    public Game Game { get; private set; }
    public List<Player> Players { get; private set; }
    public Dictionary<Player, int> Scores { get; private set; }

    public Match(Game game, List<Player> players)
    {
        Game = game;
        Players = new List<Player>(players);
        Scores = new Dictionary<Player, int>();
        foreach (var p in players)
        {
            Scores[p] = 0;
        }
    }

    public void SetScores(Dictionary<Player, int> scores)
    {
        foreach (var pair in scores)
        {
            if (Scores.ContainsKey(pair.Key))
                Scores[pair.Key] = pair.Value;
        }
    }

    public Player GetWinner()
    {
        Player highScorer = null;
        int highScore = int.MinValue;
        foreach (var pair in Scores)
        {
            if (pair.Value > highScore)
            {
                highScore = pair.Value;
                highScorer = pair.Key;
            }
        }
        return highScorer;
    }

    public string ToJson()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        return JsonSerializer.Serialize(this, options);
    }
}

// для хранения коллекций данных
public class Storage
{
    public List<Player> Players { get; set; } = new List<Player>();
    public List<Game> Games { get; set; } = new List<Game>();
    public List<Match> Matches { get; set; } = new List<Match>();

    public void SaveToFile(string filename)
    {
        try
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filename, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Данные успешно сохранены.");
        }
    }

    public static Storage LoadFromFile(string filename)
    {
        try
        {
            if (File.Exists(filename))
            {
                var json = File.ReadAllText(filename);
                var storage = JsonSerializer.Deserialize<Storage>(json);
                if (storage != null)
                    return storage;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
        }
        return new Storage();
    }
}

// Меню взаимодействия
class Program
{
    static Storage storage = new Storage();

    static void Main()
    {
        string filename = "data.json";
        storage = Storage.LoadFromFile(filename);

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("меню:");

            Console.WriteLine("1 Добавить игрок");

            Console.WriteLine("2Добавить игру");

            Console.WriteLine("3 Провести матч");
            Console.WriteLine("4 Просмотр игроков");
            Console.WriteLine("5 Просмотр игр");
            Console.WriteLine("6 Сохранить и выйти");
            Console.Write("выберите число: ");
            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        AddPlayer();
                        break;

                    case "2":
                        AddGame();
                        break;

                    case "3":
                        ConductMatch();
                        break;

                    case "4":
                        ShowPlayers();
                        break;

                    case "5":
                        ShowGames();
                        break;

                    case "6":
                        storage.SaveToFile(filename);
                        exit = true;
                        break;


                    default:
                        Console.WriteLine("неправильный выбор");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка:");
            }
        }
    }

    static void AddPlayer()
    {
        Console.Write("введите имя нового игрока: ");

        string name = Console.ReadLine();

        var player = new Player(name);

        storage.Players.Add(player);

        Console.WriteLine("игрок добавлен.");
    }

    static void AddGame()
    {

        //lj,fdktybt buhs
        Console.Write("введите название игры: ");
        string title = Console.ReadLine();
        var game = new Game(title);
        storage.Games.Add(game);
        Console.WriteLine("игра добавлена.");
    }

    static void ConductMatch()
    {
        if (storage.Games.Count == 0 || storage.Players.Count < 2)
        {
            Console.WriteLine("недостаточно данных.");
            return;
        }

        Console.WriteLine("выберите игру:");


        for (int i = 0; i < storage.Games.Count; i++)

            Console.WriteLine($"{i + 1}. {storage.Games[i].Title}");


        int gameIndex = int.Parse(Console.ReadLine()) - 1;

        Console.WriteLine("выберите игроков по номерам через ',' :");

        for (int i = 0; i < storage.Players.Count; i++)

            Console.WriteLine($"{i + 1}. {storage.Players[i].Name}");

        string[] selectedPlayers = Console.ReadLine().Split(',');


        var playersInMatch = new List<Player>();

        foreach (var sp in selectedPlayers)
        {
            int idx = int.Parse(sp.Trim()) - 1;

            if (idx >= 0 && idx < storage.Players.Count)
                playersInMatch.Add(storage.Players[idx]);
        }

        if (playersInMatch.Count < 2)
        {
            Console.WriteLine("недостаточно игроков для матча.");
            return;
        }

        var match = new Match(storage.Games[gameIndex], playersInMatch);

        //вод очков
        var scores = new Dictionary<Player, int>();


        foreach (var p in playersInMatch)
        {


            Console.Write($"Введите очки для {p.Name}: ");

            int score = int.Parse(Console.ReadLine());

            scores[p] = score;

        }
        match.SetScores(scores);

        //обнова рейтинга
        var ratingSystem = new RatingSystem();

        ratingSystem.UpdateRating(match);


        storage.Matches.Add(match);

        Console.WriteLine("Матч завершён. Рейтинг участников обновлён.");
    }

    static void ShowPlayers()
    {


        Console.WriteLine("Список игроков:");

        foreach (var p in storage.Players)
        {


            Console.WriteLine($"{p.Name} - Рейтинг: {p.Rating}");


        }

    }

    static void ShowGames()
    {


        Console.WriteLine("список игр:");


        foreach (var g in storage.Games)
        {

            Console.WriteLine(g.Title);

        }
    }
}
