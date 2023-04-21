// <copyright file="CoalescingLimiter.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.Util;

public class CoalescingLimiter<T>
{
    private readonly Func<T, Task> _func;
    private readonly List<TaskCompletionSource> _waitingTasks = new();
    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly ITimer _timer;
    private T? _lastParam;
    private bool _isInTimeout;

    public CoalescingLimiter(Func<T, Task> func, double timeout, ITimer? timer = null)
    {
        _func = func;
        _timer = timer ?? new TimerWrapper();
        _timer.Interval = timeout;
        _timer.AutoReset = false;
        _timer.Elapsed += HandleTimer;
    }

    public async Task Execute(T param)
    {
        await _semaphore.WaitAsync();
        if (_isInTimeout)
        {
            _lastParam = param;
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _waitingTasks.Add(tcs);
            _semaphore.Release();
            await tcs.Task;
        }
        else
        {
            _timer.Start();
            _isInTimeout = true;

            _semaphore.Release();
            await _func.Invoke(param);
        }
    }

    private async void HandleTimer(object? source, DateTime dateTime)
    {
        await _semaphore.WaitAsync();
        if (_waitingTasks.Any())
        {
            await _func.Invoke(_lastParam ?? throw new InvalidOperationException());
            foreach (var tcs in _waitingTasks)
            {
                tcs.SetResult();
            }

            _waitingTasks.Clear();
        }

        _isInTimeout = false;
        _semaphore.Release();
    }
}