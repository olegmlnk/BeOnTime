using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Interfaces;

public interface IIdeaRepository
{
    Task<IReadOnlyList<Idea>> GetAllAsync(Guid userId);
    Task<Idea?> GetByIdAsync(Guid userId, Guid id);
    Task<Idea> CreateAsync(Idea idea);
    Task UpdateAsync(Idea idea);
}
