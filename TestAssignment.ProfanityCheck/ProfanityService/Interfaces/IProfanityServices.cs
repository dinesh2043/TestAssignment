using System.Threading.Tasks;

using TestAssignment.ProfanityCheck.Model;

namespace TestAssignment.ProfanityCheck.ProfanityService.Interfaces
{
    public interface IProfanityServices
    {
        Task<ProfanityCheckResult> CheckProfanity(string text);
    }
}
