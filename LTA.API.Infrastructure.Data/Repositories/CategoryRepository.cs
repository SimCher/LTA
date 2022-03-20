using LTA.API.Domain.Interfaces;
using LTA.API.Domain.Models;
using LTA.API.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LTA.API.Infrastructure.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly LtaApiContext _context;

    public CategoryRepository(LtaApiContext context)
    {
        _context = context;
    }

    public IEnumerable<Category> GetAll()
        => _context.Categories.Include(c => c.Topics);

    public async Task<Category> AddAsync(string name)
    {
        var newCategory = _context.Categories.Add(new Category
        {
            Name = name
        });

        await _context.SaveChangesAsync();

        return newCategory.Entity;
    }

    public async Task UpdateAsync(int id, string name)
    {
        throw new NotImplementedException();
    }

    public async Task<Category> UpdateAndReturnAsync(int id, string name)
    {
        throw new NotImplementedException();
    }

    public Category? Get(int id)
        => _context.Categories.Find(id);

    public bool Find(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<Category>> GetAll(string[] categories)
    {
        var categoryObjects = new Category[categories.Length];
        int index = 0;

        foreach (var categoryName in categories)
        {
            var categoryObject = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName) ??
                                 await AddAsync(categoryName) ??
                                 throw new NullReferenceException("Cannot get new category in CategoryRepository");

            categoryObjects[index] = categoryObject;
            index++;
        }

        await _context.SaveChangesAsync();

        return categoryObjects;
    }
}