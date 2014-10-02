// Copyright (c) 2007-2014 Thong Nguyen (tumtumtum@gmail.com)

﻿using System;
using System.Reflection;

namespace Shaolinq
{
	public class FieldInfoFastRef
	{
		public static readonly FieldInfo GuidEmptyGuid = typeof(Guid).GetField("Empty", BindingFlags.Public | BindingFlags.Static);
	}
}
