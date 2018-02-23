using StoreRepository.Infrastructure;
using StoreRepository.Repositories;
using StoreService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Services
{
    public class GenericService<TEntity> : IGenericService<TEntity> where TEntity : class
    {
        /// <summary>
        /// The Repository
        /// </summary>
        private IRepository repository;

        /// <summary>
        /// Default constructor
        /// </summary>
        public GenericService()
        {
            this.repository = new GenericRepository();

            // Sets TimeStamped flag to true if the entity is timestamped.
            this.SetEntityProperties();

            // Ignore deleted records by default
            this.IgnoreDeletedRecords = true;
        }

        /// <summary>
        /// Constructor with user defined connectionString.
        /// </summary>
        /// <param name="connectionString">The connectionString</param>
        public GenericService(string connectionString)
        {
            this.repository = new GenericRepository(connectionString);

            // Sets TimeStamped flag to true if the entity is timestamped.
            this.SetEntityProperties();

            // Ignore deleted records by default
            this.IgnoreDeletedRecords = true;
        }

        /// <summary>
        /// Constructor with user defined context.
        /// </summary>
        /// <param name="context">User defined context.</param>
        public GenericService(DbContext context)
        {
            this.repository = new GenericRepository(context);

            // Sets TimeStamped flag to true if the entity is timestamped.
            this.SetEntityProperties();

            // Ignore deleted records by default
            this.IgnoreDeletedRecords = true;
        }

        /// <summary>
        /// Gets or sets whether the operations should be done in batch mode.
        /// </summary>
        public bool BatchMode { get; set; }

        public string ModuleName { get; set; }
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets whether deleted records should be ignored during an operation.
        /// Usable only if Timestamped is true.
        /// </summary>
        public bool IgnoreDeletedRecords { get; set; }

        /// <summary>
        /// Gets or sets whether the entity is timestamped. i.e. the entity in database has fields like DateCreated, DateModified and DateDeleted.
        /// Assumption made: An entity will always have these 3 date fields or have none of them.
        /// </summary>
        private bool TimeStamped { get; set; }

        /// <summary>
        /// Gets or sets whether the entity is uniquely identified. i.e. the entity in database has field UniqueId.
        /// </summary>
        private bool UniqueIdentified { get; set; }

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <returns>IQueryable of type TEntity</returns>
        public IQueryable<TEntity> GetQuery()
        {
            return this.repository.GetQuery<TEntity>();
        }

        /// <summary>
        /// Gets the query based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <returns>IQueryable of type TEntity</returns>
        public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> criteria)
        {
            criteria = this.FilterDeletedRecords(criteria);
            return this.repository.GetQuery<TEntity>(criteria);
        }

        /// <summary>
        /// Loads entity by id(key).
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="id">The id(key) value.</param>
        /// <returns>Class object of type TEntity</returns>
        public TEntity LoadById(object id)
        {
            return this.repository.LoadById<TEntity>(id);
        }

        /// <summary>
        /// Loads entity by unique identifier(GUID).
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="uniqueId">The unique identifier(GUID) value.</param>
        /// <returns>Class object of type TEntity</returns>
        public TEntity LoadByUniqueId(Guid uniqueId)
        {
            return this.repository.LoadByUniqueId<TEntity>(uniqueId);
        }

        /// <summary>
        /// Loads entity by GUID.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="value">The GUID value.</param>
        /// <returns>Class object of type TEntity</returns>
        public TEntity LoadByGuid(Guid value)
        {
            return this.repository.LoadByGuid<TEntity>(value);
        }

        /// <summary>
        /// Loads entity of with GUID value of given column name.
        /// </summary>
        /// <param name="columnName">The uniquely identified column name.</param>
        /// <param name="value">The GUID value of the unique column.</param>
        /// <returns>Object of type TEntity</returns>
        public TEntity LoadByUniqueIdentifier(string columnName, Guid value)
        {
            return this.repository.LoadByUniqueIdentifier<TEntity>(columnName, value);
        }

        /// <summary>
        /// Loads one entity based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <returns>Object of type TEntity</returns>
        public TEntity LoadSingle(Expression<Func<TEntity, bool>> criteria)
        {
            criteria = this.FilterDeletedRecords(criteria);
            return this.repository.LoadSingle<TEntity>(criteria);
        }

        /// <summary>
        /// Loads the first entity based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <returns>Class object of type TEntity</returns>
        public TEntity LoadFirst(Expression<Func<TEntity, bool>> criteria)
        {
            criteria = this.FilterDeletedRecords(criteria);
            return this.repository.LoadFirst<TEntity>(criteria);
        }

        /// <summary>
        /// Loads all entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <returns>Collection of entities.</returns>
        public IEnumerable<TEntity> LoadAll()
        {
            Expression<Func<TEntity, bool>> deletedFilter = this.FilterDeletedRecords(null);
            return (deletedFilter == null) ? this.repository.LoadAll<TEntity>() : this.repository.Load(deletedFilter);
        }

        /// <summary>
        /// Loads one or more entities based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <returns>IEnumerable of type TEntity</returns>
        public IEnumerable<TEntity> Load(Expression<Func<TEntity, bool>> criteria)
        {
            criteria = this.FilterDeletedRecords(criteria);
            return this.repository.Load<TEntity>(criteria);
        }

        /// <summary>
        /// Loads one or more entities based on matching criteria.
        /// </summary>
        /// <typeparam name="TKey">Generic type argument</typeparam>
        /// <param name="orderBy">Order by condition</param>
        /// <returns>IEnumerable of class TEntity</returns>
        public IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, TKey>> orderBy)
        {
            return this.repository.Load(orderBy);
        }

        /////// <summary>
        /////// Loads one or more entities based on matching criteria.
        /////// </summary>
        /////// <typeparam name="TEntity">The type of entity.</typeparam>
        /////// <param name="orderBy">Order by condition.</param>
        /////// <param name="sortOrder">Sort by condition. Ascending by default.</param>
        /////// <returns>IEnumerable of class TEntity</returns>
        ////public IEnumerable<TEntity> Load(Expression<Func<TEntity, string>> orderBy, SortOrder sortOrder)
        ////{
        ////    return this.repository.Load<TEntity>(orderBy, sortOrder);
        ////}

        /// <summary>
        /// Loads one or more entities with paging support.
        /// </summary>
        /// <typeparam name="TKey">Generic type argument</typeparam>
        /// <param name="orderBy">Order by condition</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>IEnumerable of type TEntity</returns>
        public IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize)
        {
            return this.repository.Load(orderBy, pageIndex, pageSize);
        }

        /// <summary>
        /// Loads one or more entities with paging support.
        /// </summary>
        /// <typeparam name="TKey">Generic type argument</typeparam>
        /// <param name="orderBy">Order by condition</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortOrder">Sort by condition</param>
        /// <returns>IEnumerable of type TEntity</returns>
        public IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder)
        {
            return this.repository.Load(orderBy, pageIndex, pageSize, sortOrder);
        }

        /// <summary>
        /// Loads one or more entities based on the matching criteria with order by.
        /// </summary>
        /// <typeparam name="TKey">Generic type argument</typeparam>
        /// <param name="criteria">The matching criteria</param>
        /// <param name="orderBy">Order by condition</param>
        /// <returns>IEnumerable of type TEntity</returns>
        public IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy)
        {
            criteria = this.FilterDeletedRecords(criteria);
            return this.repository.Load(criteria, orderBy);
        }

        /// <summary>
        /// Loads one or more entities based on the matching criteria with order by and sort order.
        /// </summary>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="sortOrder">Sort by condition.</param>
        /// <returns>IEnumerable of type TEntity</returns>
        public IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, SortOrder sortOrder)
        {
            criteria = this.FilterDeletedRecords(criteria);
            return this.repository.Load(criteria, orderBy, sortOrder);
        }

        /// <summary>
        /// Loads one or more entities based on the matching criteria with paging support.
        /// </summary>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>IEnumerable of type TEntity</returns>
        public IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize)
        {
            criteria = this.FilterDeletedRecords(criteria);
            return this.repository.Load(criteria, orderBy, pageIndex, pageSize, SortOrder.Ascending);
        }

        /// <summary>
        /// Loads one or more entities based on the matching criteria with paging support.
        /// </summary>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="sortOrder">Sort by condition. Ascending by default.</param>
        /// <returns>IEnumerable of type TEntity</returns>
        public IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder)
        {
            criteria = this.FilterDeletedRecords(criteria);
            return this.repository.Load(criteria, orderBy, pageIndex, pageSize, sortOrder);
        }

        /// <summary>
        /// Exeutes the specified store procedure
        /// </summary>
        /// <typeparam name="T">User defined class</typeparam>
        /// <param name="name">Name of store procedure</param>
        /// <param name="parameters">Stored procedure parameters</param>
        /// <returns>Collection of entities</returns>
        public IEnumerable<T> ExecuteStoreProcedure<T>(string name, Dictionary<string, string> parameters) where T : class
        {
            return this.repository.ExecuteStoreProcedure<T>(name, parameters);
        }

        /// <summary>
        /// Exeutes the specified store procedure
        /// </summary>
        /// <typeparam name="T">User defined class</typeparam>
        /// <param name="name">Name of store procedure</param>
        /// <returns>Collection of entities</returns>
        public IEnumerable<T> ExecuteStoreProcedure<T>(string name) where T : class
        {
            return this.repository.ExecuteStoreProcedure<T>(name);
        }

        /// <summary>
        /// Executes the specified sql query.
        /// </summary>
        /// <typeparam name="T">User defined class</typeparam>
        /// <param name="query">Sql query.</param>
        /// <returns>IEnumerable of class T.</returns>
        public IEnumerable<T> LoadByQuery<T>(string query) where T : class
        {
            return this.repository.ExecuteSqlQuery<T>(query);
        }

        /// <summary>
        /// Function that returns datatable
        /// </summary>
        /// <param name="query">pass the sql query</param>
        /// <returns>data table</returns>
        public DataTable GetDataTable(string query)
        {
            return this.repository.GetDataTable(query);
        }

        /// <summary>
        /// Function that executes the scalar sent
        /// </summary>
        /// <param name="query">pass the sql query</param>
        /// <returns>executed value obtained in the form of string</returns>
        public string ExecuteScalar(string query)
        {
            return this.repository.ExecuteScalar(query);
        }

        /// <summary>
        /// Function that executes the non query sent
        /// </summary>
        /// <param name="query">pass the sql query</param>
        /// <returns>executed value obtained in the form of int</returns>
        public int ExecuteNonQuery(string query)
        {
            return this.repository.ExecuteNonQuery(query);
        }

        /////// <summary>
        /////// Executes the specified sql query
        /////// </summary>
        /////// <param name="query">Sql query</param>
        /////// <returns>Boolean value indicating if the given sql query executed successfully</returns>
        ////public bool ExecuteNonQuery(string query)
        ////{
        ////    DateTimeHolder obj = new DateTimeHolder();
        ////    query = query.Trim();
        ////    if (!query.Substring(query.Length - 1, 1).Equals(";"))
        ////    {
        ////        query += ";";
        ////    }

        ////    query += "SELECT GETDATE() AS CurrentDate";
        ////    obj = this.repository.ExecuteSqlQuery<DateTimeHolder>(query).SingleOrDefault();

        ////    //// This will create a new instance of repository so we'll not end up with cache values
        ////    this.repository = new GenericRepository();
        ////    return obj != null;
        ////}

        /// <summary>
        /// Counts the number of entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <returns>Count of rows</returns>
        public int Count()
        {
            Expression<Func<TEntity, bool>> deletedFilter = this.FilterDeletedRecords(null);
            return (deletedFilter == null) ? this.repository.Count<TEntity>() : this.repository.Count(deletedFilter);
        }

        /// <summary>
        /// Counts the number of entities matching the specified criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The mathicng criteria.</param>
        /// <returns>Count of rows</returns>
        public int Count(Expression<Func<TEntity, bool>> criteria)
        {
            criteria = this.FilterDeletedRecords(criteria);
            return this.repository.Count<TEntity>(criteria);
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="entity">The entity to add.</param>
        public void Add(TEntity entity)
        {
            Type entityType = typeof(TEntity);

            if (this.TimeStamped)
            {
                PropertyInfo dateCreated = entityType.GetProperty("DateCreated");

                if (dateCreated != null)
                {
                    DateTime created = DateTime.Parse(dateCreated.GetValue(entity, null).ToString());

                    if (created == null || created == DateTime.MinValue)
                    {
                        dateCreated.SetValue(entity, DateTime.Now, null);
                    }
                }
            }

            if (this.UniqueIdentified)
            {
                PropertyInfo uniqueId = entityType.GetProperty("UniqueId");
                if (uniqueId == null)
                {
                    uniqueId = entityType.GetProperty("GUID");
                }

                if (uniqueId != null)
                {
                    uniqueId.SetValue(entity, Guid.NewGuid(), null);
                }
            }

            this.repository.Add<TEntity>(entity);

            if (this.BatchMode == false)
            {
                if (!string.IsNullOrEmpty(this.UserId) && !string.IsNullOrEmpty(this.ModuleName))
                {
                    this.repository.SaveChanges(ModuleName, UserId);
                }
                else
                {
                    this.repository.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Updates changes in an existing entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="entity">The entity to update.</param>
        public void Update(TEntity entity)
        {
            ////Type entityType = typeof(TEntity);

            if (this.TimeStamped)
            {
                ////PropertyInfo dateModified = entityType.GetProperty("DateModified");

                ////if (dateModified != null)
                ////{
                ////    if (dateModified.GetValue(entity, null) == null)
                ////    {
                ////        dateModified.SetValue(entity, DateTime.Now, null);
                ////    }
                ////}
            }

            this.repository.Update<TEntity>(entity);

            if (this.BatchMode == false)
            {
                if (!string.IsNullOrEmpty(this.UserId) && !string.IsNullOrEmpty(this.ModuleName))
                {
                    this.repository.SaveChanges(ModuleName, UserId);
                }
                else
                {
                    this.repository.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="entity">The entity to delete.</param>
        public void Delete(TEntity entity)
        {
            if (this.TimeStamped)
            {
                Type entityType = typeof(TEntity);

                PropertyInfo dateDeleted = entityType.GetProperty("DateDeleted");

                if (dateDeleted != null)
                {
                    dateDeleted.SetValue(entity, DateTime.Now, null);
                    this.repository.Update<TEntity>(entity);
                    if (this.BatchMode == false)
                    {
                        if (!string.IsNullOrEmpty(this.UserId) && !string.IsNullOrEmpty(this.ModuleName))
                        {
                            this.repository.SaveChanges(ModuleName, UserId);
                        }
                        else
                        {
                            this.repository.SaveChanges();
                        }
                    }

                    return;
                }
            }

            this.repository.Delete<TEntity>(entity);
            if (this.BatchMode == false)
            {
                this.repository.SaveChanges();
            }
        }

        /// <summary>
        /// Method to delete the entities lying under the criteria
        /// </summary>
        /// <param name="criteria">Criteria for deletion of entities</param>
        public void Delete(Expression<Func<TEntity, bool>> criteria)
        {
            IEnumerable<TEntity> entities = this.repository.Load<TEntity>(criteria);

            foreach (TEntity entity in entities)
            {
                this.Delete(entity);
            }
        }

        /// <summary>
        /// Main method to call SaveChanges of data context
        /// </summary>
        public void SaveChanges()
        {
            this.repository.SaveChanges();
        }

        /// <summary>
        /// Overloadable SaveChanges() method
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="userId"></param>
        public void SaveChanges(string moduleName, string userId)
        {
            this.repository.SaveChanges(moduleName, userId);
        }

        /// <summary>
        /// Refresh the context
        /// </summary>
        /// <param name="entity">The entity to refresh</param>
        public void RefreshContext(TEntity entity)
        {
            this.repository.RefreshContext<TEntity>(entity);
        }

        /// <summary>
        /// Method to return current date time of database server
        /// </summary>
        /// <returns>Returns datetime object</returns>
        public DateTime GetDateTime()
        {
            DateTimeHolder obj = new DateTimeHolder();
            obj = this.repository.ExecuteSqlQuery<DateTimeHolder>("SELECT GETDATE() AS CurrentDate").SingleOrDefault();
            return obj.CurrentDate;
        }

        #region Private Methods

        /// <summary>
        /// Sets the additional properties of the entity based on availability of fields in database.
        /// </summary>
        private void SetEntityProperties()
        {
            Type entityType = typeof(TEntity);

            // Using DateDeleted field to decide whether the property is timestamped.
            PropertyInfo dateDeleted = entityType.GetProperty("DateDeleted");
            this.TimeStamped = (dateDeleted == null) ? false : true;

            // Using UniqueId field to decide whether the property is uniquely identified.
            PropertyInfo uniqueId = entityType.GetProperty("UniqueId");
            this.UniqueIdentified = (uniqueId == null) ? false : true;

            if (!this.UniqueIdentified)
            {
                uniqueId = entityType.GetProperty("GUID");
                this.UniqueIdentified = (uniqueId == null) ? false : true;
            }
        }

        /// <summary>
        /// Gets a predicate to filter on deleted records.
        /// Applicable only for timestamped entities.
        /// </summary>
        /// <param name="criteria">Other criteria</param>
        /// <returns>Expression criteria</returns>
        private Expression<Func<TEntity, bool>> FilterDeletedRecords(Expression<Func<TEntity, bool>> criteria)
        {
            if (this.TimeStamped && this.IgnoreDeletedRecords)
            {
                Expression<Func<TEntity, bool>> predicate = this.MakeFilter("DateDeleted", null);
                criteria = (criteria == null) ? predicate : null;//predicate.And(criteria);
            }

            return criteria;
        }

        /// <summary>
        /// Creates an expression with given parameters.
        /// </summary>
        /// <typeparam name="TEntity">Expression</typeparam>
        /// <param name="prop">Property name</param>
        /// <param name="value">property value</param>
        /// <returns>Expression criteria</returns>
        private Expression<Func<TEntity, bool>> MakeFilter(string prop, object value)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TEntity), "p");
            PropertyInfo pi = typeof(TEntity).GetProperty(prop);
            MemberExpression me = Expression.MakeMemberAccess(pe, pi);
            ConstantExpression ce = Expression.Constant(value);
            BinaryExpression be = Expression.Equal(me, ce);

            return Expression.Lambda<Func<TEntity, bool>>(be, pe);
        }

        #endregion

        /// <summary>
        /// Helper class to retrieve date time
        /// </summary>
        internal class DateTimeHolder
        {
            /// <summary>
            /// Default constructor
            /// </summary>
            public DateTimeHolder()
            {
            }

            /// <summary>
            /// Declare a property of DateTime class
            /// </summary>
            public DateTime CurrentDate { get; set; }
        }
    }
}
