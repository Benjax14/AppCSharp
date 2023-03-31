using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService
{
	/// <summary>
	/// Respuesta del servicio a un solicitud
	/// </summary>
	public class Response
	{
		/// <summary>
		/// Items que forman la respuesta
		/// Por ejemplo si es un Get el diccionario tendra un "Records" y asociada una lista de los objetos solicitados.
		/// Para un Save o un Delete puede tener un "Result" y asociado un "Ok"
		/// Tambien podemos agregar un "Error" y un mensaje de error.
		/// </summary>
		public Dictionary<string, object> Items { get; set; }

		public Response()
		{
			Items  = new Dictionary<string, object>();
			
		}
	}
}
