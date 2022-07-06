// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReadingBusesAPI.Common
{
	/// <summary>
	///     Converts a string short code for an Operator into an Operator Enum and back again for the JSON converter.
	/// </summary>
	internal class ParseOperatorConverter : JsonConverter<Company>
	{
		public override Company Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return ReadingBuses.GetOperatorE(reader.GetString());
		}

		public override void Write(Utf8JsonWriter writer, Company value, JsonSerializerOptions options)
		{
			switch (value)
			{
				case Company.ReadingBuses:
					writer.WriteStringValue("RBUS");
					return;
				case Company.ThamesValley:
					writer.WriteStringValue("CTNY");
					return;
				case Company.NewburyAndDistrict:
					writer.WriteStringValue("NADS");
					return;
				default:
					writer.WriteStringValue("OTH");
					return;
			}

			throw new JsonException("Cannot marshal type Operators");
		}
	}


	/// <summary>
	///     Converts a string into a long and back again for the JSON converter.
	/// </summary>
	internal class ParseStringConverter : JsonConverter<long>
	{
		public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.String)
			{
				if (long.TryParse(reader.GetString(), out long number))
					return number;
			}

			throw new JsonException("Cannot unmarshal type long");
		}

		public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString());
		}
	}
}
