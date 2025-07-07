using Microsoft.EntityFrameworkCore;
using Wordlie.Infrastructure;
using Wordlie.Infrastructure.Database;

namespace Wordlie.Services;

public class WordService
{
    private readonly ApplicationContext _context;
    private List<string> _words = new();
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    public WordService(ApplicationContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Чтобы лишний раз не загружать всю коллекцию, загрузим 1 раз и будем её хранить.
    /// Вопрос в том, нужно ли нам вообще хранить весь список...
    /// Вообще-то нет, ведь по сути со словами из бд мы работаем только при
    /// 1) Получении рандомного слова (1 sql запрос, нам не нужна коллекция)
    /// 2) Проверке, есть ли слово в бд. Но на сервере нам также не нужна бд
    ///
    /// Однако я хочу, бд маленькая (меньше 100 к строк), а проект локальный, поэтому пусть будет так, осознаю последствия
    /// =)
    ///
    /// upd:
    /// Что ж, оказалось, бд не такая и маленькая)
    /// </summary>
    /// <returns></returns>

    public async Task<List<string>> GetWords()
    {
        if (_words.Count > 0)
            return _words;

        await _cacheLock.WaitAsync();
        try
        {
            if (_words.Count == 0)
            {
                _words = await _context.Words
                    .AsNoTracking()
                    .Select(x => x.Word)
                    .ToListAsync();
            }
            return _words;
        }
        finally
        {
            _cacheLock.Release();
        }
    }
    
    public async Task<string> GetRandomWordAsync()
    {
        var count = await _context.Words.CountAsync();
        var randomIndex = new Random().Next(0, count);
        return await _context.Words
            .OrderBy(x => x.Id)
            .Skip(randomIndex)
            .Select(x => x.Word)
            .FirstAsync();
    }

    public async Task<bool> ContainsWordAsync(string word) 
        => await _context.Words.
            Select(x => x.Word)
            .ContainsAsync(word);
}
