using ExpenseTracker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExpenseTracker.Services
{
    public class FileExpenseService : IExpenseService
    {
        private readonly string _filePath;
        private readonly object _fileLock = new();

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public FileExpenseService(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            EnsureFileExists();
        }

        private void EnsureFileExists()
        {
            var dir = Path.GetDirectoryName(_filePath) ?? ".";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        private List<Expense> ReadAll()
        {
            lock (_fileLock)
            {
                var json = File.ReadAllText(_filePath);
                try
                {
                    var list = JsonSerializer.Deserialize<List<Expense>>(json, _jsonOptions);
                    return list ?? new List<Expense>();
                }
                catch
                {
                    // If parsing fails, reset file to empty list to avoid repeated errors.
                    var empty = new List<Expense>();
                    File.WriteAllText(_filePath, JsonSerializer.Serialize(empty, _jsonOptions));
                    return empty;
                }
            }
        }

        private void WriteAll(List<Expense> list)
        {
            var temp = _filePath + ".tmp";
            var json = JsonSerializer.Serialize(list, _jsonOptions);
            lock (_fileLock)
            {
                File.WriteAllText(temp, json);
                File.Copy(temp, _filePath, overwrite: true);
                File.Delete(temp);
            }
        }

        // Interface implementations
        public Task<List<Expense>> GetAllAsync()
        {
            var list = ReadAll().OrderByDescending(e => e.Date).ToList();
            return Task.FromResult(list);
        }

        public Task<Expense?> GetAsync(int id)
        {
            var e = ReadAll().FirstOrDefault(x => x.Id == id);
            return Task.FromResult<Expense?>(e);
        }

        public Task AddAsync(Expense e)
        {
            var list = ReadAll();

            var nextId = list.Any() ? list.Max(x => x.Id) + 1 : 1;
            e.Id = nextId;

            if (e.CreatedAt == default) e.CreatedAt = DateTime.UtcNow;
            list.Add(e);
            WriteAll(list);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Expense e)
        {
            var list = ReadAll();
            var existing = list.FirstOrDefault(x => x.Id == e.Id);
            if (existing == null) throw new KeyNotFoundException("Expense not found");

            existing.Title = e.Title;
            existing.Amount = e.Amount;
            existing.Category = e.Category;
            existing.Date = e.Date;
            existing.Notes = e.Notes;
            // preserve CreatedAt
            WriteAll(list);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var list = ReadAll();
            var existing = list.FirstOrDefault(x => x.Id == id);
            if (existing != null)
            {
                list.Remove(existing);
                WriteAll(list);
            }
            return Task.CompletedTask;
        }
    }
}
