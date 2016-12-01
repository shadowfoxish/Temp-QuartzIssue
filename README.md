# Temp-QuartzIssue

When you run the example (based on the instructions at the top of Program.cs) you'll get the following exception in one of the two app instances leading to a somewhat unrecoverable state.

```
Quartz.JobPersistenceException: Couldn't store job: Unable to store Job: 'DEFAULT.DummyJob', because one already exists with this identification. ---> Quartz.ObjectAlreadyExistsException: Unable to store Job: 'DEFAULT.DummyJob', because one already exists with this identification.
   at Quartz.Impl.AdoJobStore.JobStoreSupport.StoreJob(ConnectionAndTransactionHolder conn, IJobDetail newJob, Boolean replaceExisting) in c:\Program Files (x86)\Jenkins\workspace\Quartz.NET\src\Quartz\Impl\AdoJobStore\JobStoreSupport.cs:line 913
   --- End of inner exception stack trace ---
   at Quartz.Impl.AdoJobStore.JobStoreSupport.StoreJob(ConnectionAndTransactionHolder conn, IJobDetail newJob, Boolean replaceExisting) in c:\Program Files (x86)\Jenkins\workspace\Quartz.NET\src\Quartz\Impl\AdoJobStore\JobStoreSupport.cs:line 928
   at Quartz.Impl.AdoJobStore.JobStoreSupport.<>c__DisplayClass4.<StoreJobAndTrigger>b__3(ConnectionAndTransactionHolder conn) in c:\Program Files (x86)\Jenkins\workspace\Quartz.NET\src\Quartz\Impl\AdoJobStore\JobStoreSupport.cs:line 855
   at Quartz.Impl.AdoJobStore.JobStoreSupport.ExecuteInNonManagedTXLock[T](String lockName, Func`2 txCallback, Func`3 txValidator) in c:\Program Files (x86)\Jenkins\workspace\Quartz.NET\src\Quartz\Impl\AdoJobStore\JobStoreSupport.cs:line 3562
   at Quartz.Impl.AdoJobStore.JobStoreSupport.ExecuteInNonManagedTXLock[T](String lockName, Func`2 txCallback) in c:\Program Files (x86)\Jenkins\workspace\Quartz.NET\src\Quartz\Impl\AdoJobStore\JobStoreSupport.cs:line 3498
   at Quartz.Impl.AdoJobStore.JobStoreTX.ExecuteInLock[T](String lockName, Func`2 txCallback) in c:\Program Files (x86)\Jenkins\workspace\Quartz.NET\src\Quartz\Impl\AdoJobStore\JobStoreTX.cs:line 76
   at Quartz.Impl.AdoJobStore.JobStoreSupport.StoreJobAndTrigger(IJobDetail newJob, IOperableTrigger newTrigger) in c:\Program Files (x86)\Jenkins\workspace\Quartz.NET\src\Quartz\Impl\AdoJobStore\JobStoreSupport.cs:line 853
   at Quartz.Core.QuartzScheduler.ScheduleJob(IJobDetail jobDetail, ITrigger trigger) in c:\Program Files (x86)\Jenkins\workspace\Quartz.NET\src\Quartz\Core\QuartzScheduler.cs:line 718
   at Quartz.Impl.StdScheduler.ScheduleJob(IJobDetail jobDetail, ITrigger trigger) in c:\Program Files (x86)\Jenkins\workspace\Quartz.NET\src\Quartz\Impl\StdScheduler.cs:line 262
   at QuartzJobRunner.MyService.ConfigureJob() in c:\users\user\documents\visual studio 2015\Projects\QuartzJobRunner\QuartzJobRunner\Program.cs:line 111
   at QuartzJobRunner.MyService.SettingsCache_SettingsCacheFileChanged(Object sender, EventArgs e) in c:\users\user\documents\visual studio 2015\Projects\QuartzJobRunner\QuartzJobRunner\Program.cs:line 117
   at QuartzJobRunner.SettingsCache.NotifyEvent(Object sender) in c:\users\user\documents\visual studio 2015\Projects\QuartzJobRunner\QuartzJobRunner\Program.cs:line 152
   at QuartzJobRunner.SettingsCache.<>c.<.cctor>b__6_0(Object s, FileSystemEventArgs e) in c:\users\user\documents\visual studio 2015\Projects\QuartzJobRunner\QuartzJobRunner\Program.cs:line 159
   at System.IO.FileSystemWatcher.OnChanged(FileSystemEventArgs e)
   at System.IO.FileSystemWatcher.NotifyFileSystemEventArgs(Int32 action, String name)
   at System.IO.FileSystemWatcher.CompletionStatusChanged(UInt32 errorCode, UInt32 numBytes, NativeOverlapped* overlappedPointer)
   at System.Threading._IOCompletionCallback.PerformIOCompletionCallback(UInt32 errorCode, UInt32 numBytes, NativeOverlapped* pOVERLAP) [See nested exception: Quartz.ObjectAlreadyExistsException: Unable to store Job: 'DEFAULT.DummyJob', because one already exists with this identification.
   at Quartz.Impl.AdoJobStore.JobStoreSupport.StoreJob(ConnectionAndTransactionHolder conn, IJobDetail newJob, Boolean replaceExisting) in c:\Program Files (x86)\Jenkins\workspace\Quartz.NET\src\Quartz\Impl\AdoJobStore\JobStoreSupport.cs:line 913]
   ```
