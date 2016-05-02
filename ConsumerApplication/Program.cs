namespace SlowServiceAdaptor
{
	using MassTransit;
	using SlowSingleThreadApplication;
	using System;
	using System.Diagnostics;
	using System.Threading.Tasks;
	using Topshelf;

	public class Program
	{
		private static int Main(string[] args)
		{
			return (int)HostFactory.Run(cfg => cfg.Service(x => new SlowServiceListener()));
		}
	}

	internal class SlowServiceListener : ServiceControl
	{
		private IBusControl _bus;

		public async Task Execute(SlowServiceRequest message)
		{
			Process p = new Process();
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.FileName = @"D:\Source\PriorityMessageBus\SlowSingleThreadApplication\bin\Debug\SlowSingleThreadApplication.exe";
			p.StartInfo.Arguments = String.Format("\"{0} executed with priority {1}\"", message.Message, message.Priority);
			p.Start();

			string output = p.StandardOutput.ReadToEnd();
			p.WaitForExit();

			await Console.Out.WriteLineAsync(output);
		}

		public bool Start(HostControl hostControl)
		{
			_bus = ConfigureBus();
			_bus.Start();
			return true;
		}

		private IBusControl ConfigureBus()
		{
			return Bus.Factory.CreateUsingRabbitMq(cfg =>
			{
				var host = cfg.Host(new Uri("rabbitmq://monkey.floatingman.org/"), h =>
				{
					h.Username("admin");
					h.Password("admin");
					cfg.EnablePriority(5);
					cfg.PrefetchCount = 1;
				});
				cfg.ReceiveEndpoint(host, "slow_service_queue", e =>
				{
					e.Handler<SlowServiceRequest>(context => Execute(context.Message));
					e.EnablePriority(5);
					e.PrefetchCount = 1;
				});
			});
		}

		public bool Stop(HostControl hostControl)
		{
			_bus?.Stop();// TimeSpan.FromSeconds(30));
			return true;
		}
	}
}