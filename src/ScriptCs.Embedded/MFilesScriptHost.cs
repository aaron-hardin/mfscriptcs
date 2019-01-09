using System.Collections.Generic;
using MFiles.VAF.Configuration;
using MFilesAPI;
using ScriptCs.Contracts;

namespace ScriptCs.Embedded
{
	public class MFilesScriptHost : ScriptHost
	{
		public MFilesScriptHost( IScriptPackManager scriptPackManager, ScriptEnvironment environment, ScriptHostArguments arguments )
			: base( scriptPackManager, environment )
		{
			ListingItems = arguments.ListingItems;
			Vault = arguments.Vault;
			Properties = arguments.Properties;
		}

		public dynamic ListingItems { get; set; }
		public Vault Vault { get; set; }
		public Dictionary<string, MFIdentifier> Properties { get; set; }
	}
}