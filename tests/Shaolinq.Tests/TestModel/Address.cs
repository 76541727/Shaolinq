// Copyright (c) 2007-2014 Thong Nguyen (tumtumtum@gmail.com)

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shaolinq.Tests.TestModel
{
	[DataAccessObject]
	public abstract class Address
		: DataAccessObject<long>
	{
		[PersistedMember]
		public abstract int Number { get; set; }

		[PersistedMember]
		public abstract string Street{ get; set; }

		[PersistedMember]
		public abstract string PostalCode { get; set; }

		[PersistedMember]
		public abstract string State { get; set; }

		[PersistedMember]
		public abstract string Country { get; set; }
	}
}
