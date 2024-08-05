using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryCard : Card
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
        else if (card is CountrySpecialCard countrySpecialCard)
        {
            canBePutOn = countrySpecialCard.Country == Country;
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

    #region Operators

    public static bool operator ==(CountryCard countryCard, CountryCard other) 
    {
        return countryCard.Country == other.Country;
    }

    public static bool operator !=(CountryCard countryCard, CountryCard other)
    {
        return countryCard.Country != other.Country;
    }

    public static bool operator ==(CountryCard countryCard, string other)
    {
        return countryCard.Country == other;
    }

    public static bool operator !=(CountryCard countryCard, string other)
    {
        return countryCard.Country != other;
    }

    #endregion
}
