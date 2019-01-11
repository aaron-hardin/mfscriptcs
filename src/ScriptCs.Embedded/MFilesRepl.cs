using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScriptCs.Contracts;

namespace ScriptCs.Embedded
{
	public class MFilesRepl : Repl
	{
		public MFilesRepl( string[] scriptArgs, IFileSystem fileSystem, IScriptEngine scriptEngine, IObjectSerializer serializer, ILogProvider logProvider, IScriptLibraryComposer composer, IConsole console, IFilePreProcessor filePreProcessor, IEnumerable<IReplCommand> replCommands, Printers printers, IScriptInfo scriptInfo )
			: base( scriptArgs, fileSystem, scriptEngine, serializer, logProvider, composer, console, filePreProcessor, replCommands, printers, scriptInfo )
		{
		}

		public override void Reset()
		{
			base.Reset();
			Initialize();
		}

		public void Initialize()
		{
			var references = new[] { "Interop.MFilesAPI.dll", "MFiles.VAF.dll", "MFiles.VAF.Configuration.dll" };
			AddReferences( references.Select( reference => Path.Combine( FileSystem.HostBin, reference ) ).ToArray() );
			ImportNamespaces( "MFiles.VAF.Common", "MFiles.VAF.Configuration", "MFilesAPI", "ScriptCs.Embedded.Extensions" );
		}
	}
}
