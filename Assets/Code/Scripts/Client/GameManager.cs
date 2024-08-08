using Assets.Code.Scripts.Client;
using Assets.Code.Scripts.Common.Commands;
using Assets.Code.Scripts.Common.Sockets;
using Assets.Code.Scripts.Server;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Code.Scripts.Client.EventInfos;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static readonly Lazy<GameManager> lazy = new Lazy<GameManager>(() => new GameManager());

    #endregion

    #region Fields

    private AsyncTCPClient client;
    private List<Player> players;
    private Player currentPlayer;
    private Card currentCard;
    private Dictionary<CommandType, Action<Command, AsyncTCPClient>> commandCallbacks;

    #endregion

    #region Constructors

    private GameManager()
    {
        client = new AsyncTCPClient();
        client.DataReceived += HandleDataReceived;

        commandCallbacks = new Dictionary<CommandType, Action<Command, AsyncTCPClient>>();
        commandCallbacks.Add(CommandType.InformClientGameInit, HandleGameInit);
        commandCallbacks.Add(CommandType.InformStatus, HandleStatus);
    }

    #endregion

    #region Events

    public event Action<GameInitializedEventArgs> GameInitialized;
    public event Action<InformStatusEventArgs> InformStatus;
    public event Action Reset;

    #endregion

    #region Properties

    public static GameManager Instance
    {
        get
        {
            return lazy.Value;
        }
    }

    #endregion

    #region Methods

    public void ResetAll()
    {
        players.Clear();
        currentCard = null;

        Reset?.Invoke();
    }

    public async Task TakeCard()
    {
        var data = new
        {
            CardsPlayed = (object)null,
            currentPlayer.Hand
        };

        Command command = new Command(CommandType.PlayDone, data);
        command.Data = command.ObjectToJsonData();

        await client.SendAsync(command);
    }

    public async Task PlayCards(List<Card> cards)
    {
        var data = new
        {
            CardsPlayed = cards,
            currentPlayer.Hand
        };

        Command command = new Command(CommandType.PlayDone, data);
        command.Data = command.ObjectToJsonData();

        await client.SendAsync(command);
    }

    #region Command Handlers

    private void HandleStatus(Command command, AsyncTCPClient client)
    {
        (Player Winner, Card ExposedCard, List<Player> Players, string CurrentTurnPlayerID, object SpecialInfo) result = command
            .DataToJson<(Player Winner, Card ExposedCard, List<Player> Players, string CurrentTurnPlayerID, object SpecialInfo)>();

        List<Card> currentSelectableCards = null;
        bool isCurrentTurn = currentPlayer.ID.Equals(result.CurrentTurnPlayerID);

        // The current turn is the player's turn
        if (isCurrentTurn)
        {
            currentSelectableCards = GetSelectableCards(result.ExposedCard);
        }

        InformStatus?.Invoke(new InformStatusEventArgs(result.Winner, result.ExposedCard, result.Players,
            isCurrentTurn, currentSelectableCards, result.SpecialInfo));
    }

    private void HandleGameInit(Command command, AsyncTCPClient client)
    {
        (Card ExposedCard, List<Player> Players, string FirstPlayerID) result = command
            .DataToJson<(Card ExposedCard, List<Player> Players, string FirstPlayerID)>();

        List<Card> currentSelectableCards = null;
        bool isCurrentTurn = currentPlayer.ID.Equals(result.FirstPlayerID);

        // The current turn is the player's turn
        if (isCurrentTurn)
        {
            currentSelectableCards = GetSelectableCards(result.ExposedCard);
        }

        GameInitialized?.Invoke(new GameInitializedEventArgs(currentSelectableCards, result.Players, isCurrentTurn));
    }

    #endregion

    /// <summary>
    /// Gets all the cards that can be played on the current exposed card
    /// </summary>
    /// <returns></returns>
    private List<Card> GetSelectableCards(Card exposedCard)
    {
        List<Card> currentSelectableCards = new List<Card>();

        currentSelectableCards.AddRange(currentPlayer.Hand.Where(card => card.CanBePutOn(exposedCard)));

        return currentSelectableCards;
    }

    private void HandleDataReceived(string message, AsyncTCPClient client)
    {
        Command command = Command.FromString(message);
        commandCallbacks[command.CommandType]?.Invoke(command, client);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion
}
