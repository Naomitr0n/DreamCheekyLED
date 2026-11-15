Task? _task = null;
CancellationTokenSource _cts = new CancellationTokenSource();

// Start new processing thread
_task = RunLoop(_cts.Token);

// Do stuff here

// Cleanly exit
_cts.Cancel();
_task?.Wait();