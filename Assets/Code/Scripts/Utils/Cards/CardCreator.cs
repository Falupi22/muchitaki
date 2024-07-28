using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// Change destination card
/// </summary>
public static class CardCreator
{
    #region Methods

    public static (SpecialCard[], CountryCard[], DestinationCard[]) Create()
    {
        using (StreamReader reader = new StreamReader("../Cards.json"))
        {
            string json = reader.ReadToEnd();
            List<CardJSONObject> items = JsonConvert.DeserializeObject<List<CardJSONObject>>(json);
            items.ForEach(item => Debug.Log(JsonConvert.SerializeObject(item)));
        }

        return (null, null, null);
    }

    #endregion
}
