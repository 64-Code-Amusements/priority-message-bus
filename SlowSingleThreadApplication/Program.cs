namespace SlowSingleThreadApplication
{
	using System;

	internal class Program
	{
		private static void Main(string[] args)
		{
			string message;
			if (args.Length == 0)
			{
				message = "Default message.";
			}
			else
			{
				message = args[0];
			}
			var napTime = new Random().Next(500,2000);
			System.Threading.Thread.Sleep(napTime);
			Console.Write("[{0,6}]\t{1}", napTime, message);
		}
	}

	public class SlowServiceRequest
	{
		public string Message { get; set; } = "Standard message";

		public int Priority { get; set; } = 0;
	}
}