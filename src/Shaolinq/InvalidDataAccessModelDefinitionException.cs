﻿// Copyright (c) 2007-2018 Thong Nguyen (tumtumtum@gmail.com)

using System;

namespace Shaolinq
{
	public class InvalidDataAccessModelDefinitionException
		: Exception
	{
		public InvalidDataAccessModelDefinitionException()
		{
		}

		public InvalidDataAccessModelDefinitionException(string message)
			: base(message)
		{
		}

		public InvalidDataAccessModelDefinitionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
