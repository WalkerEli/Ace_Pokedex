using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Models.ViewModels.Type;

namespace TeamAceProject.Controllers
{
    // Builds and serves the full Pokemon type matchup chart
    public class TypeController : Controller
    {
        // All 18 Pokemon types in display order
        private static readonly string[] Types =
        {
            "Normal", "Fire", "Water", "Electric", "Grass", "Ice",
            "Fighting", "Poison", "Ground", "Flying", "Psychic", "Bug",
            "Rock", "Ghost", "Dragon", "Dark", "Steel", "Fairy"
        };

        // Maps each type name to its row/column index in the damage chart
        private static readonly Dictionary<string, int> TypeIndex =
            Types.Select((t, i) => (t, i)).ToDictionary(x => x.t, x => x.i);

        // The 18x18 damage multiplier grid, computed once at startup
        private static readonly double[][] Chart = BuildChart();

        // Fills the chart with all attack-vs-defence multipliers using Gen 6 rules
        private static double[][] BuildChart()
        {
            int n = Types.Length;
            var chart = new double[n][];
            for (int i = 0; i < n; i++)
            {
                chart[i] = new double[n];
                for (int j = 0; j < n; j++)
                    chart[i][j] = 1.0;
            }

            void Set(string atk, string def, double val) =>
                chart[TypeIndex[atk]][TypeIndex[def]] = val;

            // Normal
            Set("Normal", "Rock", 0.5);   Set("Normal", "Ghost", 0);    Set("Normal", "Steel", 0.5);

            // Fire
            Set("Fire", "Grass", 2);      Set("Fire", "Ice", 2);        Set("Fire", "Bug", 2);   Set("Fire", "Steel", 2);
            Set("Fire", "Fire", 0.5);     Set("Fire", "Water", 0.5);    Set("Fire", "Rock", 0.5); Set("Fire", "Dragon", 0.5);

            // Water
            Set("Water", "Fire", 2);      Set("Water", "Ground", 2);    Set("Water", "Rock", 2);
            Set("Water", "Water", 0.5);   Set("Water", "Grass", 0.5);   Set("Water", "Dragon", 0.5);

            // Electric
            Set("Electric", "Water", 2);  Set("Electric", "Flying", 2);
            Set("Electric", "Electric", 0.5); Set("Electric", "Grass", 0.5); Set("Electric", "Dragon", 0.5);
            Set("Electric", "Ground", 0);

            // Grass
            Set("Grass", "Water", 2);     Set("Grass", "Ground", 2);    Set("Grass", "Rock", 2);
            Set("Grass", "Fire", 0.5);    Set("Grass", "Grass", 0.5);   Set("Grass", "Poison", 0.5);
            Set("Grass", "Flying", 0.5);  Set("Grass", "Bug", 0.5);     Set("Grass", "Dragon", 0.5); Set("Grass", "Steel", 0.5);

            // Ice
            Set("Ice", "Grass", 2);       Set("Ice", "Ground", 2);      Set("Ice", "Flying", 2);  Set("Ice", "Dragon", 2);
            Set("Ice", "Fire", 0.5);      Set("Ice", "Water", 0.5);     Set("Ice", "Ice", 0.5);   Set("Ice", "Steel", 0.5);

            // Fighting
            Set("Fighting", "Normal", 2); Set("Fighting", "Ice", 2);    Set("Fighting", "Rock", 2);
            Set("Fighting", "Dark", 2);   Set("Fighting", "Steel", 2);
            Set("Fighting", "Poison", 0.5); Set("Fighting", "Flying", 0.5); Set("Fighting", "Psychic", 0.5);
            Set("Fighting", "Bug", 0.5);  Set("Fighting", "Fairy", 0.5);
            Set("Fighting", "Ghost", 0);

            // Poison
            Set("Poison", "Grass", 2);    Set("Poison", "Fairy", 2);
            Set("Poison", "Poison", 0.5); Set("Poison", "Ground", 0.5); Set("Poison", "Rock", 0.5); Set("Poison", "Ghost", 0.5);
            Set("Poison", "Steel", 0);

            // Ground
            Set("Ground", "Fire", 2);     Set("Ground", "Electric", 2); Set("Ground", "Poison", 2);
            Set("Ground", "Rock", 2);     Set("Ground", "Steel", 2);
            Set("Ground", "Grass", 0.5);  Set("Ground", "Bug", 0.5);
            Set("Ground", "Flying", 0);

            // Flying
            Set("Flying", "Grass", 2);    Set("Flying", "Fighting", 2); Set("Flying", "Bug", 2);
            Set("Flying", "Electric", 0.5); Set("Flying", "Rock", 0.5); Set("Flying", "Steel", 0.5);

            // Psychic
            Set("Psychic", "Fighting", 2); Set("Psychic", "Poison", 2);
            Set("Psychic", "Psychic", 0.5); Set("Psychic", "Steel", 0.5);
            Set("Psychic", "Dark", 0);

            // Bug
            Set("Bug", "Grass", 2);       Set("Bug", "Psychic", 2);     Set("Bug", "Dark", 2);
            Set("Bug", "Fire", 0.5);      Set("Bug", "Fighting", 0.5);  Set("Bug", "Flying", 0.5);
            Set("Bug", "Ghost", 0.5);     Set("Bug", "Steel", 0.5);     Set("Bug", "Fairy", 0.5);

            // Rock
            Set("Rock", "Fire", 2);       Set("Rock", "Ice", 2);        Set("Rock", "Flying", 2);  Set("Rock", "Bug", 2);
            Set("Rock", "Fighting", 0.5); Set("Rock", "Ground", 0.5);   Set("Rock", "Steel", 0.5);

            // Ghost
            Set("Ghost", "Psychic", 2);   Set("Ghost", "Ghost", 2);
            Set("Ghost", "Dark", 0.5);
            Set("Ghost", "Normal", 0);

            // Dragon
            Set("Dragon", "Dragon", 2);
            Set("Dragon", "Steel", 0.5);
            Set("Dragon", "Fairy", 0);

            // Dark
            Set("Dark", "Psychic", 2);    Set("Dark", "Ghost", 2);
            Set("Dark", "Fighting", 0.5); Set("Dark", "Dark", 0.5);     Set("Dark", "Fairy", 0.5);

            // Steel
            Set("Steel", "Ice", 2);       Set("Steel", "Rock", 2);      Set("Steel", "Fairy", 2);
            Set("Steel", "Fire", 0.5);    Set("Steel", "Water", 0.5);   Set("Steel", "Electric", 0.5); Set("Steel", "Steel", 0.5);

            // Fairy
            Set("Fairy", "Fighting", 2);  Set("Fairy", "Dragon", 2);    Set("Fairy", "Dark", 2);
            Set("Fairy", "Fire", 0.5);    Set("Fairy", "Poison", 0.5);  Set("Fairy", "Steel", 0.5);

            return chart;
        }

        // Returns the type chart view with the prebuilt multiplier grid
        [HttpGet]
        public IActionResult Index()
        {
            return View(new TypeChartViewModel { Types = Types, Chart = Chart });
        }
    }
}
