using System.Collections.Concurrent;

namespace ClinicAppointments.Api.Features.Specializations;

public sealed class InMemorySpecializationService : ISpecializationService
{
    private readonly ConcurrentDictionary<int, SpecializationResponse> _specializations = new();
    private int _nextId;

    public Task<IReadOnlyCollection<SpecializationResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyCollection<SpecializationResponse> result =
            _specializations.Values.ToArray();

        return Task.FromResult(result);
    }

    public Task<SpecializationResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _specializations.TryGetValue(id, out var specialization);

        return Task.FromResult(specialization);
    }

    public Task<SpecializationResponse> CreateAsync(
        CreateSpecializationRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = Interlocked.Increment(ref _nextId);

        var specialization = new SpecializationResponse(
            id,
            request.Name,
            request.Description);

        _specializations[id] = specialization;

        return Task.FromResult(specialization);
    }

    public Task<bool> UpdateAsync(
        int id,
        UpdateSpecializationRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_specializations.ContainsKey(id))
        {
            return Task.FromResult(false);
        }

        var updated = new SpecializationResponse(
            id,
            request.Name,
            request.Description);

        var result = ((ICollection<KeyValuePair<int, SpecializationResponse>>)_specializations)
            .Remove(new KeyValuePair<int, SpecializationResponse>(
                id,
                _specializations[id]));

        if (!result)
        {
            return Task.FromResult(false);
        }

        _specializations[id] = updated;

        return Task.FromResult(true);
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