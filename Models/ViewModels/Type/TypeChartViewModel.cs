namespace TeamAceProject.Models.ViewModels.Type
{
    public class TypeChartViewModel
    {
        public string[] Types { get; set; } = Array.Empty<string>();
        public double[][] Chart { get; set; } = Array.Empty<double[]>(); // [attacker][defender]
    }
}
