using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService
{
	/// <summary>
	/// Solicitud para borrar un registro
	/// </summary>
	public class RequestDelete
	{
		/// <summary>
		/// Id del item a borrar
		/// </summary>
		public int Id { get; set; }
	}
}
