using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService
{
	/// <summary>
	/// Solicitud para un Get
	/// </summary>
	public class RequestGet
	{
		/// <summary>
		/// Propiedad por la que se deben ordenar los registros resultantes (OrderBy)
		/// </summary>
		public string OrderProperty {get;set;}
		
		/// <summary>
		/// Dirección del órden de los registros (OrderBy)
		/// </summary>
		public OrderDirection OrderDirection {get;set;}

		/// <summary>
		/// Indice de pagina. Si es null es porque no estan usando paginas
		/// </summary>
		public int? PageIndex { get; set; }

		/// <summary>
		/// Cantidad de items por pagina. Si es null es porque no estan usando paginas
		/// </summary>
		public int? PageSize { get; set; }

		/// <summary>
		/// Filtros para filtrar la lista de registros resultantes
		/// </summary>
		public List<RequestFilter> Filters { get; set; }

		public RequestGet()
		{
			Filters = new List<RequestFilter>();
		}

	}
}
