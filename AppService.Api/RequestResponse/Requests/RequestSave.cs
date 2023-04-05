using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService
{
	/// <summary>
	/// Solicitud para guardar un registro.
	/// Sea nuevo o modificación de uno existente
	/// </summary>
	public class RequestSave
	{
		/// <summary>
		/// JSON del item a guardar
		/// </summary>
		public string ItemJson {get;set;}

		

	}
}
