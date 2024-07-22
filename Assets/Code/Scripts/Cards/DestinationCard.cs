using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationCard : Card
{
    #region Fields

    private string country;
    private string destination;
    private bool isCapital;
    private bool isMostPopular;

    #endregion

    #region Properties


    public bool IsCapital
    {
        get { return isCapital; }
        set { isCapital = value; }
    }

    public bool IsMostPopular
    {
        get { return isMostPopular; }
        set { isMostPopular = value; }
    }

    public string Destination
    {
        get { return destination; }
        set { destination = value; }
    }

    public string Country
    {
        get { return country; }
        set { country = value; }
    }

    #endregion

    #region Methods

    public override bool CanBePutOn(Card card)
    {
        bool canBePutOn = false;

        if (card is DestinationCard destinationCard)
        {
            // A destination card can be put on another destination card of the same country
            canBePutOn = destinationCard.Country == country;
        }
        else if (card is CountryCard countryCard) {
            canBePutOn = countryCard == country;
        }

        return canBePutOn;
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
