using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TeamAceProject.Models.Dtos.PokeApi;
using TeamAceProject.Models.ViewModels.Type;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    public class TypeController : Controller
    {
        private static readonly string[] Types =
        {
            "Normal", "Fire", "Water", "Electric", "Grass", "Ice",
            "Fighting", "Poison", "Ground", "Flying", "Psychic", "Bug",
            "Rock", "Ghost", "Dragon", "Dark", "Steel", "Fairy"
        };

        private readonly IPokeApiRepository _pokeApi;
        private readonly IMemoryCache _cache;

        public TypeController(IPokeApiRepository pokeApi, IMemoryCache cache)
        {
            _pokeApi = pokeApi;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string cacheKey = "type_chart";
            if (!_cache.TryGetValue(cacheKey, out double[][]? chart) || chart == null)
            {
                chart = await BuildChartFromApiAsync();
                _cache.Set(cacheKey, chart, TimeSpan.FromHours(24));
            }

            return View(new TypeChartViewModel { Types = Types, Chart = chart });
        }

        private async Task<double[][]> BuildChartFromApiAsync()
        {
            var typeIndex = Types
                .Select((t, i) => (t, i))
                .ToDictionary(x => x.t, x => x.i, StringComparer.OrdinalIgnoreCase);

            int n = Types.Length;
            var chart = new double[n][];
            for (int i = 0; i < n; i++)
            {
                chart[i] = new double[n];
                for (int j = 0; j < n; j++)
                    chart[i][j] = 1.0;
            }

            var tasks = Types.Select(t => _pokeApi.GetTypeByNameAsync(t.ToLowerInvariant())).ToList();
            TypeDetailDto?[] results = await Task.WhenAll(tasks);

            for (int atkIdx = 0; atkIdx < Types.Length; atkIdx++)
            {
                TypeDetailDto? typeDetail = results[atkIdx];
                if (typeDetail == null) continue;

                foreach (var target in typeDetail.Damage_Relations.Double_Damage_To)
                    if (typeIndex.TryGetValue(target.Name, out int defIdx))
                        chart[atkIdx][defIdx] = 2.0;

                foreach (var target in typeDetail.Damage_Relations.Half_Damage_To)
                    if (typeIndex.TryGetValue(target.Name, out int defIdx))
                        chart[atkIdx][defIdx] = 0.5;

                foreach (var target in typeDetail.Damage_Relations.No_Damage_To)
                    if (typeIndex.TryGetValue(target.Name, out int defIdx))
                        chart[atkIdx][defIdx] = 0.0;
            }

            return chart;
        }
    }
}
