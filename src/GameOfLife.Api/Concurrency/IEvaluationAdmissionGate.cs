namespace GameOfLife.Api.Concurrency;

/// <summary>
/// Bounds how many CPU-bound evaluations run at once. Distinct from the input caps in
/// GameOfLifeOptions: caps bound the cost of one request, this bounds how many expensive requests
/// run concurrently. See docs/design.md §8.4.
/// </summary>
public interface IEvaluationAdmissionGate
{
    /// <summary>Returns a lease to release on completion, or null if no slot was available within <paramref name="timeout"/>.</summary>
    Task<IDisposable?> TryAcquireAsync(TimeSpan timeout, CancellationToken ct);
}
