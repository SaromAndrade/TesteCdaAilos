using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Program
{
    private static readonly HttpClient client = new HttpClient();

    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year).GetAwaiter().GetResult();

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year).GetAwaiter().GetResult();

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static async Task<int> getTotalScoredGoals(string team, int year)
    {
        int totalGoals = 0;

        // Contar gols como time1 (mandante)
        totalGoals += await CountGoals(client, team, year, "team1");

        // Contar gols como time2 (visitante)
        totalGoals += await CountGoals(client, team, year, "team2");

        return totalGoals;
    }
    private static async Task<int> CountGoals(HttpClient client, string team, int year, string teamKey)
    {
        int totalGoals = 0;
        int page = 1;

        while (true)
        {
            string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&{teamKey}={team}&page={page}";
            string response = await client.GetStringAsync(url);
            JObject data = JObject.Parse(response);

            foreach (var match in data["data"])
            {
                totalGoals += int.Parse(match[$"{teamKey}goals"].ToString());
            }

            int totalPages = data["total_pages"].Value<int>();
            if (page >= totalPages)
                break;

            page++;
        }
        return totalGoals;
    }

}