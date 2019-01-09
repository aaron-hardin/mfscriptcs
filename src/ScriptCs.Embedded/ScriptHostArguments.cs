using System.Collections.Generic;
using MFiles.VAF.Configuration;
using MFilesAPI;

namespace ScriptCs.Embedded
{
	public class ScriptHostArguments
	{
		public dynamic ListingItems { get; set; }
		public Vault Vault { get; set; }
		public Dictionary<string, MFIdentifier> Properties { get; set; }
	}
}