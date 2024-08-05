using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountrySpecialCard : SpecialCard
{
    #region Fields

    private string country;

    #endregion

    #region Properties

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

        if (card is CountryCard)
        {
            // If there is a country card with no other cards on it, another country card could be put
            canBePutOn = true;
        }
        if (card is DestinationCard destinationCard)
        {
            canBePutOn = destinationCard.Country == country;
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
