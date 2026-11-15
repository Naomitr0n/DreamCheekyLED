private static Task? _task = null;
private static CancellationTokenSource _cts = new CancellationTokenSource();

private static void Main(string[] args)
{
	// Do setup stuff here
	bool shouldFinish = false;

	using USBInterface u = USBInterface.Open();
	{
		do
		{
			// Do initial pre-multithread setup here

			// End running tasks
			if (_task != null)
			{
				_cts.Cancel();
				_task.Wait();
			}

			// Start new processing thread
			_cts = new CancellationTokenSource();
			_task = RunLoop(_cts.Token);

			// Continue after multithread branch here

		} while (!shouldFinish)

		// Cleanly exit
		_cts.Cancel();
		_task?.Wait();
	}
}