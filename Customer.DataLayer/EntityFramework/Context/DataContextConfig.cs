using Customer.Api.DataLayer.EntityFramework.Entities;
using Customer.Api.DataLayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Customer.Api.DataLayer.EntityFramework.Context
{
    public class DataContextConfig
    {
        #region Internal Members
        /// <summary>
        /// Application Service
        /// </summary>
        IBaseAppSettingsUtil _appSettingsUtil;
        #endregion

        #region Protected Members
        /// <summary>
        /// Name
        /// </summary>
        protected string _name;

        /// <summary>
        /// Connection String
        /// </summary>
        protected string _connectionString;

        /// <summary>
        /// Force Default Connection
        /// </summary>
        public bool _forceDefaultConnection;

        /// <summary>
        /// Entities
        /// </summary>
        protected Dictionary<Type, Type> _entities = new Dictionary<Type, Type>();

        /// <summary>
        /// Entity Assemblies
        /// </summary>
        protected List<Assembly> _entityAssemblies = new List<Assembly>();
        #endregion

        #region Private Properties
        /// <summary>
        /// Content Info
        /// </summary>
        private static Dictionary<string, DataContextConfig> _contexts = new Dictionary<string, DataContextConfig>();
        #endregion

        #region Public Properties
        /// <summary>
        /// Entities
        /// </summary>
        public Dictionary<Type, Type> Entities
        {
            get
            {
                return _entities;
            }
            set
            {
                _entities = value;
            }
        }

        /// <summary>
        /// Context Config
        /// </summary>
        public static Dictionary<String, DataContextConfig> Contexts
        {
            get
            {
                return _contexts;
            }
            set
            {
                _contexts = value;
            }
        }

        /// <summary>
        /// Entity Assemblies
        /// </summary>
        public List<Assembly> EntityAssemblies
        {
            get
            {
                return _entityAssemblies;
            }
            set
            {
                _entityAssemblies = value;
            }
        }
        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        /// <summary>
        /// Connection string
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionName">Connection Name</param>
        /// <param name="entityAssemblies">Entity Assemblies</param>
        /// <param name="appSettingsUtil">Application Service</param>
        public DataContextConfig(string connectionName, string[] entityAssemblies, IBaseAppSettingsUtil appSettingsUtil, string connectionString = null, bool forceDefaultConnection = false)
        {
            _name = connectionName;
            _appSettingsUtil = appSettingsUtil;
            _connectionString = connectionString ?? _appSettingsUtil.GetConnectionStringDefaultDb();
            _forceDefaultConnection = forceDefaultConnection;

            if (this.EntityAssemblies.Any(a => entityAssemblies.Contains(a.GetName().Name)) == false)
            {
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName()).ToList();
                foreach (var name in loadedAssemblies)
                {
                    if (entityAssemblies.Contains(name.Name))
                    {
                        this.EntityAssemblies.Add(Assembly.Load(name));
                    }
                }
            }

            GetEntities();
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Configure Model
        /// </summary>
        private void GetEntities()
        {
            foreach (AssemblyName name in this.EntityAssemblies.Select(a => a.GetName()).Distinct())
            {
                RegisterEntityTypesFromAssembly(name);
            }
        }

        /// <summary>
        /// Register Entity Types
        /// </summary>
        /// <param name="a">Assembly</param>
        protected void RegisterEntityTypesFromAssembly(AssemblyName a)
        {
            try
            {
                var allTypes = Assembly.Load(a).GetTypes(); 
                var configEntityTypes = allTypes.Where(x => IsEntityTypeConfiguration(x.GetInterfaces()));

                var entities = new Dictionary<Type, Type>();
                foreach (var configType in configEntityTypes) 
                { 
                    var implements = configType.GetInterfaces(); 
                    var implement = implements.Where(x => x.Name.Contains("IEntityTypeConfiguration")).First(); 
                    var entityType = implement.GenericTypeArguments[0]; 
                    if (entityType.GetCustomAttributes(typeof(DataContextAttribute)).Any(t => ((DataContextAttribute)t).DataContextName == _name || (((DataContextAttribute)t).DataContextName == "DefaultConnection" && _forceDefaultConnection)) 
                        || ((_name == "Default" || _name == "DefaultConnection") 
                            && entityType.GetCustomAttributes(typeof(DataContextAttribute)).ToList().Count == 0)) 
                    { 
                        entities.Add(entityType, configType); 
                    } 
                }

                IEnumerable<Type> entityTypes = Assembly.Load(a)
                    .GetTypes()
                    .Where(x => x.IsSubclassOf(typeof(BaseEntity)) && !x.IsAbstract)
                    .Where(x =>
                    {

                        var ca = x.GetCustomAttribute(typeof(DataContextAttribute));
                        var res = x.GetCustomAttributes(typeof(DataContextAttribute)).Any(t => ((DataContextAttribute)t).DataContextName == _name || (((DataContextAttribute)t).DataContextName == "DefaultConnection" && _forceDefaultConnection)) || ((_name == "Default" || _name == "DefaultConnection") && x.GetCustomAttributes(typeof(DataContextAttribute)).ToList().Count == 0);

                        return res;
                    })
                    .Where(x => entities.ContainsKey(x) == false);

                this.Entities = this.Entities.Union(entities).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                foreach (var entity in entityTypes)
                {
                    this.Entities.Add(entity, null);
                }
            }
            catch (System.Reflection.ReflectionTypeLoadException e)
            {
                throw new Exception(String.Format("Entity types will not be loaded from assembly {0} as it could not be found. {1}", a.Name, e.Message));
            }
        }

        /// <summary>
        /// Helper to See if Class is EntityTypeConfiguration
        /// </summary>
        /// <param name="implements"></param>
        /// <returns></returns>
        protected bool IsEntityTypeConfiguration(Type[] implements)
        {
            foreach (var type in implements)
            {
                if (type.ToString().Contains("IEntityTypeConfiguration"))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
