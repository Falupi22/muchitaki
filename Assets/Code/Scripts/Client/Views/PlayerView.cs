using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    #region Fields

    private List<CardView> hand;
    private string playerName;
    private List<CardView> selectedCards;

    #endregion

    #region Constructors

    public PlayerView(string name, List<CardView> hand = null)
    {
        playerName = name;
        this.hand = hand;
        selectedCards = new List<CardView>();
    }

    #endregion

    #region Properties

    public List<CardView> Hand
    {
        get { return hand; }
    }

    public string Name { get => playerName; }

    public List<CardView> SelectedCards { get => selectedCards; }


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
