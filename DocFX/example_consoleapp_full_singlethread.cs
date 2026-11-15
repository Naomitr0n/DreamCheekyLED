private static void Main(string[] args)
{
	string filePathInput = "";

	// This is done in case a path that includes spaces is supplied, without being surrounded by quotes
	filePathInput = string.Join(' ', args.Prepend(filePathInput)).Trim();

	using USBInterface u = USBInterface.Open();
	{
		do
		{
			//Ignore blank input and repeat prompt
			if (!String.IsNullOrEmpty(filePathInput))
			{
				//If there is a quoted section at the start of the input, isolate it and ignore the rest
				if (filePathInput.StartsWith('"') && filePathInput[1..].Contains('"'))
				{
					filePathInput = filePathInput[1..];
					filePathInput = filePathInput[..filePathInput.IndexOf('"')];
				}

				if (!Path.Exists(filePathInput))
				{
					Console.WriteLine($"ERROR! File not found: \"{filePathInput}\"");
				}
				else
				{
					try
					{
						u.SetFrameBuffer(ImageLoader.StaticScreenDataFromPath(filePathInput, autoSetBrightness: true).MakeFrameBuffer());

						if (!u.IsRunning)
						{
							u.Run();
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine($"ERROR! {ex.Message}"); ;
					}
				}
			}

			Console.Write("Enter file path to display (or 'Q' to quit): ");
			filePathInput = (Console.ReadLine() ?? "").Trim();
		} while (!filePathInput.Equals("Q", StringComparison.CurrentCultureIgnoreCase));

		u.Stop();
	}
}