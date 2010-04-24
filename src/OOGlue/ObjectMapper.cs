using System;
using System.Collections.Generic;
using System.Reflection;

namespace ooglue
{
	public class ObjectMapper
	{
		public ObjectMapper ()
		{
		}
		
		//TODO: Implement
		public object MapAnonymous(params object [] inputs)
		{
			throw new NotImplementedException("This method has not yet been implemented, but when it is, it'll rock.");
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="inputs">
		/// A <see cref="System.Object[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Object"/> of the type specified on the method call 
		/// </returns>
		public T Map<T>(params object[] inputs) where T : new()
		{
			return Map<T>(MappingDirection.FirstIn, inputs);
		}
		
		/// <summary>
		/// This method maps any number of objects by <see cref="ColumnAttribute"/>s. An object of the specified type is returned.
		/// </summary>
		/// <param name="direction">
		/// A <see cref="MappingDirection"/> that determines whether the last value or the first value that conflicts should be taken.
		/// <remarks>I need to clarify this some more...</remarks>
		/// </param>
		/// <param name="inputs">
		/// A <see cref="System.Object[]"/> a list of objects to be mapped against the new object.
		/// </param>
		/// <returns>
		/// A type which has had it's values mapped to from the inputs into the method.
		/// </returns>
		public T Map<T>(MappingDirection direction, params object[] inputs) where T : new()
		{
			T returnObject = new T();
			//This parameter map will contain each name, and all values of each parameter. This way, based on the MappingDirection, we can
			//then determine what value must be assigned to our new object.
			Dictionary<string, List<object>> parameterMap = new Dictionary<string, List<object>>();
			
			//we'll traverse our input objects
			foreach(object input in inputs)
			{
				//and gather each property from them
				foreach(PropertyInfo property in input.GetType().GetProperties())
				{
					//we'll capture all attributes that are ooglue.ColumnAttributes so that we can map to our generic object based on those
					//values
					object[] attributes = property.GetCustomAttributes(typeof(ooglue.ColumnAttribute), true);
					//for each attribute, we need to add it to the appropriate section of the parameter map
					foreach(ColumnAttribute currentColumnAttribute in attributes)
					{
						//check if the parameter map contains the item... if not, then we'll add it as well as the initial list with a 
						//single entry.
						if(!(parameterMap.ContainsKey(currentColumnAttribute.Name)))
						{
							parameterMap.Add(currentColumnAttribute.Name, new List<object>() { property.GetValue(input, null) });
						}
						else
						{
							//otherwise, we must have the item, and an instantiated list, so we can simply add to the existing list
							parameterMap[currentColumnAttribute.Name].Add(property.GetValue(input, null));
						}
					}					
				}
			}
			
			//Get properties from the new object
			PropertyInfo [] newObjectProperties = typeof(T).GetProperties();
			foreach(PropertyInfo propertyInfo in newObjectProperties)
			{
				foreach(ColumnAttribute newObjectColumnAttribute in propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true))
				{
					//map each property that has a corresponding entry in the parameter map
					if(parameterMap.ContainsKey(newObjectColumnAttribute.Name))
					{
						//determine whether we should process the first instance of a value that we've found, or the last.
						switch(direction)
						{
						case MappingDirection.FirstIn:
							propertyInfo.SetValue(returnObject, parameterMap[newObjectColumnAttribute.Name][0], null);
							break;
						case MappingDirection.LastIn:
							propertyInfo.SetValue(returnObject, parameterMap[newObjectColumnAttribute.Name][parameterMap[newObjectColumnAttribute.Name].Count - 1], null);
							break;
						}
					}
					                          
				}
			}
			
			return returnObject;
		}
	}
}
