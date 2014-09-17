﻿using System;
using Platform.Validation;

namespace Shaolinq.Tests.TestModel
{
	[DataAccessObject]
	public abstract class Dog
		: DataAccessObject<long>
	{
		[Index(SortOrder = SortOrder.Ascending)]
		[PersistedMember]
		public abstract string Name { get; set; }

		[PersistedMember]
		public abstract Cat CompanionCat { get; set; }
	}
}
