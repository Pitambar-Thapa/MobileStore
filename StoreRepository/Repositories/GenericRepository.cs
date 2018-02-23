using StoreRepository.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoreRepository.Repositories
{
    /// <summary>
    /// Generic repository class of type IRepository
    /// </summary>
    public class GenericRepository : IRepository
    {
        /// <summary>
        /// The context.
        /// </summary>
        private DbContext context;

        /// <summary>
        /// Unit of work.
        /// </summary>
        private IUnitOfWork unitOfWork;

        /// <summary>
        /// private property connection of type SqlConnection
        /// </summary>
        private SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository"/> class.
        /// </summary>
        public GenericRepository()
        {
            try
            {
                foreach (System.Configuration.ConnectionStringSettings con in System.Configuration.ConfigurationManager.ConnectionStrings)
                {
                    if (con.Name.ToLower(CultureInfo.InvariantCulture).Contains("entities"))
                    {
                        this.context = new DbContext(con.ConnectionString);
                        break;
                    }
                }

                if (this.context == null)
                {
                    throw new EntityException("Context not initialized");
                }
            }
            catch
            {
                throw new EntityException("Context not initialized");
            }
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="connectionString">The connectionString.</param>
        public GenericRepository(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString", "Connection string not correct");
            }

            this.context = new DbContext(connectionString);
            this.SqlConnectionString = this.context.Database.Connection.ConnectionString;
            ////this.context.Dispose();
            this.connection = new SqlConnection(this.SqlConnectionString);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public GenericRepository(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "Context not initialized");
            }
            this.context = context;
        }

        /// <summary>
        /// Gets the unit of work.
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (this.unitOfWork == null)
                {
                    this.unitOfWork = new UnitOfWork(this.DBContext);
                }

                return this.unitOfWork;
            }
        }

        /// <summary>
        /// private property KeepConnectionOpen of type bool
        /// </summary>
        public bool KeepConnectionOpen { get; set; }

        /// <summary>
        /// private property sqlconnectionstring of type string
        /// </summary>
        private string SqlConnectionString { get; set; }

        /// <summary>
        /// property to establish connection
        /// </summary>
        private SqlConnection Connection
        {
            get
            {
                if (this.connection.State != ConnectionState.Open)
                {
                    this.connection.Open();
                }

                return this.connection;
            }
        }

        /// <summary>
        /// Pluralization service to pluralize entity names.
        /// </summary>
        ////private readonly PluralizationService pluralizer = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en"));      

        /// <summary>
        /// Gets the context.
        /// </summary>        
        private DbContext DBContext
        {
            get
            {
                return this.context;
            }
        }

        /// <summary>
        /// The type of entity
        /// </summary>
        /// <typeparam name="TEntity">class of type TEntity</typeparam>
        /// <returns>IQueryable of type TEntity</returns>
        public IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class
        {
            // Gets the entity name
            var entityName = this.GetEntityName<TEntity>();

            // Returns the object qyery
            return ((IObjectContextAdapter)this.DBContext).ObjectContext.CreateQuery<TEntity>(entityName);
        }

        /// <summary>
        /// Gets the query based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <returns>IQueryable of class TEntity</returns>
        public IQueryable<TEntity> GetQuery<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class
        {
            return this.GetQuery<TEntity>().Where(criteria);
        }

        /// <summary>
        /// Loads entity by id(key).
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="id">The id(key) value.</param>
        /// <returns>Class of type TEntity</returns>
        public TEntity LoadById<TEntity>(object id) where TEntity : class
        {
            // Gets the primary key field of the entity
            EntityKey key = this.GetEntityId<TEntity>(id);

            // Gets the entity from object context with the key
            object entity;
            if (((IObjectContextAdapter)this.DBContext).ObjectContext.TryGetObjectByKey(key, out entity))
            {
                return (TEntity)entity;
            }

            return default(TEntity);
        }

        /// <summary>
        /// Loads entity by unique identifier(GUID).
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="uniqueId">The unique identifier(GUID) value.</param>
        /// <returns>Class of type TEntity</returns>
        public TEntity LoadByUniqueId<TEntity>(Guid uniqueId) where TEntity : class
        {
            return this.LoadByUniqueIdentifier<TEntity>("UniqueId", uniqueId);
        }

        /// <summary>
        /// Loads entity by GUID.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="value">The GUID value.</param>
        /// <returns>Class of type TEntity</returns>
        public TEntity LoadByGuid<TEntity>(Guid value) where TEntity : class
        {
            return this.LoadByUniqueIdentifier<TEntity>("GUID", value);
        }

        /// <summary>
        /// Loads entity of with GUID value of given column name.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="columnName">The uniquely identified column name.</param>
        /// <param name="value">The GUID value of the unique column.</param>
        /// <returns>Object of type TEntity</returns>
        public TEntity LoadByUniqueIdentifier<TEntity>(string columnName, Guid value) where TEntity : class
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "tEntity");
            MemberExpression memberExpression = Expression.PropertyOrField(parameterExpression, columnName);
            ConstantExpression valueExpression = Expression.Constant(new Guid(value.ToString()), typeof(Guid));
            BinaryExpression binaryExpression = Expression.Equal(memberExpression, valueExpression);
            var criteria = Expression.Lambda<Func<TEntity, bool>>(binaryExpression, parameterExpression);
            return this.LoadSingle(criteria);
        }

        /// <summary>
        /// Loads one entity based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <returns>Class of type TEntity</returns>
        public TEntity LoadSingle<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class
        {
            return this.GetQuery<TEntity>().FirstOrDefault<TEntity>(criteria);
        }

        /// <summary>
        /// Loads the first entity based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <returns>Class of type TEntity</returns>
        public TEntity LoadFirst<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class
        {
            return this.GetQuery<TEntity>().FirstOrDefault(criteria);
        }

        /// <summary>
        /// Loads all entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <returns>IEnumerable of TEntity</returns>
        public IEnumerable<TEntity> LoadAll<TEntity>() where TEntity : class
        {
            return this.GetQuery<TEntity>().AsEnumerable();
        }

        /// <summary>
        /// Loads one or more entities based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <returns>IEnumerable of TEntity</returns>
        public IEnumerable<TEntity> Load<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class
        {
            return this.GetQuery<TEntity>().Where(criteria);
        }

        /// <summary>
        /// Loads one or more entities based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="orderBy">Order by condition.</param>
        /// <returns>IEnumerable of class TEntity</returns>
        public IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, TKey>> orderBy) where TEntity : class
        {
            return this.GetQuery<TEntity>().OrderBy(orderBy).AsEnumerable();
        }

        /// <summary>
        /// Loads one or more entities based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="sortOrder">Sort by condition. Ascending by default.</param>
        /// <returns>IEnumerable of class TEntity</returns>
        public IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, TKey>> orderBy, SortOrder sortOrder) where TEntity : class
        {
            if (sortOrder == SortOrder.Ascending)
            {
                return this.GetQuery<TEntity>().OrderBy(orderBy).AsEnumerable();
            }
            else
            {
                return this.GetQuery<TEntity>().OrderByDescending(orderBy).AsEnumerable();
            }
        }

        /// <summary>
        /// Loads one or more entities with paging support.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>IEnumerable of TEntity</returns>
        public IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize) where TEntity : class
        {
            return this.GetQuery<TEntity>().OrderBy(orderBy).Skip(pageIndex).Take(pageSize).AsEnumerable();
        }

        /// <summary>
        /// Loads one or more entities with paging support.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="sortOrder">Sort by condition. Ascending by default.</param>
        /// <returns>IEnumerable of TEntity</returns>
        public IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder) where TEntity : class
        {
            if (sortOrder == SortOrder.Ascending)
            {
                return this.GetQuery<TEntity>().OrderBy(orderBy).Skip(pageIndex).Take(pageSize).AsEnumerable();
            }
            else
            {
                return this.GetQuery<TEntity>().OrderByDescending(orderBy).Skip(pageIndex).Take(pageSize).AsEnumerable();
            }
        }

        /// <summary>
        /// Loads one or more entities based on the matching criteria with paging support.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>IEnumerable of TEntity</returns>
        public IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize) where TEntity : class
        {
            return this.GetQuery<TEntity>().Where(criteria).OrderBy(orderBy).Skip(pageIndex).Take(pageSize).AsEnumerable();
        }

        /// <summary>
        /// Loads one or more entities based on the matching criteria with paging support.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="sortOrder">Sort by condition. Ascending by default.</param>
        /// <returns>IEnumerable of TEntity</returns>
        public IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder) where TEntity : class
        {
            if (sortOrder == SortOrder.Ascending)
            {
                return this.GetQuery<TEntity>().Where(criteria).OrderBy(orderBy).Skip(pageIndex).Take(pageSize).AsEnumerable();
            }
            else
            {
                return this.GetQuery<TEntity>().Where(criteria).OrderByDescending(orderBy).Skip(pageIndex).Take(pageSize).AsEnumerable();
            }
        }

        /// <summary>
        /// Loads one or more entities based on the matching criteria and order by.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <param name="orderBy">Order by condition.</param>
        /// <returns>IEnumerable of TEntity</returns>
        public IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy) where TEntity : class
        {
            return this.GetQuery<TEntity>().Where(criteria).OrderBy(orderBy).AsEnumerable();
        }

        /// <summary>
        /// Loads one or more entities based on the matching criteria and order by and sort order with default ascending sort.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="sortOrder">Sort by condition. Ascending by default.</param>
        /// <returns>IEnumerable of TEntity</returns>
        public IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, SortOrder sortOrder) where TEntity : class
        {
            if (sortOrder == SortOrder.Ascending)
            {
                return this.GetQuery<TEntity>().Where(criteria).OrderBy(orderBy).AsEnumerable();
            }
            else
            {
                return this.GetQuery<TEntity>().Where(criteria).OrderByDescending(orderBy).AsEnumerable();
            }
        }

        /// <summary>
        /// Exeutes the specified store procedure.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="procName">Name of store procedure.</param>
        /// <param name="parameters">Stored procedure parameters</param>
        /// <returns>IEnumerable of TEntity.</returns>
        public IEnumerable<TEntity> ExecuteStoreProcedure<TEntity>(string procName, Dictionary<string, string> parameters) where TEntity : class
        {
            SqlParameter[] sqlParams = new SqlParameter[parameters.Count];
            string sql = "EXEC " + procName;

            for (int i = 0; i < parameters.Count; i++)
            {
                var item = parameters.ElementAt(i);
                sqlParams[i] = new SqlParameter("@" + item.Key, item.Value);
                sql += " @" + item.Key + ",";
            }

            sql = sql.Remove(sql.Length - 1, 1);
            return this.context.Database.SqlQuery<TEntity>(sql, sqlParams);
        }

        /// <summary>
        /// Exeutes the specified store procedure.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="procName">Name of store procedure.</param>
        /// <returns>IEnumerable of TEntity.</returns>
        public IEnumerable<TEntity> ExecuteStoreProcedure<TEntity>(string procName) where TEntity : class
        {
            string sql = "EXEC " + procName;
            return this.context.Database.SqlQuery<TEntity>(sql);
        }

        /// <summary>
        /// Executes the specified sql query.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="query">Sql query.</param>
        /// <returns>IEnumerable of TEntity.</returns>
        public IEnumerable<TEntity> ExecuteSqlQuery<TEntity>(string query) where TEntity : class
        {
            return this.context.Database.SqlQuery<TEntity>(query);
        }

        /// <summary>
        /// Function that returns datatable
        /// </summary>
        /// <param name="query">pass the sql query</param>
        /// <returns>data table</returns>
        public DataTable GetDataTable(string query)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(query, this.Connection);
            da.Fill(dt);
            da.Dispose();

            if (!this.KeepConnectionOpen)
            {
                this.Connection.Close();
            }

            return dt;
        }

        /// <summary>
        /// Function that executes the scalar sent
        /// </summary>
        /// <param name="query">pass the sql query</param>
        /// <returns>executed value obtained in the form of string</returns>
        public string ExecuteScalar(string query)
        {
            SqlCommand cmd = new SqlCommand(query, this.Connection);
            cmd.CommandTimeout = 300;
            string value = cmd.ExecuteScalar().ToString();
            cmd.Dispose();

            if (!this.KeepConnectionOpen)
            {
                this.Connection.Close();
            }

            return value;
        }

        /// <summary>
        /// Function that executes the non query sent
        /// </summary>
        /// <param name="query">pass the sql query</param>
        /// <returns>executed value obtained in the form of int</returns>
        public int ExecuteNonQuery(string query)
        {
            SqlCommand cmd = new SqlCommand(query, this.Connection);
            cmd.CommandTimeout = 300;
            int value = cmd.ExecuteNonQuery();
            cmd.Dispose();

            if (!this.KeepConnectionOpen)
            {
                this.Connection.Close();
            }

            return value;
        }

        /// <summary>
        /// Counts the number of entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <returns>Total rows count</returns>
        public int Count<TEntity>() where TEntity : class
        {
            return this.GetQuery<TEntity>().Count();
        }

        /// <summary>
        /// Counts the number of entities matching the specified criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The mathicng criteria.</param>
        /// <returns>Total rows count</returns>
        public int Count<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class
        {
            return this.GetQuery<TEntity>().Count(criteria);
        }

        /// <summary>
        /// Adds the specified entity.
        /// Here, the entity is added only to the collection. To save the changes in database, Save() should be called.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="entity">The entity to add.</param>
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.DBContext.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// Updates changes in an existing entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="entity">The entity to update.</param>
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.DBContext.Set<TEntity>().Attach(entity);
            this.DBContext.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes the specified entity.
        /// Here, the entity is delete only from the context. To save the changes in database, Save() should be called.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="entity">The entity to delete.</param>
        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.DBContext.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// Deletes one or more entities matching the specified criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        public void Delete<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class
        {
            IEnumerable<TEntity> entities = this.Load<TEntity>(criteria);

            foreach (TEntity entity in entities)
            {
                this.Delete<TEntity>(entity);
            }
        }

        /// <summary>
        /// Saves changes from the context.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        public void SaveChanges()
        {
            this.DBContext.SaveChanges();
        }

        /// <summary>
        ///  Overloadable SaveChanges() method
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="userId"></param>
        public void SaveChanges(string moduleName, string userId)
        {
            if (((IObjectContextAdapter)this.DBContext).ObjectContext.Connection.State == ConnectionState.Closed)
            {
                ((IObjectContextAdapter)this.DBContext).ObjectContext.Connection.Open();
            }

            ((IObjectContextAdapter)this.DBContext).ObjectContext.ExecuteStoreCommand("exec spSetUserContext @userInfo = {0}", moduleName + (string.IsNullOrEmpty(userId) ? string.Empty : ("/" + userId)));
            ((IObjectContextAdapter)this.DBContext).ObjectContext.SaveChanges();

        }

        /// <summary>
        /// Refresh the context
        /// </summary>
        /// <typeparam name="TEntity">The type of entity</typeparam>
        /// <param name="entity">The entity to refresh</param>
        public void RefreshContext<TEntity>(TEntity entity) where TEntity : class
        {
            var dbContext = ((IObjectContextAdapter)this.DBContext).ObjectContext;

            ObjectStateEntry stateEntry = null;
            bool isPresent = dbContext.ObjectStateManager.TryGetObjectStateEntry(entity, out stateEntry);

            if (isPresent)
            {
                dbContext.Refresh(RefreshMode.StoreWins, entity);
            }
        }

        /// <summary>
        /// Gets the primary key (id) of the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="keyValue">Value of the key.</param>
        /// <returns>Object EntityKey</returns>
        private EntityKey GetEntityId<TEntity>(object keyValue) where TEntity : class
        {
            // Gets the qualified name of the entity
            var entityName = this.GetEntityName<TEntity>();

            // Gets the object context and the first key from the key collection
            var objectSet = ((IObjectContextAdapter)this.DBContext).ObjectContext.CreateObjectSet<TEntity>();
            var keyPropertyName = objectSet.EntitySet.ElementType.KeyMembers[0].ToString();

            // Returns the key
            var entityKey = new EntityKey(entityName, new[] { new EntityKeyMember(keyPropertyName, keyValue) });
            return entityKey;
        }

        /// <summary>
        /// Gets the qualified name of the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <returns>Entity name</returns>
        private string GetEntityName<TEntity>() where TEntity : class
        {
            ////return string.Format("{0}.{1}", ((IObjectContextAdapter)this.DBContext).ObjectContext.DefaultContainerName, pluralizer.Pluralize(typeof(TEntity).Name));
            return string.Format("{0}.{1}", ((IObjectContextAdapter)this.DBContext).ObjectContext.DefaultContainerName, typeof(TEntity).Name);
        }
    }
}
