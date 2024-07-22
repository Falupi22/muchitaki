using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GlobalSpecialCard : SpecialCard
{
    #region Methods

    public override bool CanBePutOn(Card card)
    {
        // Cards from this type can be put on any card.
        return true;
    }

    #endregion
}
