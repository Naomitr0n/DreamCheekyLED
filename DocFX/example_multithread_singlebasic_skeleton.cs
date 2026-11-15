private static Task? _task = null;
private static CancellationTokenSource _cts = new CancellationTokenSource();

private static void Main(string[] args)
{
	// Start new processing thread
	_cts = new CancellationTokenSource();
	_task = RunLoop(_cts.Token);

	// Do stuff here

	// Cleanly exit
	_cts.Cancel();
	_task?.Wait();
}

private static async Task RunLoop(CancellationToken cancellationToken)
{
	try
	{
		// Pre-loop setup

		do
		{
			// Loop logic
		} while (!cancellationToken.IsCancellationRequested);
	}
	catch (OperationCanceledException) { } // Will often throw errors on cancellation
	finally
	{
		// Post-loop housekeeping goes here
	}
}