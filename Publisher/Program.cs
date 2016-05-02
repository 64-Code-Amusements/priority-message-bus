using MassTransit;
using SlowSingleThreadApplication;
using System;

namespace Publisher
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var busControl = ConfigureBus();
			busControl.Start();
			System.Threading.Thread.Sleep(5000);
			for (var i = 0; i < 100; i++)
//			do
			{
				//				Console.WriteLine("Enter message (or quit to exit)");
				//				Console.Write("> ");
				//				string value = Console.ReadLine();

				//				if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
				//					break;
				char letter = (char)((i % 26) + 65);
				string value = new string(letter, 5);
				byte priority = (byte)new Random().Next(1, 6);
				busControl.Publish<SlowServiceRequest>(new SlowServiceRequest
				{
					Message = value,
					Priority = priority
				}, x => x.SetPriority(priority));
			System.Threading.Thread.Sleep(1000);
			}
//			while (true);

			busControl.Stop();
		}

		private static IBusControl ConfigureBus()
		{
			return Bus.Factory.CreateUsingRabbitMq(cfg =>
			{
				cfg.Host(new Uri("rabbitmq://monkey.floatingman.org/"), h =>
					{
						h.Username("admin");
						h.Password("admin");
					cfg.EnablePriority(5);
					cfg.PrefetchCount = 1;
					});
			});
		}
	}
}