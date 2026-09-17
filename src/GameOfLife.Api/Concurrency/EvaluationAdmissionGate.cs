using GameOfLife.Api.Options;
using Microsoft.Extensions.Options;

namespace GameOfLife.Api.Concurrency;

public sealed class EvaluationAdmissionGate : IEvaluationAdmissionGate, IDisposable
{
    private readonly SemaphoreSlim _semaphore;

    public EvaluationAdmissionGate(IOptions<GameOfLifeOptions> options)
    {
        var permits = options.Value.ResolvedMaxConcurrentEvaluations;
        _semaphore = new SemaphoreSlim(permits, permits);
    }

    public async Task<IDisposable?> TryAcquireAsync(TimeSpan timeout, CancellationToken ct)
    {
        var acquired = await _semaphore.WaitAsync(timeout, ct).ConfigureAwait(false);
        return acquired ? new Lease(_semaphore) : null;
    }

    public void Dispose() => _semaphore.Dispose();

    private sealed class Lease : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private bool _released;

        public Lease(SemaphoreSlim semaphore) => _semaphore = semaphore;

        public void Dispose()
        {
            if (_released)
            {
                return;
            }

            _released = true;
            _semaphore.Release();
        }
    }
}
