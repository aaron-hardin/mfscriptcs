using System;
using ScriptCs.Command;
using ScriptCs.Contracts;

namespace ScriptCs.Embedded
{
	public class HtmlExecuteReplCommand
	{
		private readonly string _scriptName;
		private readonly string[] _scriptArgs;
		private readonly IFileSystem _fileSystem;
		private readonly IScriptPackResolver _scriptPackResolver;
		private readonly IRepl _repl;
		private readonly ILog _logger;
		private readonly IConsole _console;
		private readonly IAssemblyResolver _assemblyResolver;
		private readonly IFileSystemMigrator _fileSystemMigrator;
		private readonly IScriptLibraryComposer _composer;

		public Repl Repl => (Repl) this._repl;

		public HtmlExecuteReplCommand( string scriptName, string[] scriptArgs, ScriptServices scriptServices )
			: this( scriptName,
				scriptArgs,
				scriptServices.FileSystem,
				scriptServices.ScriptPackResolver,
				scriptServices.Repl,
				scriptServices.LogProvider,
				scriptServices.Console,
				scriptServices.AssemblyResolver,
				scriptServices.FileSystemMigrator,
				scriptServices.ScriptLibraryComposer )
		{
		}

		public HtmlExecuteReplCommand(
			string scriptName,
			string[] scriptArgs,
			IFileSystem fileSystem,
			IScriptPackResolver scriptPackResolver,
			IRepl repl,
			ILogProvider logProvider,
			IConsole console,
			IAssemblyResolver assemblyResolver,
			IFileSystemMigrator fileSystemMigrator,
			IScriptLibraryComposer composer )
		{
			this._scriptName = scriptName;
			this._scriptArgs = scriptArgs;
			this._fileSystem = fileSystem;
			this._scriptPackResolver = scriptPackResolver;
			this._repl = repl;
			this._logger = logProvider.ForCurrentType();
			this._console = console;
			this._assemblyResolver = assemblyResolver;
			this._fileSystemMigrator = fileSystemMigrator;
			this._composer = composer;
		}

		public string[] ScriptArgs => this._scriptArgs;

		public CommandResult Execute()
		{
			this._fileSystemMigrator.Migrate();

			this._console.WriteLine( "scriptcs (ctrl-c to exit or :help for help)" + Environment.NewLine );

			var workingDirectory = this._fileSystem.CurrentDirectory;
			var assemblies = this._assemblyResolver.GetAssemblyPaths( workingDirectory );
			var scriptPacks = this._scriptPackResolver.GetPacks();

			this._composer.Compose( workingDirectory );

			this._repl.Initialize( assemblies, scriptPacks, ScriptArgs );

			if( !string.IsNullOrWhiteSpace( this._scriptName ) )
			{
				this._logger.InfoFormat( "Executing script '{0}'", this._scriptName );
				try
				{
					this._repl.Execute( string.Format( "#load {0}", this._scriptName ) );
				}
				catch( Exception ex )
				{
					this._logger.ErrorException( "Error executing script '{0}'", ex, this._scriptName );
					return CommandResult.Error;
				}
			}

			return CommandResult.Success;
		}

		public void Cleanup()
		{
			this._repl.Terminate();
		}

		public bool ExecuteLine( string commandText )
		{
			// TODO: this was here to help determine if we are continuing or enter a new command, would like to show something to user
			var prompt = string.IsNullOrWhiteSpace( this._repl.Buffer ) ? "> " : "* ";

			try
			{
				var line = commandText; //this._console.ReadLine( prompt );

				if( line == null )
					return false;

				if( !string.IsNullOrWhiteSpace( line ) )
				{
					this._repl.Execute( line );
				}

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}