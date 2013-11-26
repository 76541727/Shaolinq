// Copyright (c) 2007-2013 Thong Nguyen (tumtumtum@gmail.com)

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shaolinq.Tests.DataAccessModel.Test
{
	[DataAccessObject]
	public abstract class Fraternity
		: DataAccessObject<string>
	{
		[RelatedDataAccessObjects]
		public abstract RelatedDataAccessObjects<Student> Students { get; }
	}
}
