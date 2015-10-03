	[System.ComponentModel.RunInstaller(true)]
	public class ProjectInstaller : System.Configuration.Install.Installer
	{
		public static readonly string SERVICENAME = //#NTS_NAME;

		private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller serviceInstaller;

		public ProjectInstaller()
		{
			serviceInstaller = new System.ServiceProcess.ServiceInstaller();
			serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();

			serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			serviceProcessInstaller.Username = //#NTS_USER;
			serviceProcessInstaller.Password = //#NTS_PASS;

			serviceInstaller.DisplayName = //#NTS_DISP_NAME;
			serviceInstaller.ServiceName = ProjectInstaller.SERVICENAME;
			serviceInstaller.StartType = //#NTS_START_TYPE;

			Installers.AddRange(new System.Configuration.Install.Installer[] { serviceProcessInstaller, serviceInstaller });
		}

		public override string HelpText
		{
		   get
		   {
		      return //#NTS_HELP;
		   }
		}

	}//EOC

	public class NetzService : System.ServiceProcess.ServiceBase
	{
		protected System.Threading.Thread serviceThread;
		protected System.Threading.ManualResetEvent resetEvent;
		protected string[] args;

		public NetzService(string[] args)
		{
			this.args = args;
			resetEvent = new System.Threading.ManualResetEvent(false);
			ServiceName = ProjectInstaller.SERVICENAME;
			Init();
		}

		private void Init()
		{
			CanPauseAndContinue = false;
			CanHandlePowerEvent = false;
			CanShutdown = true;
			CanStop = true;
			AutoLog = false;
			//CanHandleSessionChangeEvent = false;
		}

		protected override void OnStart(string[] args)
		{
			if(serviceThread == null)
			{
				serviceThread = new System.Threading.Thread(new System.Threading.ThreadStart(ServiceThread));
				serviceThread.IsBackground = true;
				serviceThread.Start();
			}
		}

		protected override void OnStop()
		{
			if(serviceThread != null)
			{
				serviceThread.Abort();
			}
			//ExitCode = 0;
		}

		protected override void OnShutdown()
		{
			base.OnShutdown();
		}

		protected void ServiceThread()
		{
			try
			{
				NetzStarter.StartApp(args);
				try
				{
					// Run until the service is stopped, which raises the ThreadAbortException.
					resetEvent.WaitOne();
				}
				catch (System.Threading.ThreadAbortException)
				{
					// Log thread exited
				}
			}
			catch(Exception ex)
			{
				Log("Failed: " + ex.Message + " @ " + ex.StackTrace);
			}
			finally
			{
				serviceThread = null;
			}
		}

		private void Log(string msg)
		{
			if(msg == null) msg = string.Empty;
			try
			{
				if(!System.Diagnostics.EventLog.SourceExists(ServiceName))
				{
					System.Diagnostics.EventLog.CreateEventSource(ServiceName, "NetzServiceLog");
				}
				System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog();
				eventLog.Source = ServiceName;
				eventLog.WriteEntry(msg);
				eventLog = null;
			}
			catch
			{
				System.Diagnostics.Debug.WriteLine("Netz: " + ServiceName + " Error: " + msg);
			}
		}

	}//EOC
