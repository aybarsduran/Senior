using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    public static class ItemUtility
    {
		public static int ChangeStack(this IItem item, int amount)
		{
			int prevStack = item.StackCount;
			item.StackCount += amount;

			return Mathf.Abs(prevStack - item.StackCount);
		}

		/// <summary>
		/// Use this if you are sure the item has this property.
		/// </summary>
		public static IItemProperty[] GetAllPropertiesWithId(this IItem item, int id)
		{
			var matchedProperties = new List<IItemProperty>();
			var properties = item.Properties;

			foreach (var prop in properties)
			{
				if (prop.Id == id)
					matchedProperties.Add(prop);
			}

			return matchedProperties.ToArray();
		}

		/// <summary>
		/// Use this if you are sure the item has this property.
		/// </summary>
		public static IItemProperty[] GetAllPropertiesWithName(this IItem item, string name)
		{
			var matchedProperties = new List<IItemProperty>();
			var properties = item.Properties;

			foreach (var prop in properties)
			{
				if (prop.Name == name)
					matchedProperties.Add(prop);
			}

			return matchedProperties.ToArray();
		}
		
		/// <summary>
		/// Returns true if the item has a property with the given id.
		/// </summary>
		public static bool HasPropertyWithId(this IItem item, int id)
		{
			var properties = item.Properties;

			foreach (var prop in properties)
			{
				if (prop.Id == id)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Returns true if the item has a property with the given name.
		/// </summary>
		public static bool HasPropertyWithName(this IItem item, string name)
		{
			var properties = item.Properties;
			
			foreach (var prop in properties)
			{
				if (prop.Name == name)
					return true;
			}

			return false;
		}
		
		/// <summary>
		/// Use this if you are sure the item has this property.
		/// </summary>
		public static IItemProperty GetPropertyWithId(this IItem item, int id)
		{
			var properties = item.Properties;
			
			foreach (var prop in properties)
			{
				if (prop.Id != id) 
					continue;
				
				return prop;
			}

			return null;
		}

		/// <summary>
		/// Use this if you are sure the item has this property.
		/// </summary>
		public static IItemProperty GetPropertyWithName(this IItem item, string name)
		{
			var properties = item.Properties;
			
			foreach (var prop in properties)
			{
				if (prop.Name != name)
					continue;
				
				return prop;
			}

			return null;
		}

		/// <summary>
		/// Use this if you are NOT sure the item has this property.
		/// </summary>
		public static bool TryGetPropertyWithId(this IItem item, int id, out IItemProperty itemProperty)
		{
			var properties = item.Properties;

			foreach (var prop in properties)
			{
				if (prop.Id != id)
					continue;
				
				itemProperty = prop;
				return true;
			}

			itemProperty = null;
			return false;
		}

		/// <summary>
		/// Use this if you are NOT sure the item has this property.
		/// </summary>
		public static bool TryGetPropertyWithName(this IItem item, string name, out IItemProperty itemProperty)
		{
			var properties = item.Properties;

			foreach (var prop in properties)
			{
				if (prop.Name != name)
					continue;
				
				itemProperty = prop;
				return true;
			}

			itemProperty = null;
			return false;
		}
	}
}