private static async Task RunLoop(IEnumerable<FrameData> frames, USBInterface u, CancellationToken cancellationToken)
{
	try
	{
		// Pre-loop setup

		do
		{
			// Loop logic
		} while (!cancellationToken.IsCancellationRequested);
	}
	catch (OperationCanceledException) { } // Can tbe thrown by method calls monitoring IsCancellationRequested flag
	finally
	{
		// Post-loop housekeeping goes here
	}
}