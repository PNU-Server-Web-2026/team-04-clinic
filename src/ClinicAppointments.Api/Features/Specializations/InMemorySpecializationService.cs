using System.Collections.Concurrent;

namespace ClinicAppointments.Api.Features.Specializations;

public sealed class InMemorySpecializationService : ISpecializationService
{
    private readonly ConcurrentDictionary<int, Specialization> _specializations = new();
    private int _nextId;

    public Task<IReadOnlyCollection<SpecializationResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyCollection<SpecializationResponse> result =
            _specializations.Values
                .Select(s => new SpecializationResponse(
                    s.Id,
                    s.Name,
                    s.Description))
                .ToArray();

        return Task.FromResult(result);
    }

    public Task<SpecializationResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_specializations.TryGetValue(id, out var specialization))
        {
            return Task.FromResult<SpecializationResponse?>(null);
        }

        var result = new SpecializationResponse(
            specialization.Id,
            specialization.Name,
            specialization.Description);

        return Task.FromResult<SpecializationResponse?>(result);
    }

    public Task<SpecializationResponse> CreateAsync(
        CreateSpecializationRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = Interlocked.Increment(ref _nextId);

        var specialization = new Specialization
        {
            Id = id,
            Name = request.Name,
            Description = request.Description
        };

        _specializations[id] = specialization;

        var result = new SpecializationResponse(
            specialization.Id,
            specialization.Name,
            specialization.Description);

        return Task.FromResult(result);
    }

    public Task<bool> UpdateAsync(
        int id,
        UpdateSpecializationRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        while (_specializations.TryGetValue(id, out var current))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var updated = new Specialization
            {
                Id = id,
                Name = request.Name,
                Description = request.Description
            };

            if (_specializations.TryUpdate(id, updated, current))
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    public Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var deleted = _specializations.TryRemove(id, out _);

        return Task.FromResult(deleted);
    }
}