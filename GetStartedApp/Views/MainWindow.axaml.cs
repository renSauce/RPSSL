using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GetStartedApp.Views;

public partial class MainWindow : Window
{
    private readonly Random rng = new();
    private int playerScore = 0;
    private int botScore = 0;
    private const int TargetScore = 3;

    public MainWindow()
    {
        InitializeComponent();
        TargetScoreText.Text = TargetScore.ToString();
        UpdateScoreboard();
        SetPrompt("Choose a shape:");
    }
    
    // Enums because we have more than 2 outcomes
    private enum Shape { Rock, Paper, Scissors, Spock, Lizard }
    private enum Outcome { PlayerWins, BotWins, Draw }
    

    private void Pick_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button b || b.Tag is null) return;

        var playerShape = Enum.Parse<Shape>(b.Tag.ToString()!);
        var botShape = (Shape)rng.Next(0, 5);

        var outcome = Resolve(playerShape, botShape);
        ApplyOutcome(outcome);

        UpdateUI(playerShape, botShape, outcome);
        CheckForMatchEnd();
    }

    private void Reset_Click(object? sender, RoutedEventArgs e)
    {
        playerScore = 0;
        botScore = 0;
        UpdateScoreboard();
        SetPrompt("Choose a shape:");
        RoundDetail.Text = string.Empty;
    }
    
    //Use private & static because we dont want this to be accessed outside this class or changed
    private static Outcome Resolve(Shape player, Shape bot)
    {   
        // Egdecase: bot and players choose the same Shape
        if (player == bot) return Outcome.Draw;

        // All player winning combinations
        return (player, bot) switch

        {
            (Shape.Rock, Shape.Scissors) => Outcome.PlayerWins,
            (Shape.Rock, Shape.Lizard) => Outcome.PlayerWins,

            (Shape.Paper, Shape.Rock) => Outcome.PlayerWins,
            (Shape.Paper, Shape.Spock) => Outcome.PlayerWins,

            (Shape.Scissors, Shape.Paper) => Outcome.PlayerWins,
            (Shape.Scissors, Shape.Lizard) => Outcome.PlayerWins,

            (Shape.Lizard, Shape.Spock) => Outcome.PlayerWins,
            (Shape.Lizard, Shape.Paper) => Outcome.PlayerWins,

            (Shape.Spock, Shape.Scissors) => Outcome.PlayerWins,
            (Shape.Spock, Shape.Rock) => Outcome.PlayerWins,

            //All Player winning combinations have been tested, therefore the bot wins.
            // _ = if nothing else matches. This way i do not have to list all bot winning combinations.
            _ => Outcome.BotWins
        };
    }

    private void UpdateUI(Shape player, Shape bot, Outcome outcome)
    {
        var (pEmoji, bEmoji) = (EmojiOf(player), EmojiOf(bot));

        var verdict = outcome switch
        {
            Outcome.PlayerWins => "Player wins",
            Outcome.BotWins    => "Bot wins",
            _                  => "Draw."
        };

        SetPrompt(verdict);
        RoundDetail.Text = $"You {pEmoji}  vs  {bEmoji} Bot";
        UpdateScoreboard();
    }

// A simple switch statement to increment the score
    private void ApplyOutcome(Outcome outcome)
    {
        switch (outcome)
        {
            case Outcome.PlayerWins:
                playerScore++;
                break;
            case Outcome.BotWins:
                botScore++;
                break;
            case Outcome.Draw:
                break;
        }
    }
    
    //Update the scoreboard text: use toString to convert int to string
    private void UpdateScoreboard()
    {
        PlayerScoreText.Text = playerScore.ToString();
        BotScoreText.Text = botScore.ToString();
    }

    // The game ends when target score is reached. Display msg and reset score.
    // Use void because we dont need to return anything just display a message and reset.
    private void CheckForMatchEnd()
    {
        if (playerScore >= TargetScore || botScore >= TargetScore)
        {
            var msg = playerScore > botScore ? "You won" : "U suck - Bot won";
            SetPrompt(msg + " (Scores reset.)");
            playerScore = 0;
            botScore = 0;
            UpdateScoreboard();
            RoundDetail.Text = string.Empty;
        }
    }
    
    // Helper function to aviod repeating code for all prompt changes
    private void SetPrompt(string text) => RoundCaption.Text = text;
    
    // Assign the enum strings to emojis for a better UI
    private static string EmojiOf(Shape s) => s switch
    {
        Shape.Rock => "🪨",
        Shape.Paper => "📄",
        Shape.Scissors => "✂️",
        Shape.Spock => "🖖",
        Shape.Lizard => "🦎",
        _ => "❓"
    };
}
