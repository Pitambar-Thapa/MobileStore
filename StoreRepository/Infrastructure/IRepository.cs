using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoreRepository.Infrastructure
{
    /// <summary>
    /// IRepository Interface 
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Gets the unit of work.
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <returns>IQueryable of type TEntity</returns>
        IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class;

        /// <summary>
        /// Gets the query based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <returns>IQueryable of type TEntity</returns>
        IQueryable<TEntity> GetQuery<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;

        /// <summary>
        /// Loads entity by id(key).
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="id">The id(key) value.</param>
        /// <returns>Class of type TEntity</returns>
        TEntity LoadById<TEntity>(object id) where TEntity : class;

        /// <summary>
        /// Loads entity by unique identifier(GUID).
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="uniqueId">The unique identifier(GUID) value.</param>
        /// <returns>Class of type TEntity</returns>
        TEntity LoadByUniqueId<TEntity>(Guid uniqueId) where TEntity : class;

        /// <summary>
        /// Loads entity by GUID.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="value">The GUID value.</param>
        /// <returns>Class of type TEntity</returns>
        TEntity LoadByGuid<TEntity>(Guid value) where TEntity : class;

        /// <summary>
        /// Loads entity of with GUID value of given column name.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="columnName">The uniquely identified column name.</param>
        /// <param name="value">The GUID value of the unique column.</param>
        /// <returns>Object of type TEntity</returns>
        TEntity LoadByUniqueIdentifier<TEntity>(string columnName, Guid value) where TEntity : class;

        /// <summary>
        /// Loads one entity based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <returns>Class of type TEntity</returns>
        TEntity LoadSingle<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;

        /// <summary>
        /// Loads the first entity based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <returns>Class of type TEntity</returns>
        TEntity LoadFirst<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;

        /// <summary>
        /// Loads all entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <returns>IEnumerable of type TEntity</returns>
        IEnumerable<TEntity> LoadAll<TEntity>() where TEntity : class;

        /// <summary>
        /// Loads one or more entities based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <returns> IEnumerable of type TEntity</returns>
        IEnumerable<TEntity> Load<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;

        /// <summary>
        /// Loads one or more entities based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="orderBy">Order by condition.</param>
        /// <returns>IEnumerable of class TEntity</returns>
        IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, TKey>> orderBy) where TEntity : class;

        /// <summary>
        /// Loads one or more entities based on matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="sortOrder">Sort by condition. Ascending by default.</param>
        /// <returns>IEnumerable of class TEntity</returns>
        IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, TKey>> orderBy, SortOrder sortOrder) where TEntity : class;

        /// <summary>
        /// Loads one or more entities based on the matching criteria with order by.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <param name="orderBy">Order by condition.</param>
        /// <returns>IEnumerable of class TEntity</returns>
        IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy) where TEntity : class;

        /// <summary>
        /// Loads one or more entities based on the matching criteria with order by.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="sortOrder">Sort by condition. Ascending by default.</param>
        /// <returns>IEnumerable of class TEntity</returns>
        IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, SortOrder sortOrder) where TEntity : class;

        /// <summary>
        /// Loads one or more entities with paging support.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>IEnumerable of class TEntity</returns>
        IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize) where TEntity : class;

        /// <summary>
        /// Loads one or more entities with paging support.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="sortOrder">Sort by condition. Ascending by default.</param>
        /// <returns>IEnumerable of class TEntity</returns>
        IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder) where TEntity : class;

        /// <summary>
        /// Loads one or more entities based on the matching criteria with paging support.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <typeparam name="TKey">Generic type argument.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        /// <param name="orderBy">Order by condition.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>IEnumerable of class TEntity</returns>
        IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize) where TEntity : class;

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
        /// <returns>IEnumerable of class TEntity</returns>
        IEnumerable<TEntity> Load<TEntity, TKey>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TKey>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder) where TEntity : class;

        /// <summary>
        /// Exeutes the specified store procedure.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="procName">Name of store procedure.</param>
        /// <param name="parameters">Stored procedure parameters</param>
        /// <returns>IEnumerable of TEntity.</returns>
        IEnumerable<TEntity> ExecuteStoreProcedure<TEntity>(string procName, Dictionary<string, string> parameters) where TEntity : class;

        /// <summary>
        /// Exeutes the specified store procedure.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="procName">Name of store procedure.</param>
        /// <returns>IEnumerable of TEntity.</returns>
        IEnumerable<TEntity> ExecuteStoreProcedure<TEntity>(string procName) where TEntity : class;

        /// <summary>
        /// Executes the specified sql query.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="query">Sql query.</param>
        /// <returns>IEnumerable of TEntity.</returns>
        IEnumerable<TEntity> ExecuteSqlQuery<TEntity>(string query) where TEntity : class;

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

        /// <summary>
        /// Counts the number of entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <returns>Total rows count</returns>
        int Count<TEntity>() where TEntity : class;

        /// <summary>
        /// Counts the number of entities matching the specified criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The mathicng criteria.</param>
        /// <returns>Total rows count</returns>
        int Count<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="entity">The entity to add.</param>
        void Add<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Updates changes in an existing entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="entity">The entity to update.</param>
        void Update<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="entity">The entity to delete.</param>
        void Delete<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Deletes one or more entities matching the specified criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        /// <param name="criteria">The matching criteria.</param>
        void Delete<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;

        /// <summary>
        /// Saves changes from the context.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity.</typeparam>
        void SaveChanges();

        /// <summary>
        /// Overloadable SaveChanges
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="userId"></param>
        void SaveChanges(string moduleName, string userId);

        /// <summary>
        /// Refresh the entity
        /// </summary>
        /// <typeparam name="TEntity">The type of entity</typeparam>
        /// <param name="entity">The entity to refresh</param>
        void RefreshContext<TEntity>(TEntity entity) where TEntity : class;
    }
}
