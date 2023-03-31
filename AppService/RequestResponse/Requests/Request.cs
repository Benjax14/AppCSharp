using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService
{
	/// <summary>
	/// Solicitud General al Servicio
	/// </summary>
	public class Request
	{
		/// <summary>
		/// Nombre de la entidad que debe atender la solicitud
		/// </summary>
		public string EntityName {get;set;}
		
		/// <summary>
		/// Nombre del metodo dentro de la entidad que debe atender la solicitud
		/// </summary>
		public string MethodName {get;set;}

		/// <summary>
		/// JSON del request especifico, si es un Get/Save/Delete u otro
		/// </summary>
		public string InnerRequestJson {get;set;}
	}

}
