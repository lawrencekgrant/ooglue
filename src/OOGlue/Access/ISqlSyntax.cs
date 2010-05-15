
using System;

namespace ooglue.Access
{
	public interface ISqlSyntax
	{
		string SelectTemplate {get;}
		string UpdateTemplate {get;}
		string DeleteTemplate {get;}
		string InsertTemplate {get;}
		string CreateTableTemplate {get;}
	}
}
