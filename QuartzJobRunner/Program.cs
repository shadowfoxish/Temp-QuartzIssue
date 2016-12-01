using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Topshelf;

namespace QuartzJobRunner
{
	class Program
	{
		/// <summary>
		/// To run this sample,
		/// 1) create a folder 'C:\QuartzTest' and place a text file 'CacheFile.txt' in it.
		///     Any time you make a change to that file, the quartz jobs should be re-scheduled.
		/// 2) Setup a quartz DB using the scripts from the Quartz download package
		/// 3) Run two instances of this application. It should come alive correctly and log that the job is working.
		/// 4) Then, edit the cache file and watch both services try to save the same job at the same time. This will 
		/// in most attempts generate an exception in at least one of the two app instances.
		/// </summary>
		static void Main(string[] args)
		{
			Console.WriteLine("Starting App");

			HostFactory.Run(x =>
			{
				x.Service<MyService>(s =>
				{
					s.ConstructUsing(name => new MyService());
					s.WhenStarted(tc => tc.Start());
					s.WhenStopped(tc => tc.Stop());
				});
				x.RunAsLocalSystem();
				x.SetDescription("Quartz Demo");
				x.SetDisplayName("Quartz Demo");
				x.SetServiceName("Quartz Demo");
			});
		}
	}

	class MyService
	{
		IScheduler quartzScheduler;

		public void Start()
		{
			SettingsCache.SettingsCacheFileChanged += SettingsCache_SettingsCacheFileChanged;

			NameValueCollection properties = new NameValueCollection();
			properties["quartz.scheduler.instanceName"] = "AUTO";
			properties["quartz.scheduler.instanceId"] = "AUTO";
			properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
			properties["quartz.threadPool.threadCount"] = "5";
			properties["quartz.threadPool.threadPriority"] = "Normal";
			properties["quartz.jobStore.misfireThreshold"] = "60000";
			properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
			properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz";
			properties["quartz.jobStore.useProperties"] = "false";
			properties["quartz.jobStore.dataSource"] = "default";
			properties["quartz.jobStore.clusterCheckinInterval"] = "15000";
			//TODO Update this prefix if your table names are prefixed differently
			properties["quartz.jobStore.tablePrefix"] = "QRTZ";
			properties["quartz.jobStore.clustered"] = "true";
			properties["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz";
			//TODO Update this connection string to point to your enviro
			properties["quartz.dataSource.default.connectionString"] = @"Server=machinename;Database=Jobs;User ID=userid;Password=classified;Min Pool Size=1;Max Pool Size=15;Connection Timeout=180;Pooling=True;";
			properties["quartz.dataSource.default.provider"] = "SqlServer-20";

			quartzScheduler = new StdSchedulerFactory(properties).GetScheduler();
			
			quartzScheduler.Start();

			ConfigureJob();
		}

		private void ConfigureJob()
		{
			quartzScheduler.Clear();

			IJobDetail job = JobBuilder.Create<DummyJob>()
					.WithIdentity(nameof(DummyJob))
					.StoreDurably(false)
					.Build();

			//Each time the cache is reset, actual app goes out and grabs new configuration from the database, which would then be accessed
			//  here to potentially change the schedule of the job
			TimeSpan interval = new TimeSpan(0, 0, 0, 5); //5s
			Console.WriteLine($"Creating new {nameof(DummyJob)} with interval of {interval}");

			ITrigger trigger = TriggerBuilder.Create()
				.WithIdentity(nameof(DummyJob) + "Trigger", "MyTriggers")
				.WithSimpleSchedule(s => s.WithInterval(interval).RepeatForever())
				.ForJob(job.Key)
				.StartNow()
				.Build();

			quartzScheduler.ScheduleJob(job, trigger);
		}

		private void SettingsCache_SettingsCacheFileChanged(object sender, EventArgs e)
		{
			Console.WriteLine("Reconfiguring job due to cachefile write");
			ConfigureJob();
		}

		public void Stop()
		{
			quartzScheduler.Shutdown();
		}
	}

	class DummyJob : IJob
	{
		public void Execute(IJobExecutionContext context)
		{
			Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} {Thread.CurrentThread.Name} - I'm running now, yay");
		}
	}

	static class SettingsCache
	{
		static FileSystemWatcher ChangeMonitor;

		public static event EventHandler SettingsCacheFileChanged;
		private static long LastChangeNotification;

		private static void NotifyEvent(object sender)
		{
			//Debounce the FS events to once every half-second
			//Antivirus or other FS hooks cause multiple events
			long now = DateTime.Now.Ticks;
			if ((now - SettingsCache.LastChangeNotification) < (TimeSpan.TicksPerMillisecond * 500))
			{
				return;
			}
			SettingsCache.LastChangeNotification = now;

			SettingsCacheFileChanged?.Invoke(sender, new EventArgs());
		}

		static SettingsCache()
		{
			SettingsCache.ChangeMonitor = new FileSystemWatcher("C:\\QuartzTest\\", "CacheFile.txt");
			SettingsCache.ChangeMonitor.NotifyFilter = NotifyFilters.LastWrite;
			SettingsCache.ChangeMonitor.Changed += (s, e) => NotifyEvent(s);
			SettingsCache.ChangeMonitor.EnableRaisingEvents = true;
		}
	}
}
