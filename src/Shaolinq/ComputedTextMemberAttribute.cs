﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Shaolinq.Persistence;

namespace Shaolinq
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ComputedTextMemberAttribute
		: Attribute
	{
		internal static readonly Regex FormatRegex = new Regex(@"\{([^\}]+)\}", RegexOptions.Compiled);

		public string Format
		{
			get;
			set;
		}

		public ComputedTextMemberAttribute()
			: this("")
		{
		}

		public ComputedTextMemberAttribute(string format)
		{
			this.Format = format;
		}

		public IEnumerable<string> GetPropertyReferences()
		{
			var matches = FormatRegex.Matches(this.Format);

			return from Match match in matches select match.Groups[1].Value;
		}

		public IEnumerable<PropertyDescriptor> GetPropertyReferencesAsPropertyDescriptors(TypeDescriptor parentType)
		{
			var matches = FormatRegex.Matches(this.Format);

			return from Match match in matches select parentType.GetPropertyDescriptorByPropertyName(match.Groups[1].Value);
		}
	}
}
