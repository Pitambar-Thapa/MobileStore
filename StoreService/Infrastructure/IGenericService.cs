using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Infrastructure
{
    /// <summary>
    /// Interface for Service layer
    /// </summary>
    /// <typeparam name="TEntity">Class of type TEntity</typeparam>
    public interface IGenericService<TEntity> where TEntity : class
    {
        /// <summary>
        /// Boolean to identify if ignore deleted records
        /// </summary>
        bool IgnoreDeletedRecords { get; set; }

        /// <summary>
        /// Batch mode
        /// </summary>
        bool BatchMode { get; set; }

        /// <summary>
        /// Method to return Query of given object
        /// </summary>
        /// <returns>IQueryable object of type TEntity</returns>
        IQueryable<TEntity> GetQuery();

        /// <summary>
        /// Method to return Query of given object
        /// </summary>
        /// <param name="criteria">Expression for where condtion</param>
        /// <returns>IQueryable object of type TEntity</returns>
        IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> criteria);

        /// <summary>
        /// Load the single record by key column
        /// </summary>
        /// <param name="id">Value of primary key column</param>
        /// <returns>Object of type TEntity</returns>
        TEntity LoadById(object id);

        /// <summary>
        /// Load the single record on GUID data type
        /// </summary>
        /// <param name="uniqueId">GUID value</param>
        /// <returns>Object of type TEntity</returns>
        TEntity LoadByUniqueId(Guid uniqueId);

        /// <summary>
        /// Load the single record on GUID data type
        /// </summary>
        /// <param name="value">GUID value</param>
        /// <returns>Object of type TEntity</returns>
        TEntity LoadByGuid(Guid value);

        /// <summary>
        /// Loads entity of with GUID value of given column name.
        /// </summary>
        /// <param name="columnName">The uniquely identified column name.</param>
        /// <param name="value">The GUID value of the unique column.</param>
        /// <returns>Object of type TEntity</returns>
        TEntity LoadByUniqueIdentifier(string columnName, Guid value);

        /// <summary>
        /// Load the single record on given criteria
        /// </summary>
        /// <param name="criteria">Expression criteria</param>
        /// <returns>Object of type TEntity</returns>
        TEntity LoadSingle(Expression<Func<TEntity, bool>> criteria);

        /// <summary>
        /// Load the first record on given criteria
        /// </summary>
        /// <param name="criteria">Expression criteria</param>
        /// <returns>Object of type TEntity</returns>
        TEntity LoadFirst(Expression<Func<TEntity, bool>> criteria);

        /// <summary>
        /// Load all records
        /// </summary>
        /// <returns>IEnumerable of type TEntity</returns>
        IEnumerable<TEntity> LoadAll();

        /// <summary>
        /// Load records on given parameters
        /// </summary>
        /// <param name="criteria">Expression for order by field</param>
        /// <returns>IEnumerable of type TEntity</returns>
        IEnumerable<TEntity> Load(Expression<Func<TEntity, bool>> criteria);

        /// <summary>
        /// Loads one or more entities based on matching criteria.
        /// </summary>
        /// <typeparam name="TKey">The type of key</typeparam>
        /// <param name="orderBy">Order by condition</param>
        /// <returns>IEnumerable of class TEntity</returns>
        IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, TKey>> orderBy);

        /////// <summary>
        /////// Loads one or more entities based on matching criteria.
        /////// </summary>
        /////// <typeparam name="TEntity">The type of entity.</typeparam>
        /////// <param name="orderBy">Order by condition.</param>
        /////// <param name="sortOrder">Sort by condition. Ascending by default.</param>
        /////// <returns>IEnumerable of class TEntity</returns>
        ////IEnumerable<TEntity> Load(Expression<Func<TEntity, string>> orderBy, SortOrder sortOrder);

        /// <summary>
        /// Load records on given parameters
        /// </summary>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="orderBy">Expression for order by field</param>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>IEnumerable of type TEntity</returns>
        IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize);

        /// <summary>
        /// Load records on given parameters
        /// </summary>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="orderBy">Expression for order by field</param>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortOrder">SortOrder enum value</param>
        /// <returns>IEnumerable of type TEntity</returns>
        IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder);

        /// <summary>
        /// Load records on given parameters
        /// </summary>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">Expression where criteria</param>
        /// <param name="orderBy">Expression for order by field</param>
        /// <returns>IEnumerable of type TEntity</returns>
        IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy);

        /// <summary>
        /// Load records on given parameters
        /// </summary>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">Expression where criteria</param>
        /// <param name="orderBy">Expression for order by field</param>
        /// <param name="sortOrder">SortOrder enum value</param>
        /// <returns>IEnumerable of type TEntity</returns>
        IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, SortOrder sortOrder);

        /// <summary>
        /// Load records on given parameters
        /// </summary>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">Expression where criteria</param>
        /// <param name="orderBy">Expression for order by field</param>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>IEnumerable of type TEntity</returns>
        IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize);

        /// <summary>
        /// Load records on given parameters
        /// </summary>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">Expression where criteria</param>
        /// <param name="orderBy">Expression for order by field</param>
        /// <param name="pageIndex">Current page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortOrder">SortOrder enum value</param>
        /// <returns>IEnumerable of type TEntity</returns>
        IEnumerable<TEntity> Load<TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder);

        /// <summary>
        /// Exeutes the specified store procedure
        /// </summary>
        /// <typeparam name="T">User defined class</typeparam>
        /// <param name="name">Name of store procedure</param>
        /// <param name="parameters">Stored procedure parameters</param>
        /// <returns>Collection of entities</returns>
        IEnumerable<T> ExecuteStoreProcedure<T>(string name, Dictionary<string, string> parameters) where T : class;

        /// <summary>
        /// Exeutes the specified store procedure
        /// </summary>
        /// <typeparam name="T">User defined class</typeparam>
        /// <param name="name">Name of store procedure</param>
        /// <returns>Collection of entities</returns>
        IEnumerable<T> ExecuteStoreProcedure<T>(string name) where T : class;

        /// <summary>
        /// Executes the specified sql query.
        /// </summary>
        /// <typeparam name="T">User defined class</typeparam>
        /// <param name="query">Sql query.</param>
        /// <returns>IEnumerable of class T.</returns>
        IEnumerable<T> LoadByQuery<T>(string query) where T : class;

        /// <summary>
        /// Function that returns datatable
        /// </summary>
        /// <param name="query">pass the sql query</param>
        /// <returns>data table</returns>
        DataTable GetDataTable(string query);

        /// <summary>
        /// Function that executes the scalar sent
        /// </summary>
        /// <param name="query">pass the sql query</param>
        /// <returns>executed value obtained in the form of string</returns>
        string ExecuteScalar(string query);

        /// <summary>
        /// Function that executes the non query sent
        /// </summary>
        /// <param name="query">pass the sql query</param>
        /// <returns>executed value obtained in the form of int</returns>
        int ExecuteNonQuery(string query);

        /////// <summary>
        /////// Executes the specified sql query
        /////// </summary>
        /////// <param name="query">Sql query</param>
        /////// <returns>Boolean value indicating if the given sql query executed successfully</returns>
        ////bool ExecuteNonQuery(string query);

        /// <summary>
        /// Method to find count of given Entity (TEntity)
        /// </summary>
        /// <returns>Number of rows</returns>
        int Count();

        /// <summary>
        /// Method to find count on given criteria
        /// </summary>
        /// <param name="criteria">Expression criteria</param>
        /// <returns>Number of rows</returns>
        int Count(Expression<Func<TEntity, bool>> criteria);

        /// <summary>
        /// Method to add given entity
        /// </summary>
        /// <param name="entity">Object of type TEntity</param>
        void Add(TEntity entity);

        /// <summary>
        /// Method to update give entity
        /// </summary>
        /// <param name="entity">Object of type TEntity</param>
        void Update(TEntity entity);

        /// <summary>
        /// Method to delete given entity
        /// </summary>
        /// <param name="entity">Object of type TEntity</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Method to delete the entities lying under the criteria
        /// </summary>
        /// <param name="criteria">Criteria for deletion of entities</param>
        void Delete(Expression<Func<TEntity, bool>> criteria);

        /// <summary>
        /// Main method to call SaveChanges() of Entity framework
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Refresh the context
        /// </summary>
        /// <param name="entity">The entity to refresh</param>
        void RefreshContext(TEntity entity);

        /// <summary>
        /// Method to return current date time of database server
        /// </summary>
        /// <returns>Returns datetime object</returns>
        DateTime GetDateTime();
    }
}
