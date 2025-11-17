using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlightWatch.Infrastructure.ExternalServices.OpenSky.Models;

public class OpenSkyResponse
{
    [JsonPropertyName("time")]
    public long Time { get; set; }

    [JsonPropertyName("states")]
    public JsonElement? States { get; set; }

    public List<OpenSkyStateVector> GetStateVectors()
    {
        if (States == null || States.Value.ValueKind != JsonValueKind.Array)
            return [];

        var stateVectors = new List<OpenSkyStateVector>();

        foreach (var stateElement in States.Value.EnumerateArray())
        {
            if (stateElement.ValueKind != JsonValueKind.Array)
                continue;

            var stateArray = stateElement.EnumerateArray().ToArray();
            stateVectors.Add(OpenSkyStateVector.FromJsonElements(stateArray));
        }

        return stateVectors;
    }
}

