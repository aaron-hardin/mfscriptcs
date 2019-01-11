using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Autofac;
using MFiles.VAF.Common;
using MFiles.VAF.Configuration;
using MFilesAPI;
using Newtonsoft.Json;
using ScriptCs.Contracts;
using ScriptCs.Hosting;

namespace ScriptCs.Embedded
{
	public class ScriptCsLauncher
	{
		private HtmlExecuteReplCommand _command;
		private BufferedConsole _console;

		public void Launch( string applicationPath, dynamic dynVault, dynamic dynListItems )
		{
			if( !Debugger.IsAttached )
				Debugger.Launch();

			// TODO: heard this is bad, but don't have another way to do it currently
			// Need to set the app path because it is used later on to get the bin
			//AppDomain.CurrentDomain.SetData( "APPBASE", Path.Combine( applicationPath, "bin" ) );
			// Sets AppDomain.CurrentDomain.BaseDirectory
			AppDomain.CurrentDomain.SetData( "APPBASE", applicationPath );

			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

			Console.WriteLine( "Launching" );

			this._console = new BufferedConsole();

			try
			{
				var config = Config.Create( ScriptCsArgs.Parse( new string[] { } ) );
				var scriptArgs = new string[] { };
				config.Console = this._console;
				var scriptServicesBuilder = (ScriptServicesBuilder)ScriptServicesBuilderFactory.Create( config, scriptArgs );

				var clientApp = new MFilesClientApplication();
				var clientVault = clientApp.BindToVault( (string)dynVault.Name, IntPtr.Zero, true, true );
				var props = clientVault.PropertyDefOperations.GetPropertyDefs();
				var propsDictionary = new Dictionary<string, MFIdentifier>();
				foreach( PropertyDef propertyDef in props )
				{
					// TODO: what if multiple have same name?
					propsDictionary.Add( propertyDef.Name, new MFIdentifier( propertyDef.GUID ) );
				}
				var globs = new ScriptHostArguments
				{
					Vault = clientVault,
					// TODO: convert this list using clientVault to something useful
					// TODO: move the script dashboard to non popup (that way we can pass the current listing)? what if there are multiple? (home page)
					ListingItems = dynListItems,
					Properties = propsDictionary
				};
				var myFactory = new MFilesScriptHostFactory( globs, this._console );
				scriptServicesBuilder.SetOverride<IScriptHostFactory, MFilesScriptHostFactory>( myFactory );
				scriptServicesBuilder.SetOverride<IRepl, MFilesRepl>();
				var scriptServices = scriptServicesBuilder.Build();

				var runtimeServices = (RuntimeServices)scriptServicesBuilder.RuntimeServices;
				runtimeServices.Container.Resolve<Printers>().AddCustomPrinter<Vault>( vault => vault.Name );
				runtimeServices.Container.Resolve<Printers>().AddCustomPrinter<IVault>( vault => vault.Name );
				runtimeServices.Container.Resolve<Printers>().AddCustomPrinter<VaultClass>( vault => vault.Name );
				runtimeServices.Container.Resolve<Printers>().AddCustomPrinter<MFSearchBuilder>( search => $"Search for {search.Vault.Name} ({search.Conditions.Count})" );

				this._command = new HtmlExecuteReplCommand( config.ScriptName, scriptArgs, scriptServices );
				this._command.Repl.Initialize();
				this._command.Execute();
			}
			catch( Exception e )
			{
				Console.WriteLine( "Failed to set up script services" );
				Console.WriteLine( e );
			}
		}

		public string GetMessages()
		{
			return this._console?.GetMessages();
		}

		public bool Execute( string commandText )
		{
			var status = this._command.ExecuteLine( commandText );
			if( !status )
			{
				this._command.Cleanup();
			}
			return status;
		}

		private static Assembly CurrentDomain_AssemblyResolve( object sender, ResolveEventArgs args )
		{
			switch( args.Name )
			{
				case "Newtonsoft.Json, Version=6.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed":
					return typeof( JsonConvert ).Assembly;
				default:
					return null;
			}
		}
	}
}
