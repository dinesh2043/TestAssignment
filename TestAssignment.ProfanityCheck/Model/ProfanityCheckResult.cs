namespace TestAssignment.ProfanityCheck.Model
{
    public class ProfanityCheckResult
    {
        public bool ContainsProfanity { get; set; }
        public int ProfanityWordCount { get; set; }
        public double ProcessingTime { get; set; }
    }
}
