using System;
using System.Collections.Generic;


class Game
{
    public string Title;

    public Game(string title)
    {
        Title = title;
    }
}


class Player
{
    public string Name;
    public int Rating;

    public Player(string name)
    {
        Name = name;
        Rating = 0; 
    }
}


class Match
{
    public Game Game;

    public List<Player> Players;

    public Dictionary<Player, int> Scores = new Dictionary<Player, int>();



    public Match(Game game, List<Player> players)
    {

        Game = game;

        Players = players;

       
        foreach (var player in players)
        { 
            Scores[player] = 0;

        }
    }

   
    public void Setscores(Dictionary<Player, int> scores)
    {
        foreach (var pair in scores)
        {
            if (Scores.ContainsKey(pair.Key))
            {
                Scores[pair.Key] = pair.Value;
            }
        }
    }


    public Player winner()
    {
        Player playerhighscore = null;
        int highscore = int.MinValue;




        //foreach (KeyValuePair<Player, int> pair in Scores)
        //{
        //    int crntscore = pair.Value;
        //    if (crntscore >= highscore)
        //    {

        //        highscore = crntscore;
        //        playerhighscore = pair.Key;
        //    }
        //}




        // игрок - очки
        foreach (KeyValuePair<Player, int> pair in Scores)
        {
            int crntscore = pair.Value;
            if (crntscore > highscore)
            {
                
                highscore = crntscore; 
                playerhighscore = pair.Key;
            }
        }




        return playerhighscore; 
    }
}


class Rating
{
    public static void Update(Match match)
    {
        Player winner = match.winner();
        if (winner != null)
        {
            winner.Rating += 1; 
        }




        //foreach (player in match.Players)
        //{
        //Player.Rating = ;
        //}

        foreach (Player player in match.Players)
        {
            if (player != winner)
            {
                player.Rating -= 1; 
            }
        }
    }
}


class Program
{
    static void Main()
    {

        var game = new Game("нарды");


        var player1 = new Player("саша");
        
        
        var player2 = new Player("паша");
        
        List<Player> players = new List<Player> { player1, player2 };

        var match = new Match(game, players);

  
        var scores = new Dictionary<Player, int>
        
        
        {
        
            
            [player1] = 25,

            [player2] = 30
        
        
        };
        
        
        match.Setscores(scores);



        Rating.Update(match);




        Player winner = match.winner();



        



        Console.WriteLine($"название игры {game.Title}");

        Console.WriteLine($"победитель:{winner.Name}");
        
        
        
        foreach (Player p in players)
        {

            Console.WriteLine($"{p.Name}:рейтинг {p.Rating}");
        
        
        
        
        }
    }
}
