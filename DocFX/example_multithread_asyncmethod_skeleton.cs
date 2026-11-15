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