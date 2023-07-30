using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;

public class PokeAPIController : MonoBehaviour
{
    // UI elements to display Pokemon information
    public RawImage pokemonImage;
    public TMP_Text pokemonTypeLabel, pokemonStatLabel;
    public TMP_Text pokemonNameText;
    public TMP_Text pokemonNumber;
    public TMP_Text[] pokemonTypesArray; // An array of Text elements to display Pokemon types
    public TMP_Text[] pokemonStatsArray; // An array of Text elements to display Pokemon stats

    // Base URL for the PokeAPI
    private readonly string basePokemonURL = "https://pokeapi.co/api/v2/";

    private void Start()
    {
        // Initialize UI elements with default values
        pokemonImage.texture = Texture2D.blackTexture;
        pokemonNameText.text = "";
        pokemonNumber.text = "";
        pokemonTypeLabel.text = "";
        pokemonStatLabel.text = "";

        // Clear Pokemon type texts
        foreach (TMP_Text pokemonType in pokemonTypesArray)
        {
            pokemonType.text = "";
        }
        
        // Clear Pokemon stat texts
        foreach (TMP_Text pokemonStats in pokemonStatsArray)
        {
            pokemonStats.text = "";
        }
    }

    public void OnRandomPokemonButtonClick()
    {
        // Generate a random Pokemon index (ID)
        int randomPokemonIndex = Random.Range(1, 899); // Pokemon IDs range from 1 to 898;  Min Include -> 1 ; Max Excluded -> 899

        // Reset UI elements to indicate loading
        pokemonImage.texture = Texture2D.blackTexture;
        pokemonNameText.text = "L O A D I N G . . .";
        pokemonNumber.text = "# " + randomPokemonIndex;
        pokemonStatLabel.text = "";
        pokemonTypeLabel.text = "";

        // Clear Pokemon type texts
        foreach (TMP_Text pokemonType in pokemonTypesArray)
        {
            pokemonType.text = "";
        }

        foreach (TMP_Text pokemonStat in pokemonStatsArray)
        {
            pokemonStat.text = "";
        }

        // Fetch Pokemon data from the PokeAPI
        StartCoroutine(GetPokemonAtIndex(randomPokemonIndex));
    }

    IEnumerator GetPokemonAtIndex(int pokemonIndex)
    {
        /* ==================================
         * Get pokemon info
         * ================================== */

        // Build the URL to fetch data for the specified Pokemon index
        string pokemonURL = basePokemonURL + "pokemon/" + pokemonIndex.ToString(); // Example : https://pokeapi.co/api/v2/pokemon/151   */

        // Create a UnityWebRequest to fetch the Pokemon data
        UnityWebRequest pokemonInfoRequest = UnityWebRequest.Get(pokemonURL);

        // Send the request and wait for the response
        yield return pokemonInfoRequest.SendWebRequest();

        // Check for errors in the response
        if ((pokemonInfoRequest.result == UnityWebRequest.Result.ConnectionError) || (pokemonInfoRequest.result == UnityWebRequest.Result.ProtocolError))
        {
            // Log the error and exit the coroutine
            Debug.LogError(pokemonInfoRequest.error);
            yield break;
        }

        // Parse the JSON response from the PokeAPI
        JSONNode pokemonInfo = JSON.Parse(pokemonInfoRequest.downloadHandler.text);

        // Extract relevant information from the JSON data
        string pokemonName = pokemonInfo["name"];
        string pokemonSpriteURL = pokemonInfo["sprites"]["front_default"];

        // Extract the types of the Pokemon from the JSON data
        JSONNode pokemonTypes = pokemonInfo["types"];
        string[] pokemonTypeNames = new string[pokemonTypes.Count];

        // Extract type names and store them in reverse order (for display)
        for (int i = 0, j = pokemonTypes.Count - 1; i < pokemonTypes.Count; i++, j--)
        {
            pokemonTypeNames[j] = pokemonTypes[i]["type"]["name"];
        }

        // Extract the stats and the stat value of the Pokemon from the JSON data
        JSONNode pokemonStats = pokemonInfo["stats"];
        string[] pokemonStatNames = new string[pokemonStats.Count];
        string[] pokemonStatValues = new string[pokemonStats.Count];
        string[] pokemonStatCombined = new string[pokemonStats.Count];

        // Extract stat names and values and store them in reverse order (for display)
        for (int i = 0, j = pokemonStats.Count - 1; i < pokemonStats.Count; i++, j--)
        {
            pokemonStatNames[j] = pokemonStats[i]["stat"]["name"];
            pokemonStatValues[j] = pokemonStats[i]["base_stat"];
            //Debug.Log(pokemonStatNames[j] + " " + pokemonStatValues[j]);
            pokemonStatCombined[j] = pokemonStatNames[j] + " " + pokemonStatValues[j];
        }


        /* ==================================
         * Get pokemon sprite
         * ================================== */

        // Fetch the Pokemon sprite from the provided URL
        UnityWebRequest pokemonSpriteRequest = UnityWebRequestTexture.GetTexture(pokemonSpriteURL);

        // Create a UnityWebRequest to fetch the Pokemon sprite data
        yield return pokemonSpriteRequest.SendWebRequest();

        // Check for errors in the response
        if ((pokemonSpriteRequest.result == UnityWebRequest.Result.ConnectionError) || (pokemonSpriteRequest.result == UnityWebRequest.Result.ProtocolError))
        {
            // Log the error and exit the coroutine
            Debug.LogError(pokemonSpriteRequest.error);
            yield break;
        }

        /* ==================================
         * Set UI Objects
         * ================================== */

        // Set the fetched Pokemon sprite and update UI elements with the data
        pokemonImage.texture = DownloadHandlerTexture.GetContent(pokemonSpriteRequest);
        pokemonImage.texture.filterMode = FilterMode.Point;

        pokemonTypeLabel.text = "Type";
        pokemonStatLabel.text = "Stat";
        pokemonNameText.text = CapitalizeFirstLetter(pokemonName);

        // Update UI elements with Pokemon types
        for (int i = 0; i < pokemonTypeNames.Length; i++)
        {
            pokemonTypesArray[i].text = CapitalizeFirstLetter(pokemonTypeNames[i]);
        }

        for (int i = 0; i < pokemonStatCombined.Length; i++)
        {
            pokemonStatsArray[i].text = CapitalizeFirstLetter(pokemonStatCombined[i]);
        }
    }

    // Helper function to capitalize the first letter of a string
    private string CapitalizeFirstLetter(string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1);
    }
}