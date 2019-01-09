using ScriptCs.Contracts;

namespace ScriptCs.Embedded
{
	internal class MFilesScriptHostFactory : IScriptHostFactory
	{
		private readonly ScriptHostArguments _globs;
		private readonly IConsole _console;
		private MFilesScriptHost _host;

		public MFilesScriptHostFactory( ScriptHostArguments globs, IConsole console )
		{
			this._globs = globs;
			this._console = console;
		}
		public IScriptHost CreateScriptHost( IScriptPackManager scriptPackManager, string[] scriptArgs )
		{
			this._host = new MFilesScriptHost( scriptPackManager, new ScriptEnvironment( scriptArgs, this._console, null ), this._globs );
			return this._host;
		}
	}
}