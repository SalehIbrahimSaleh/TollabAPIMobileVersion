using DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Z.BulkOperations;

namespace DataAccess.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {

       public IConnectionFactory _connectionFactory;


        public GenericRepository()
        {
            this._connectionFactory = new ConnectionFactory();

        }

        #region CRD Operation
        public async Task<long> Add(TEntity entity)
        {

            return await _connectionFactory.GetConnection.InsertAsync(entity);
        }
        
        public virtual async Task<bool> Update(TEntity entity)
        {
             return await _connectionFactory.GetConnection.UpdateAsync(entity);
        }

        public bool Delete(TEntity entity)
        {
           var d= _connectionFactory.GetConnection.Delete(entity);
            if (d)
            {
                return d;
            }
            return false;
        }
        public bool DeleteWhere(string where)
        {
            if (string.IsNullOrEmpty( where))
            {
                return false;
            }
            var query = $"delete  from {typeof(TEntity).Name} "+where;


            var d = _connectionFactory.GetConnection.Execute(query);
            if (d>0)
            {
                return true;
            }
            return false;
        }


        #endregion



        public async Task<TEntity> Get(long? Id)
        {
            try
            {
            var query = $"select * from [{typeof(TEntity).Name}] where Id ="+Id;
            var order = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<TEntity>(query);

            return order;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<TEntity> GetWhere(string where)
        {
            try
            {
                var query = $"select * from [{typeof(TEntity).Name}]";

                if (!string.IsNullOrWhiteSpace(where))
                    query += where;
                var order = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<TEntity>(query);

                return order;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public async Task<IEnumerable<TEntity>> GetAll(string where)
        {

            var query = $"select * from [{typeof(TEntity).Name}]";

            if (!string.IsNullOrWhiteSpace(where))
                query += where;

            var list = await _connectionFactory.GetConnection.QueryAsync<TEntity>(query);

            return list;


        }
        public async Task<IEnumerable<TEntity>> GetAll()
        {

            var query = $"select * from [{typeof(TEntity).Name}]";
             

            var list = await _connectionFactory.GetConnection.QueryAsync<TEntity>(query);

            return list;


        }


        public async Task<IEnumerable<TEntity>> GetAllByQuery(string query)
        {
           var list = await _connectionFactory.GetConnection.QueryAsync<TEntity>(query);
            return list;
        }

        public async Task<TEntity> GetOneByQuery(string query)
        {
            

            var one = await _connectionFactory.GetConnection.QueryFirstOrDefaultAsync<TEntity>(query);

            return one;
        }


    }
}
