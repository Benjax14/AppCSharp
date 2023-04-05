using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService
{
	/// <summary>
	/// Filtro para un RequestGet
	/// </summary>
	public class RequestFilter
	{
		/// <summary>
		/// Propiedad por la que quieren filtrar
		/// </summary>
		public string PropertyName {get;set;}

		/// <summary>
		/// Comparador para el Value
		/// </summary>
		public Comparer Comparer {get;set;}

		/// <summary>
		/// Valor en formato string.
		/// Si la propiedad es de otro tipo, el string debe poder hacer un cast valido
		/// </summary>
		public string Value {get;set;}
	}
}
