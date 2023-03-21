using Microsoft.Data.SqlClient;
using Customer.Api.DataLayer.EntityFramework.Context.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Customer.Api.DataLayer.EntityFramework.Context
{
    public class SQLRequest
    {
        #region Private Strings
        /// <summary>
        /// SQL Command Internal
        /// </summary>
        private string _sqlCommand = "";

        /// <summary>
        /// SQL Command
        /// </summary>
        public string SqlCommand
        {
            get
            {
                if (String.IsNullOrEmpty(_sqlCommand) == true)
                {
                    _sqlCommand = GetBuildCommand();
                }
                return _sqlCommand;
            }
            set
            {
                _sqlCommand = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public SQLRequest()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get Parameter List using reflection
        /// </summary>
        /// <returns>List of SQL Parameters</returns>
        public virtual object[] GetParameters()
        {
            List<object> parms = new List<object>();

            foreach (PropertyInfo f in this.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                string parameterName = "";
                string propertyName = f.Name;

                foreach (object fi in f.GetCustomAttributes(false))
                {
                    if (fi is ParameterNameAttribute)
                    {
                        parameterName = ((ParameterNameAttribute)fi).Name;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(parameterName) == false)
                {

                    object val = f.GetValue(this);
                    Type pType = f.PropertyType;

                    if (val == null)
                    {
                        parms.Add(new SqlParameter(parameterName, DBNull.Value));
                    }
                    else if (Nullable.GetUnderlyingType(pType) != null)
                    {
                        PropertyInfo[] props = pType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        PropertyInfo hasValueProp = null;
                        PropertyInfo valueProp = null;

                        foreach (var p in props)
                        {
                            if (p.Name.ToLower() == "hasvalue")
                            {
                                hasValueProp = p;
                            }
                            else if (p.Name.ToLower() == "value")
                            {
                                valueProp = p;
                            }
                        }
                        if (hasValueProp != null && valueProp != null)
                        {
                            bool hasValue = (bool)hasValueProp.GetValue(val);
                            if (hasValue == false)
                            {
                                parms.Add(new SqlParameter(parameterName, DBNull.Value));
                            }
                            else
                            {
                                object nullableVal = valueProp.GetValue(val);

                                parms.Add(new SqlParameter(parameterName, nullableVal));
                            }
                        }

                    }
                    else
                    {
                        parms.Add(new SqlParameter(parameterName, val));
                    }
                }
            }

            return parms.ToArray();
        }

        /// <summary>
        /// Get Data Context Name for the current Request
        /// </summary>
        /// <returns></returns>
        public virtual string GetContextName()
        {
            string name = "DefaultConnection";

            var attrib = this.GetType().GetCustomAttribute(typeof(DataContextAttribute));

            if (attrib != null)
            {
                return ((DataContextAttribute)attrib).DataContextName;
            }

            return name;
        }

        /// <summary>
        /// Get the Stored Procedure Name for the current Request
        /// </summary>
        /// <returns></returns>
        public virtual string GetProcedureName()
        {
            string name = "";

            var attrib = this.GetType().GetCustomAttribute(typeof(ProcedureNameAttribute));

            if (attrib != null)
            {
                return ((ProcedureNameAttribute)attrib).Name;
            }

            return name;
        }

        /// <summary>
        /// Build the SQL Command Text from Parameter Annotation and Property Annotations
        /// </summary>
        /// <returns></returns>
        public virtual string GetBuildCommand()
        {
            string procedureName = GetProcedureName();

            // throw an exception if the stored procedure name annotation is not defined
            if (string.IsNullOrEmpty(procedureName) == true)
            {
                throw new Exception("Undefined procedure name for " + this.GetType().ToString());
            }

            string parameters = "";

            foreach (PropertyInfo f in this.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                string parameterName = "";
                string propertyName = f.Name;

                foreach (object fi in f.GetCustomAttributes(false))
                {
                    if (fi is ParameterNameAttribute)
                    {
                        parameterName = ((ParameterNameAttribute)fi).Name;
                        if (string.IsNullOrEmpty(parameterName) == false)
                        {
                            if (string.IsNullOrEmpty(parameters) == false)
                            {
                                parameters += ", ";
                            }
                            parameters += parameterName;
                            break;
                        }
                    }
                }
            }

            return procedureName + " " + parameters;
        }
        #endregion
    }
}
