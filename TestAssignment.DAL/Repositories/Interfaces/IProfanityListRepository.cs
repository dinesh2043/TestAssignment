using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestAssignment.DAL.Repositories.Interfaces
{
    public interface IProfanityListRepository
    {
        Task<bool> AddProfanity(string profanity);
        Task<bool> DeleteProfanity(string profanity);
        Task<List<string>> GetProfanityList();
    }
}
