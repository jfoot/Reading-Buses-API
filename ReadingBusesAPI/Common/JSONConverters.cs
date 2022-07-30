// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using ReadingBusesAPI.BusServices;

namespace ReadingBusesAPI.Common
{

	/// <summary>
	///     Converts a string short code for an Operator into an Operator Enum and back again for the JSON converter.
	/// </summary>
	internal class ParseServiceObjects : JsonConverter<List<BusService>>
	{
		/// <summary>
		/// The service ID key word.
		/// </summary>
		private static readonly string ServiceId = "ServiceID";
		/// <summary>
		/// The Service operator key word.
		/// </summary>
		private static readonly string ServiceOperator = "ServiceOperator";


		public override List<BusService> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			List<BusService> services = new List<BusService>();
			bool iDNext = true;
			string serviceID = "";
			string busOperator = "";

			while (reader.Read())
			{
				switch (reader.TokenType)
				{
					case JsonTokenType.StartObject:
						break;
					case JsonTokenType.EndObject:
						if(!ReadingBuses.GetInstance().IsService(serviceID, ReadingBuses.GetOperatorE(busOperator)))
						{
							ReadingBuses.PrintWarning("Service not found - " + serviceID + " " + busOperator);
							break;
						}
						services.Add(ReadingBuses.GetInstance().GetService(serviceID, ReadingBuses.GetOperatorE(busOperator)));
						break;
					case JsonTokenType.String:
						if (iDNext)
						{
							serviceID = reader.GetString();
						}
						else
						{
							busOperator = reader.GetString();
						}
						break;
					case JsonTokenType.PropertyName:
						iDNext = reader.GetString().Equals(ServiceId);
						break;
					case JsonTokenType.EndArray:
						return services;
				}
			}
			return services;
		}

		/// <summary>
		/// Converts a Company Enum into a string value.
		/// </summary>
		/// <param name="value">Company Enum value</param>
		/// <param name="writer">Writes it to a JSON writer.</param>
		private void EnumToString(Company value, Utf8JsonWriter writer)
		{
			switch (value)
			{
				case Company.ReadingBuses:
					writer.WriteString(ServiceOperator, "RBUS");
					return;
				case Company.ThamesValley:
					writer.WriteString(ServiceOperator, "CTNY");
					return;
				case Company.NewburyAndDistrict:
					writer.WriteString(ServiceOperator, "NADS");
					return;
				default:
					writer.WriteString(ServiceOperator, "OTH");
					return;
			}
		}

		public override void Write(Utf8JsonWriter writer, List<BusService> value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();

			foreach (var service in value)
			{
				if (service == null)
					continue;

				writer.WriteStartObject();
				writer.WriteString(ServiceId, service.ServiceId);
				EnumToString(service.Company, writer);
				writer.WriteEndObject();
			}

			writer.WriteEndArray();
		}
	}



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
		}
	}

	/// <summary>
	///		Converts a datetime value between its JSON representation and the object.
	/// </summary>
	public class DateTimeOffsetConverter : JsonConverter<DateTime>
	{
		public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return DateTime.Parse(reader.GetString());
		}

		public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString(CultureInfo.CurrentCulture));
		}
	}

	/// <summary>
	///		Converts a datetime value between its JSON representation and the object.
	/// </summary>
	public class ParseBoolConverter : JsonConverter<bool>
	{
		public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return reader.GetInt32() == 1;
		}

		public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
		{
			writer.WriteNumberValue(value ? 1 : 0);
		}
	}



	/// <summary>
	///     Converts a string short code for an Operator into an Operator Enum and back again for the JSON converter.
	/// </summary>
	internal class ParseOperatorTimetableConverter : JsonConverter<Company>
	{
		public override Company Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			switch (reader.GetString())
			{
				case "Reading Buses":
					return Company.ReadingBuses;
				case "Newbury & District":
					return Company.NewburyAndDistrict;
				case "Thames Valley Buses":
					return Company.ThamesValley;
				default:
					return Company.Other;
			}
		}

		public override void Write(Utf8JsonWriter writer, Company value, JsonSerializerOptions options)
		{
			switch (value)
			{
				case Company.ReadingBuses:
					writer.WriteStringValue("Reading Buses");
					return;
				case Company.ThamesValley:
					writer.WriteStringValue("Thames Valley Buses");
					return;
				case Company.NewburyAndDistrict:
					writer.WriteStringValue("Newbury & District");
					return;
				default:
					writer.WriteStringValue("OTH");
					return;
			}
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
