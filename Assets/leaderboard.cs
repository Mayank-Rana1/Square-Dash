using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using System.Threading.Tasks;
using System.Text;
using Unity.Services.Leaderboards.Exceptions;
using Unity.Services.Core.Environments;
using TMPro;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class LeaderboardUI : MonoBehaviour
{
    public AdsInitializer adManager;
    // --- UI REFERENCES ---
    [Header("Leaderboard UI")]
    public Button topScoresButton;
    public GameObject leaderboardPanel;
    public Text leaderboardText; // Use TextMeshProUGUI for better formatting
    public Button backButton;

    [Header("Edit Name UI")]
    public Button editNameButton; // Button on your main screen to open the edit panel
    public GameObject editNamePanel;
    public TMP_InputField nameInput;
    public Button saveButton;
    public Button cancelButton;

    private const string leaderboardId = "SquareDash"; // <-- your leaderboard ID
    private const string PlayerName = "";
    private int leaderPanel = 0;
    async void Start()
    {
        TMP_Text buttonText = editNameButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = PlayerPrefs.GetString(PlayerName, "Null");
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    public async void ShowTopScores(int limit = 10)
    {
        // Show panel and prepare UI
        if (leaderboardPanel != null) leaderboardPanel.SetActive(true);
        if (leaderboardText == null) return;
        leaderboardText.text = "     Loading...";

        // Fetch scores
        var page = await LeaderboardsService.Instance.GetScoresAsync(
            leaderboardId,
            new GetScoresOptions { Limit = limit }
        );

        // Handle empty
        if (page?.Results == null || page.Results.Count == 0)
        {
            leaderboardText.text = "   No scores submitted yet.";
            return;
        }

        // --- IMPROVED FORMATTING ---
        // Use a StringBuilder to efficiently create the text
        var sb = new StringBuilder();

        // Header: Note the spaces between the format items for clear columns
        // {Rank, -5} = 5 characters, left-aligned
        // {Name, -18} = 18 characters, left-aligned
        // {Score, 8} = 8 characters, right-aligned
        sb.AppendLine(string.Format("{0,-8} {1,-26} {2,10}", "  Rank", "Name", "Score"));
        sb.AppendLine("-----------------------------------------------------"); // A simple separator line

        // Get local player ID for highlighting
        string localPlayerId = AuthenticationService.Instance.IsSignedIn ? AuthenticationService.Instance.PlayerId : null;

        foreach (var entry in page.Results)
        {
            // Get and clean up the display name
            string displayName = !string.IsNullOrWhiteSpace(entry.PlayerName) ? entry.PlayerName : entry.PlayerId;
            displayName = Regex.Replace(displayName ?? "", @"#\d+$", "");

            // Truncate name if it's too long to prevent breaking the layout
            if (displayName.Length > 15)
            {
                displayName = displayName.Substring(0, 15) + "...";
            }

            // Mark the current player
            if (!string.IsNullOrEmpty(localPlayerId) && entry.PlayerId == localPlayerId)
            {
                displayName += " (YOU)";
            }

            // Format the row with the same spacing as the header
            sb.AppendLine(string.Format("{0,-10} {1,-18} {2,13}",
                "   "+(entry.Rank + 1) + ".",
                displayName,
                entry.Score));
        }

        leaderboardText.text = sb.ToString();
    }


    public async void SubmitScore(long score)
    {
        await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
    }
    public void OnShowTopScoresButton()
    {
        ShowTopScores();
    }
    public void PanalBack()
    {
        leaderboardPanel.SetActive(false);
        leaderPanel++;
        if (leaderPanel>=5){
            adManager.ShowInterAd();
            leaderPanel = 0;
        }
    }

    public async Task upSaveName()
    {
        name=nameInput.text;
        PlayerPrefs.SetString(PlayerName,name);
        PlayerPrefs.Save();
        await AuthenticationService.Instance.UpdatePlayerNameAsync(name);
    }
    public async void clickSave()
    {
        if (saveButton != null) saveButton.interactable = false;

        await upSaveName();

        if (saveButton != null)
        {
            saveButton.interactable = true;
            TMP_Text buttonText = editNameButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = PlayerPrefs.GetString(PlayerName, "Null");
        }
        if (editNamePanel != null) editNamePanel.SetActive(false);
    }
}
