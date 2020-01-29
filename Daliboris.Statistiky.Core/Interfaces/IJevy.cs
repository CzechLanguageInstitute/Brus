using System.Collections.Generic;
using System.ComponentModel;

namespace Daliboris.Statistiky.Rozhrani.Jevy
{
	public interface IJevy : ICollection<IJev>, IDictionary<string, IJev>, IBindingList, ITypedList 
	{ 
	}

}