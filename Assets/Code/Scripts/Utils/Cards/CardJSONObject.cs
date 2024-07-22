using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// Change destination card
/// </summary>
public class CardJSONObject
{
	#region Fields

	private string country;
	private DestinationCard[] subCards;

	#endregion

	#region Properties

	public string Country
	{
		get { return country; }
		set { country = value; }
	}

	public DestinationCard[] SubCards
	{
		get { return subCards; }
		set { subCards = value; }
	}

	#endregion
}
