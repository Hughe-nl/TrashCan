using System; 				// Attribute / Console
using System.Data;			// DataTable / DataRow / DataColumn
using System.Reflection;	// PropertyInfo 
using System.Collections;	// IEnumerable
using System.IO;			// TextWriter

public class DemoClass {

	/* Create our own custom Attribute */
	public class Column : Attribute {
		
		public string Name {get; set;}
		
		/* This is the preferred Column Name for our DataTableWriter */
		public Column(string Name) {
			this.Name = Name;
		}
	}
	
	public class Person {
	
		/* We use Name here, however we want FullName in the DataTable */
		[Column("FullName")]
		public string Name {get; set;}

		/* Age we think is just fine */
		public int Age {get; set;} 
		
	}

	public class DataTableWriter {
	
		/* A method that Writes a dataTable to the provided TextWriter */
		public static void Write(DataTable dataTable, TextWriter textWriter) {
		
			foreach (DataColumn dc in dataTable.Columns) {
				textWriter.Write(String.Format(" {0,-20} ", dc.ToString()));
			}
			textWriter.WriteLine();
		
			foreach (DataRow dr in dataTable.Rows) {
				foreach (DataColumn dc in dataTable.Columns) {
					textWriter.Write(String.Format(" {0,-20} ", dr[dc].ToString()));
				}
				textWriter.WriteLine();
			}
		
		}
	
		/* The method that turns an IEnumerable to a DataTable */
		public static DataTable ToDataTable(IEnumerable objs) {
		
			DataTable result = new DataTable();
			
			foreach (object obj in objs) {
				
				DataRow dataRow = ToDataRow(obj, result);
					
				result.Rows.Add(dataRow);
			}
			return result;
		}
		
		/* An helper method that turns every object in a DataRow */
		private static DataRow ToDataRow(object obj, DataTable dataTable) {
			
			DataRow result = dataTable.NewRow();
			
			PropertyInfo[] properties = obj.GetType().GetProperties();
				
			foreach (PropertyInfo property in properties) {

				string propertyColumnName = ToColumnName(property);
				object propertyValue = property.GetValue(obj);
			
				/* If Column Name is not present, we do not add this property 
				to the DataTable */
				if (propertyColumnName != "") {
					/* If the ColumnName is not present, add it to the DataTable */
					if (! dataTable.Columns.Contains(propertyColumnName)) {
						dataTable.Columns.Add(propertyColumnName);
					}
			
					result[propertyColumnName] = propertyValue;
				}	
			}
				
			return result;
		}

		/* An helper function that determines what the correct column name is*/
		private static string ToColumnName(PropertyInfo property) {
	
			Attribute[] attrs = Attribute.GetCustomAttributes(property);				
					
			foreach (Attribute attr in attrs) {
				if (attr is Column) {
					Column c = (Column) attr;
					return c.Name;
				}
			}
			return property.Name;
		}
	}
	

	public static void Main() {

		// Create two persons
		Person p1 = new Person() { Name = "Person1", Age = 29 };
		Person p2 = new Person() { Name = "Person2", Age = 35 };
		
		// Put the two persons together in an array
		Person[] persons = { p1, p2 };
		
		// Make a DataTable of the array
		DataTable dataTable = DataTableWriter.ToDataTable(persons);		
		
		// Show DataTable
		DataTableWriter.Write(dataTable, Console.Out);
		
		// Wait for a keystroke
		Console.ReadKey();
	}
	
}