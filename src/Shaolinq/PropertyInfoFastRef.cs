// Copyright (c) 2007-2014 Thong Nguyen (tumtumtum@gmail.com)

﻿using System.Reflection;

namespace Shaolinq
{
	public static class PropertyInfoFastRef
	{
		public static readonly PropertyInfo DataAccessObjectInternalIsNewProperty = typeof(IDataAccessObject).GetProperty("IsNew", BindingFlags.Public | BindingFlags.Instance);
		public static readonly PropertyInfo DataAccessObjectInternalIsMissingAnyAutoIncrementIntegerPrimaryKeyValues = typeof(IDataAccessObject).GetProperty("IsMissingAnyAutoIncrementIntegerPrimaryKeyValues", BindingFlags.Public | BindingFlags.Instance);
		public static readonly PropertyInfo ObjectPropertyValueValueProperty = typeof(ObjectPropertyValue).GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
	}
}
