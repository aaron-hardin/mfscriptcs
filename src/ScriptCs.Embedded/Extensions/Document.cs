using MFiles.VAF.Common;
using MFiles.VAF.Configuration;
using MFilesAPI;

namespace ScriptCs.Embedded.Extensions
{
	public class Document : ObjVerEx
	{
		public Document( Vault vault, int id ) : base( vault, 0, id, -1 )
		{
		}

		public object this[ MFIdentifier key ]
		{
			get => GetPropertyText( key );
			set
			{
				if( !key.IsResolved )
				{
					key.Resolve( Vault, typeof(PropertyDef) );
				}
				var dataType = Vault.PropertyDefOperations.GetPropertyDef( key ).DataType;
				SetProperty( key, dataType, value );
			}
		}
	}
}
