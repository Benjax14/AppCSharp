﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService
{
	public enum Comparer {
		Equal = 0,
		NotEqual = 1,
		Greater = 2,
		GreaterOrEqual = 3,
		Lower = 4,
		LowerOrEqual = 5,
		Contains = 6
	}
}
