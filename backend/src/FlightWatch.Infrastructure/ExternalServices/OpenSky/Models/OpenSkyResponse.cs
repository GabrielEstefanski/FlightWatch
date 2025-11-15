<<<<<<< HEAD
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
            return new List<OpenSkyStateVector>();

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

=======
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
            return new List<OpenSkyStateVector>();

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

>>>>>>> 46c33f55e4420f09ba269fe78a84593a0d6687a2
