using ExpenseTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTracker.Services
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetAllAsync();
        Task<Expense?> GetAsync(int id);   // nullable expense when not found
        Task AddAsync(Expense e);
        Task UpdateAsync(Expense e);
        Task DeleteAsync(int id);
    }
}
