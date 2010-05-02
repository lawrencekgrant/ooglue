
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using NUnit.Framework;
using Mono.Data;
using Mono.Data.SqliteClient;

using ooglue.Access;

namespace ooglue.test
{
	[Table("notes")]
	public class Note
	{
		[Column("id")]
		public int Id {get;set;}
		[Column("note_text")]
		public string Text {get;set;}
		[Column("title")]
		public string Title {get;set;}
	}

	[TestFixture()]
	public class SqliteAccessTests
	{
		
		SqliteAccess _access;
		DataExchange _exchange;
		DataConveyor _conveyor;
		
		[TestFixtureSetUp]
		public void SetUp()
		{
			try
			{
				Console.WriteLine("Starting setup");
				_access = new SqliteAccess();
				_exchange = new DataExchange(_access);
				_conveyor = new DataConveyor(_access);
				//ClearDatabase();
				CreateDatabase();
				Console.WriteLine("Setup Complete");
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
		
		[TestFixtureTearDown]
		public void TearDown()
		{
			ClearDatabase();
			_access = null;
		}
		 
		[Test]
		public void ConnectionStringTest()
		{
			Console.WriteLine("Connection String: {0}", _access.ConnectionString);
			Assert.IsNotEmpty(_access.ConnectionString);
		}
		
		[Test]
		public void NewConnectionTest()
		{
			Assert.IsNotNull(_access.NewConnection);
		}
		
		[Test]
		public void InsertRecordsTest()
		{
			for(int i = 0; i < 5; i++)
			{
				_conveyor.InsertObject<Note>(
					new Note() 
				    {
						Id = i, 
						Text = string.Format("Note Text: {0}", i), 
						Title = string.Format("Note: {0}", i)
					});
				Console.WriteLine("Added note number {0}", i);
			}
		}
		
		[Test]
		public void SelectRecordsTest()
		{
			List<Note> notes = _conveyor.FetchObjectListBySql<Note>("select * from notes");
			Console.WriteLine("Notes Found: {0}", notes.Count);
			Assert.Greater(notes.Count, 0);
		}
		
		[Test]
		public void UpdateRecordsTest()
		{
		}
		
		[Test]
		public void DeleteRecordsTest()
		{
			
		}
		
		private void CreateDatabase()
		{
			using(SqliteConnection connection = (SqliteConnection)_access.NewConnection)
			{
				try
				{
					SqliteCommand command = (SqliteCommand)connection.CreateCommand();
					command.CommandType = System.Data.CommandType.Text;
					connection.Open();
					command.CommandText = "create table notes(id int, note_text varchar(255), title varchar(255))";
					command.ExecuteNonQuery();
					connection.Close();
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
					throw;
				}
				finally
				{
					if(connection != null && connection.State == System.Data.ConnectionState.Open)
						connection.Close();
							
				}
			}
		}
		
		private void ClearDatabase()
		{
			using(SqliteConnection connection = (SqliteConnection)_access.NewConnection)
			{
				try
				{
					SqliteCommand command = (SqliteCommand)connection.CreateCommand();
					command.CommandType = System.Data.CommandType.Text;
					connection.Open();
					command.CommandText = "drop table notes";
					command.ExecuteNonQuery();
					connection.Close();
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
					throw;
				}
				finally
				{
					if(connection != null && connection.State == System.Data.ConnectionState.Open)
						connection.Close();
							
				}
			}			
		}
	}
}
